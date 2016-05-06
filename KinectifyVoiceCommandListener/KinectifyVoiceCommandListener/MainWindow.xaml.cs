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

		/// <summary>
		/// Initializes a new instance of the MainWindow class.
		/// </summary>
		public MainWindow()
		{
			WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
			this.InitializeComponent();
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
				var grammar = new Choices();
				grammar.Add(new SemanticResultValue("exit program", "EXIT_PROGRAM"));
				grammar.Add(new SemanticResultValue("open note pad", "OPEN_NOTEPAD"));
				
				var gb = new GrammarBuilder { Culture = ri.Culture };
				gb.Append(grammar);
				
				var g = new Grammar(gb);
		
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

		/// <summary>
		/// Execute un-initialization tasks.
		/// </summary>
		/// <param name="sender">object sending the event.</param>
		/// <param name="e">event arguments.</param>
		private void WindowClosing(object sender, CancelEventArgs e)
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
				string voicCommand = e.Result.Semantics.Value.ToString();

				if (voicCommand == "EXIT_PROGRAM")
				{
					SendKeys.SendWait("%{F4}");
				}
				else if (voicCommand == "OPEN_NOTEPAD")
				{
					SendKeys.SendWait("^{ESC} notepad~");
				}
				//else
				//{
				//	string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Jordan\Documents\Code Samples\Kinect Examples\SpeechBasics-WPF\GrammarActionDictionary.txt");

				//	Dictionary<string, string> vcActions = new Dictionary<string, string>();

				//	foreach (string line in lines)
				//	{
				//		string delimiter = "\t";
				//		string[] vcParts = line.Split(delimiter.ToCharArray());

				//		vcActions.Add(vcParts[0], vcParts[1]);
				//	}

				//	string action = vcActions[voicCommand];

				//	SendKeys.SendWait(action);
				//}

			}
		}

		/// <summary>
		/// Handler for rejected speech events.
		/// </summary>
		/// <param name="sender">object sending the event.</param>
		/// <param name="e">event arguments.</param>
		private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
		{
			//this.ClearRecognitionHighlights();////////////////////////////////////////////////////////////fix this later
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start("http://kinectify.azurewebsites.net");
		}
	}
}