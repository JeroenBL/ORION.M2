#nullable disable
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ORION.M2.API.Service.Sample;
using ORION.M2.DbContext.Data;
using ORION.M2.DbContext.Models;

namespace ORION.M2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ISampleService<Person> _personSampleService;

        public PersonController(ApplicationDbContext db, ISampleService<Person> personSampleService)
        {
            _db = db;
            _personSampleService = personSampleService;
        }

        // GET: api/Person
        /// <summary>
        /// Retrieve all persons from the ORION.M2 database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPerson()
        {
            return await _db.Person.ToListAsync();
        }

        /// <summary>
        /// Retrieve single person (by id) from the ORION.M2 database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Person/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _db.Person.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        // PATCH: api/person/5
        /// <summary>
        /// Patch a person (by id) in the ORION.M2 database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchPerson(int id, [FromBody]JsonPatchDocument<Person> person)
        {
            var entity = await _db.Person.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            entity.DateUpdated = DateTime.Now.ToUniversalTime();
            person.ApplyTo(entity, ModelState);
            await _db.SaveChangesAsync();

            return Ok(entity);
        }

        // POST: api/Person
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Create a new person in the ORION.M2 database
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            _db.Person.Add(person);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
        }

        // DELETE: api/Person/5
        /// <summary>
        /// Delete a person (by id) from the ORION.M2 database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _db.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _db.Person.Remove(person);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Person/createSamplePersons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Create sample persons using the bogus nuget library
        /// </summary>
        /// <param name="amount">The amount of sample persons you wish to create</param>
        /// <param name="locale">The local of the sample persons. Like 'en'. For a complete overview see: https://github.com/bchavez/Bogus </param>
        /// <returns></returns>
        [HttpPost("createSamplePersons")]
        [Consumes("application/json")]
        public async Task<ActionResult<Person>> CreateSamplePersons(int amount, string locale)
        {
            var persons = _personSampleService.Create(amount, locale);
            if (persons != null)
            {
                foreach (var person in persons)
                {
                    await _db.Person.AddAsync(person);
                    await _db.SaveChangesAsync();
                }
            }
            return Ok(persons);
        }

        // DELETE: api/Person/all
        /// <summary>
        /// Remove all persons (both sample and non-sample) from the database
        /// </summary>
        /// <returns></returns>
        [HttpDelete("all")]
        public async Task<ActionResult> EmptyDatabase()
        {
            var persons = await _db.Person.ToListAsync();
            if (persons != null)
            {
                _db.Person.RemoveRange(persons);
                await _db.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NoContent();
            }
        }

        private bool PersonExists(int id)
        {
            return _db.Person.Any(e => e.Id == id);
        }
    }
}
