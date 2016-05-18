using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;

namespace KinectifyVoiceCommandListener
{
	


	/// <summary>
	/// Interaction logic for MainWindow
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "In a full-fledged application, the SpeechRecognitionEngine object should be properly disposed. For the sake of simplicity, we're omitting that code in this sample.")]
	
	
	public partial class MainWindow : Window
	{

		/// <summary>
		/// Active Kinect sensor.
		/// </summary>
		private KinectSensor kinectSensor = null;

		/// <summary>
		/// Stream for 32b-16b conversion.
		/// </summary>
		private KinectAudioStream convertStream = null;

		/// <summary>
		/// Speech recognition engine using audio data from Kinect.
		/// </summary>
		private SpeechRecognitionEngine speechEngine = null;


		private UserProfile userProfile1 = new UserProfile();
		private UserProfile userProfile2 = new UserProfile();
		private UserProfile userProfile3 = new UserProfile();

		private List<VoiceCommand> voiceCommandList = new List<VoiceCommand>();
		private Dictionary<string, string> voiceCommandActions = new Dictionary<string, string>();

		private bool voiceEnabled = false;
		private bool kinectNamesEnabled = false;
		private static bool kinectNameRecognized = false;
		private string currentKinectName;

		private BitmapImage vcReadyImage = new BitmapImage(new Uri("/KinectifyVoiceCommandListener;component/Images/small-kinect-icon-green.png", UriKind.Relative));
		private BitmapImage vcNotReadyImage = new BitmapImage(new Uri("/KinectifyVoiceCommandListener;component/Images/small-kinect-icon-black.png", UriKind.Relative));

		private DispatcherTimer voiceCommandTimer = new DispatcherTimer();

		ResourceDictionary myResourceDictionary = new ResourceDictionary();
		


		/// <summary>
		/// Initializes a new instance of the MainWindow class.
		/// </summary>
		public MainWindow()
		{
			WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			this.InitializeComponent();
			myResourceDictionary.Source = new Uri("/KinectifyVoiceCommandListener;component/NotifyIconResources.xaml", UriKind.Relative);
		}



		/// <summary>
		/// Gets the metadata for the speech recognizer (acoustic model) most suitable to
		/// process audio from Kinect device.
		/// </summary>
		/// <returns>
		/// RecognizerInfo if found, <code>null</code> otherwise.
		/// </returns>
		private static RecognizerInfo TryGetKinectRecognizer()
		{
			IEnumerable<RecognizerInfo> recognizers;

			// This is required to catch the case when an expected recognizer is not installed.
			// By default - the x86 Speech Runtime is always expected. 
			try
			{
				recognizers = SpeechRecognitionEngine.InstalledRecognizers();
			}
			catch (COMException)
			{
				return null;
			}

			foreach (RecognizerInfo recognizer in recognizers)
			{
				string value;
				recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
				if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
				{
					return recognizer;
				}
			}

			return null;
		}

		/// <summary>
		/// Execute initialization tasks.
		/// </summary>
		/// <param name="sender">object sending the event</param>
		/// <param name="e">event arguments</param>
		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			GetUserProfiles();

		}

		/// <summary>
		/// Execute un-initialization tasks.
		/// </summary>
		/// <param name="sender">object sending the event.</param>
		/// <param name="e">event arguments.</param>
		private void WindowClosing(object sender, CancelEventArgs e)
		{
			StopListening();
		}

		private void StartListening(string kinectName)
		{
			// Only one sensor is supported
			this.kinectSensor = KinectSensor.GetDefault();

			if (this.kinectSensor != null)
			{
				// open the sensor
				this.kinectSensor.Open();

				// grab the audio stream
				IReadOnlyList<AudioBeam> audioBeamList = this.kinectSensor.AudioSource.AudioBeams;
				System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

				// create the convert stream
				this.convertStream = new KinectAudioStream(audioStream);
			}
			else
			{
				// on failure, set the status text
				//this.statusBarText.Text = Properties.Resources.NoKinectReady;///////////////////////////////////////////////////////////////
				return;
			}

			RecognizerInfo ri = TryGetKinectRecognizer();

			if (null != ri)
			{
				this.speechEngine = new SpeechRecognitionEngine(ri.Id);

				// Create a grammar from grammar definition XML file.
				//using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
				//{
				//	var g = new Grammar(memoryStream);
				//	this.speechEngine.LoadGrammar(g);
				//}


				//Use this code to create grammar programmatically rather than froma grammar file.
				var g = BuildGrammar(ri);

				//var grammar = new Choices();

				//foreach( VoiceCommand vc in voiceCommandList)
				//{
				//	grammar.Add(new SemanticResultValue(vc.Phrase.ToString(), vc.Keyword.ToString()));
				//}
				//grammar.Add(new SemanticResultValue("exit program", "EXIT_PROGRAM"));

				//var gb = new GrammarBuilder { Culture = ri.Culture };
				//gb.Append(grammar);

				//var g = new Grammar(gb);

				this.speechEngine.LoadGrammar(g);

				this.speechEngine.SpeechRecognized += this.SpeechRecognized;
				this.speechEngine.SpeechRecognitionRejected += this.SpeechRejected;

				// let the convertStream know speech is going active
				this.convertStream.SpeechActive = true;

				// For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
				// This will prevent recognition accuracy from degrading over time.
				////speechEngine.UpdateRecognizerSetting("AdaptationOn", 0);

				this.speechEngine.SetInputToAudioStream(
					this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
				this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
			}
			else
			{
				//this.statusBarText.Text = Properties.Resources.NoSpeechRecognizer;///////////////////////////////add later
				/////delete later
			}
		}

		private void StopListening()
		{
			if (null != this.convertStream)
			{
				this.convertStream.SpeechActive = false;
			}

			if (null != this.speechEngine)
			{
				this.speechEngine.SpeechRecognized -= this.SpeechRecognized;
				this.speechEngine.SpeechRecognitionRejected -= this.SpeechRejected;
				this.speechEngine.RecognizeAsyncStop();
			}

			if (null != this.kinectSensor)
			{
				this.kinectSensor.Close();
				this.kinectSensor = null;
			}
		}
	

		/// <summary>
		/// Handler for recognized speech events.
		/// </summary>
		/// <param name="sender">object sending the event.</param>
		/// <param name="e">event arguments.</param>
		private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			// Speech utterance confidence below which we treat speech as if it hadn't been heard
			const double ConfidenceThreshold = 0.9;

			if (e.Result.Confidence >= ConfidenceThreshold)
			{
				// First check if voice commands are enabled.
				if (voiceEnabled == true)
				{
					// Since voice commands are enables, we get the voice command (keyword).
					string voicCommand = e.Result.Semantics.Value.ToString();

					// If Kinect Names are NOT enabled, we do this...
					if (kinectNamesEnabled == false)
					{
						if (voicCommand == "EXIT_PROGRAM")
						{
							SendKeys.SendWait("%{F4}");
						}
						else if (voicCommand != currentKinectName)
						{
							string action = voiceCommandActions[voicCommand];

							SendKeys.SendWait(action);
							RecentVoiceCommand.Content = action;
						}
					}
					else // If Kinect Names ARE enabled we do this...
					{
						// Check to see if the user said the Kinect Nickname.
						if (voicCommand == currentKinectName)
						{
							kinectNameRecognized = true;
							// We now need to start a timer that allows the user to say a voice command.
							// If there is a previous timer... stop it first.
							voiceCommandTimer.Stop();
							voiceCommandTimer.Tick += new EventHandler(voiceCommandTimer_Tick);
							voiceCommandTimer.Interval = new TimeSpan(0,0,5); // Set the Timer to 5 seconds.
							KinectIcon.Source = vcReadyImage; // change the kinect icon to show that it's listening.
							voiceCommandTimer.Start();
						}

						// If the kinect name is recognized (and the above timer is still going) we can perform the action.
						if (kinectNameRecognized == true)
						{
							if (voicCommand == "EXIT_PROGRAM")
							{
								SendKeys.SendWait("%{F4}");
							}
							else if (voicCommand != currentKinectName)
							{
								string action = voiceCommandActions[voicCommand];

								SendKeys.SendWait(action);
								RecentVoiceCommand.Content = action;
							}
						}
					}

				}
			}
		}

		/// <summary>
		/// Handler for rejected speech events.
		/// </summary>
		/// <param name="sender">object sending the event.</param>
		/// <param name="e">event arguments.</param>
		private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
		{
			RecentVoiceCommand.Content = "Voice Command not recognized.";
		}


		private Grammar BuildGrammar(RecognizerInfo ri)
		{
			//Use this code to create grammar programmatically rather than froma grammar file.
			var grammar = new Choices();

			foreach (VoiceCommand vc in voiceCommandList)
			{
				grammar.Add(new SemanticResultValue(vc.Phrase.ToString(), vc.Keyword.ToString()));
			}

			// Add the current users Kinect Name as a voice command.
			grammar.Add(new SemanticResultValue(currentKinectName.ToLower(), currentKinectName));

			var gb = new GrammarBuilder { Culture = ri.Culture };
			gb.Append(grammar);

			var g = new Grammar(gb);

			return g;
		}



		private void GetVoiceCommands(int id)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://kinectify.azurewebsites.net/");

            var url = "api/VoiceCommandsWeb/";

            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
				string myVC = "";

				IEnumerable<VoiceCommand> voiceCommands = response.Content.ReadAsAsync<IEnumerable<VoiceCommand>>().Result;
				foreach (VoiceCommand vc in voiceCommands)
				{
					if (vc.UserProfileID == id)
					{
						voiceCommandList.Add(vc);
						voiceCommandActions.Add(vc.Keyword, vc.Action);
						myVC += vc.Keyword;
					}
				}

				System.Windows.MessageBox.Show("Voice Commands : " + myVC);

            }
            else
            {
                System.Windows.MessageBox.Show("Error Code" + 
                response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }

		private void GetUserProfiles()
		{
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri("http://kinectify.azurewebsites.net/");

			var url = "api/UserProfilesWeb/";

			HttpResponseMessage response = client.GetAsync(url).Result;

			if (response.IsSuccessStatusCode)
			{
				int count = 0;

				IEnumerable<UserProfile> userProfile = response.Content.ReadAsAsync<IEnumerable<UserProfile>>().Result;
				foreach (UserProfile up in userProfile)
				{
					++count;

					if (count == 1)
					{ 
						userProfile1 = up; 
						User1Button.Content = up.UserName;
						User1KinectName.Content = up.KinectName.Trim(); 
					}
					if (count == 2)
					{ 
						userProfile2 = up; 
						User2Button.Content = up.UserName;
						User2KinectName.Content = up.KinectName.Trim();
					}
					if (count == 3) 
					{ 
						userProfile3 = up; 
						User3Button.Content = up.UserName;
						User3KinectName.Content = up.KinectName.Trim();
					}

				}
			}
			else
			{
				System.Windows.MessageBox.Show("Error Code" +
				response.StatusCode + " : Message - " + response.ReasonPhrase);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://kinectify.azurewebsites.net");
		}


		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
				System.Windows.Application.Current.Shutdown();
		}
		private void btnMinimize_Click(object sender, RoutedEventArgs e)
		{
				this.WindowState = WindowState.Minimized;
		}


		private void User1Button_Click(object sender, RoutedEventArgs e)
		{
			if(userProfile1.UserName == "New User")
			{
				System.Diagnostics.Process.Start("http://kinectify.azurewebsites.net");
			}
			else
			{
				StopListening();
				GetVoiceCommands(userProfile1.ID);
				currentKinectName = userProfile1.KinectName.Trim().ToUpper();
				User1Button.Background = Brushes.White;
				User2Button.Background = Brushes.Gray;
				User3Button.Background = Brushes.Gray;
				StartListening(currentKinectName);
			}
		}

		private void User2Button_Click(object sender, RoutedEventArgs e)
		{
			if (userProfile2.UserName == "New User")
			{
				System.Diagnostics.Process.Start("http://kinectify.azurewebsites.net");
			}
			else
			{
				StopListening();
				GetVoiceCommands(userProfile2.ID);
				currentKinectName = userProfile2.KinectName.Trim().ToUpper();
				User1Button.Background = Brushes.Gray;
				User2Button.Background = Brushes.White;
				User3Button.Background = Brushes.Gray;
				StartListening(currentKinectName);
			}
		}

		private void User3Button_Click(object sender, RoutedEventArgs e)
		{
			if (userProfile3.UserName == "New User")
			{
				System.Diagnostics.Process.Start("http://kinectify.azurewebsites.net");
			}
			else
			{
				StopListening();
				GetVoiceCommands(userProfile3.ID);
				currentKinectName = userProfile3.KinectName.Trim().ToUpper();
				User1Button.Background = Brushes.Gray;
				User2Button.Background = Brushes.Gray;
				User3Button.Background = Brushes.White;
				StartListening(currentKinectName);
			}
		}

		private void VoiceEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			voiceEnabled = true;
			KinectIcon.Source = vcReadyImage;
			showCustomBalloon();
		}
		private void VoiceEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			voiceEnabled = false;
			KinectIcon.Source = vcNotReadyImage;
		}

		private void UseKinectNameCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			kinectNamesEnabled = true;
			KinectIcon.Source = vcNotReadyImage;
		}
		private void UseKinectNameCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			kinectNamesEnabled = false;
			if (voiceEnabled == true)
			{
				KinectIcon.Source = vcReadyImage;
			}
		}



		private void voiceCommandTimer_Tick(object sender, EventArgs e)
		{
			((DispatcherTimer)sender).Stop(); // Make sure the time only activates once (not repeatedly).
			kinectNameRecognized = false;
			KinectIcon.Source = vcNotReadyImage;
		}


		private void showCustomBalloon()
		{
			FancyBalloon balloon = new FancyBalloon();
			balloon.BalloonText = "";
			//show and close after 2.5 seconds
			TaskbarIcon tb = new TaskbarIcon();
			tb = myResourceDictionary["NotifyIcon"] as TaskbarIcon;
			tb.ShowCustomBalloon(balloon, PopupAnimation.Slide, 5000);
		}
	
	}
}