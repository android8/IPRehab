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
        public async Task<ActionResult<IEnumerable<TblUser>>> GetUser()
        {
            return await _context.TblUser.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TblUser>> GetUser(int id)
        {
            var tblUser = await _context.TblUser.FindAsync(id);

            if (tblUser == null)
            {
                return NotFound();
            }

            return tblUser;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, TblUser tblUser)
        {
            if (id != tblUser.Id)
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
        public async Task<ActionResult<TblUser>> PostUser(TblUser tblUser)
        {
            _context.TblUser.Add(tblUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TblUserExists(tblUser.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTblUser", new { id = tblUser.Id }, tblUser);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var tblUser = await _context.TblUser.FindAsync(id);
            if (tblUser == null)
            {
                return NotFound();
            }

            _context.TblUser.Remove(tblUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TblUserExists(int id)
        {
            return _context.TblUser.Any(e => e.Id == id);
        }
    }
}
