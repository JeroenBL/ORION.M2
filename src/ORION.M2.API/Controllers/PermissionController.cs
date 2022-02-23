#nullable disable
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ORION.M2.DbContext.Data;
using ORION.M2.DbContext.Models;

namespace ORION.M2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public PermissionController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/Permission
        /// <summary>
        /// Retrieve all permissions from the ORION.M2 database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Permission>>> GetPermission()
        {
            return await _db.Permission.ToListAsync();
        }

        /// <summary>
        /// Retrieve single permission (by id) from the ORION.M2 database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Permission/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Permission>> GetPermission(int id)
        {
            var permission = await _db.Permission.FindAsync(id);

            if (permission == null)
            {
                return NotFound();
            }

            return permission;
        }

        /// <summary>
        /// Patch a permission (by id) in the ORION.M2 database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        // PATCH: api/person/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchPermission(int id, [FromBody] JsonPatchDocument<Permission> permission)
        {
            var entity = await _db.Permission.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            permission.ApplyTo(entity, ModelState);
            await _db.SaveChangesAsync();

            return Ok(entity);
        }

        // POST: api/Permission
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Create a new permission in the ORION.M2 database
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Permission>> PostPermission(Permission permission)
        {
            _db.Permission.Add(permission);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetPermission", new { id = permission.Id }, permission);
        }

        // DELETE: api/Permission/5
        /// <summary>
        /// Delete a permission (by id) from the ORION.M2 database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var permission = await _db.Permission.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }

            _db.Permission.Remove(permission);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool PermissionExists(int id)
        {
            return _db.Permission.Any(e => e.Id == id);
        }
    }
}
