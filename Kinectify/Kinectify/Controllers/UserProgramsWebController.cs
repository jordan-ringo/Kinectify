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
    public class UserProgramsWebController : ApiController
    {
        private KinectifyContext db = new KinectifyContext();

        // GET: api/UserProgramsWeb
        public IQueryable<UserProgram> GetUserPrograms()
        {
            return db.UserPrograms;
        }

        // GET: api/UserProgramsWeb/5
        [ResponseType(typeof(UserProgram))]
        public async Task<IHttpActionResult> GetUserProgram(int id)
        {
            UserProgram userProgram = await db.UserPrograms.FindAsync(id);
            if (userProgram == null)
            {
                return NotFound();
            }

            return Ok(userProgram);
        }

        // PUT: api/UserProgramsWeb/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserProgram(int id, UserProgram userProgram)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userProgram.ID)
            {
                return BadRequest();
            }

            db.Entry(userProgram).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProgramExists(id))
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

        // POST: api/UserProgramsWeb
        [ResponseType(typeof(UserProgram))]
        public async Task<IHttpActionResult> PostUserProgram(UserProgram userProgram)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserPrograms.Add(userProgram);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = userProgram.ID }, userProgram);
        }

        // DELETE: api/UserProgramsWeb/5
        [ResponseType(typeof(UserProgram))]
        public async Task<IHttpActionResult> DeleteUserProgram(int id)
        {
            UserProgram userProgram = await db.UserPrograms.FindAsync(id);
            if (userProgram == null)
            {
                return NotFound();
            }

            db.UserPrograms.Remove(userProgram);
            await db.SaveChangesAsync();

            return Ok(userProgram);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserProgramExists(int id)
        {
            return db.UserPrograms.Count(e => e.ID == id) > 0;
        }
    }
}