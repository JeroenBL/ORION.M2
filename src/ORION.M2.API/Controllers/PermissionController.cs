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
        /// <response code="200"></response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Permission>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Permission>>> GetPermission()
        {
            return Ok(await _db.Permission.ToListAsync());
        }

        // GET: api/Permission/5
        /// <summary>
        /// Retrieve single permission (by id) from the ORION.M2 database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="404">Permission not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Permission), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Permission>> GetPermission(int id)
        {
            var permission = await _db.Permission.FindAsync(id);

            if (permission == null)
            {
                return NotFound();
            }

            return Ok(permission);
        }

        // PATCH: api/person/5
        /// <summary>
        /// Patch a permission (by id) in the ORION.M2 database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        /// <response code="200">Permission created</response>
        /// <response code="404">Permission not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(Permission), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
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
        /// <response code="200">Permission created</response>
        /// <response code="400">Permission has missing/invalid values</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(Permission), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
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
        /// <response code="204">Permission deleted</response>
        /// <response code="404">Permission not found</response>
        /// <response code="500">Internal server error</response> 
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
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
