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
  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly IPRehabContext _context;

    public UserController(IPRehabContext context)
    {
      _context = context;
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<tblUser>>> GetUser()
    {
      return await _context.tblUser.ToListAsync();
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
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

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
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

      await _context.SaveChangesAsync();

      return NoContent();
    }

    // POST: api/User
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<tblUser>> PostUser(tblUser tblUser)
    {
      _context.tblUser.Add(tblUser);
      await _context.SaveChangesAsync();
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
