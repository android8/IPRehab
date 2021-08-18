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
    public class UserController : ControllerBase
    {
        private readonly IPRehabContext _context;

        public UserController(IPRehabContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<tblUser>>> GetUser()
        {
            return await _context.tblUser.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<tblUser>> GetUser(int id)
        {
            var tblUser = await _context.tblUser.FindAsync(id);

            if (tblUser == null)
            {
                return NotFound();
            }

            return tblUser;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, tblUser tblUser)
        {
            if (id != tblUser.ID)
            {
                return BadRequest();
            }

            _context.Entry(tblUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblUserExists(id))
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

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<tblUser>> PostUser(tblUser tblUser)
        {
            _context.tblUser.Add(tblUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblUserExists(tblUser.ID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GettblUser", new { id = tblUser.ID }, tblUser);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var tblUser = await _context.tblUser.FindAsync(id);
            if (tblUser == null)
            {
                return NotFound();
            }

            _context.tblUser.Remove(tblUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblUserExists(int id)
        {
            return _context.tblUser.Any(e => e.ID == id);
        }
    }
}
