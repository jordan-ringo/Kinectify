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
    public class UserProfilesWebController : ApiController
    {
        private KinectifyContext db = new KinectifyContext();

        // GET: api/UserProfilesWeb
        public IQueryable<UserProfile> GetUserProfiles()
        {
            return db.UserProfiles;
        }

        // GET: api/UserProfilesWeb/5
        [ResponseType(typeof(UserProfile))]
        public async Task<IHttpActionResult> GetUserProfile(int id)
        {
            UserProfile userProfile = await db.UserProfiles.FindAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }

            return Ok(userProfile);
        }

        // PUT: api/UserProfilesWeb/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserProfile(int id, UserProfile userProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userProfile.ID)
            {
                return BadRequest();
            }

            db.Entry(userProfile).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProfileExists(id))
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

        // POST: api/UserProfilesWeb
        [ResponseType(typeof(UserProfile))]
        public async Task<IHttpActionResult> PostUserProfile(UserProfile userProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserProfiles.Add(userProfile);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = userProfile.ID }, userProfile);
        }

        // DELETE: api/UserProfilesWeb/5
        [ResponseType(typeof(UserProfile))]
        public async Task<IHttpActionResult> DeleteUserProfile(int id)
        {
            UserProfile userProfile = await db.UserProfiles.FindAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }

            db.UserProfiles.Remove(userProfile);
            await db.SaveChangesAsync();

            return Ok(userProfile);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserProfileExists(int id)
        {
            return db.UserProfiles.Count(e => e.ID == id) > 0;
        }
    }
}