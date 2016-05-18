using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Kinectify.Models;

namespace Kinectify.Controllers
{
    public class VoiceCommandsWebController : ApiController
    {
        private KinectifyContext db = new KinectifyContext();

        // GET: api/VoiceCommandsWeb
        public IQueryable<VoiceCommand> GetVoiceCommands()
        {
            return db.VoiceCommands;
        }

        // GET: api/VoiceCommandsWeb/5
        [ResponseType(typeof(VoiceCommand))]
        public async Task<IHttpActionResult> GetVoiceCommand(int id)
        {
            VoiceCommand voiceCommand = await db.VoiceCommands.FindAsync(id);
            if (voiceCommand == null)
            {
                return NotFound();
            }

            return Ok(voiceCommand);
        }

        // PUT: api/VoiceCommandsWeb/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutVoiceCommand(int id, VoiceCommand voiceCommand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != voiceCommand.ID)
            {
                return BadRequest();
            }

            db.Entry(voiceCommand).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoiceCommandExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/VoiceCommandsWeb
        [ResponseType(typeof(VoiceCommand))]
        public async Task<IHttpActionResult> PostVoiceCommand(VoiceCommand voiceCommand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.VoiceCommands.Add(voiceCommand);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = voiceCommand.ID }, voiceCommand);
        }

        // DELETE: api/VoiceCommandsWeb/5
        [ResponseType(typeof(VoiceCommand))]
        public async Task<IHttpActionResult> DeleteVoiceCommand(int id)
        {
            VoiceCommand voiceCommand = await db.VoiceCommands.FindAsync(id);
            if (voiceCommand == null)
            {
                return NotFound();
            }

            db.VoiceCommands.Remove(voiceCommand);
            await db.SaveChangesAsync();

            return Ok(voiceCommand);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VoiceCommandExists(int id)
        {
            return db.VoiceCommands.Count(e => e.ID == id) > 0;
        }
    }
}