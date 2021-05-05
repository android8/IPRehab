using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IPRehabModel;

namespace IPRehabWebAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPRehabContext _context;

        public PatientController(IPRehabContext context)
        {
            _context = context;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblPatient>>> GetTblPatient()
        {
            return await _context.TblPatient.ToListAsync();
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblPatient>> GetTblPatient(string id)
        {
            var tblPatient = await _context.TblPatient.FindAsync(id);

            if (tblPatient == null)
            {
                return NotFound();
            }

            return tblPatient;
        }

        // PUT: api/Patients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTblPatient(string id, TblPatient tblPatient)
        {
            if (id != tblPatient.Icn)
            {
                return BadRequest();
            }

            _context.Entry(tblPatient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblPatientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Patients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TblPatient>> PostTblPatient(TblPatient tblPatient)
        {
            _context.TblPatient.Add(tblPatient);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblPatientExists(tblPatient.Icn))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblPatient", new { id = tblPatient.Icn }, tblPatient);
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTblPatient(string id)
        {
            var tblPatient = await _context.TblPatient.FindAsync(id);
            if (tblPatient == null)
            {
                return NotFound();
            }

            _context.TblPatient.Remove(tblPatient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblPatientExists(string id)
        {
            return _context.TblPatient.Any(e => e.Icn == id);
        }
    }
}
