using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UserModel;

namespace IPRehabWebAPI2.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class MasterReportsUserController : ControllerBase
  {
    private readonly MasterreportsContext _context;


    public MasterReportsUserController(MasterreportsContext context)
    {
      _context = context;
    }

    /// <summary>
    /// get user by executing stored procedure at MasterReports.uspVSSCMain_SelectAccessInformationFromNSSD
    /// </summary>
    /// <param name="networkID"></param>
    /// <returns></returns>
    [HttpGet()]
    public async Task<ActionResult<uspVSSCMain_SelectAccessInformationFromNSSDResult>> GetUserPermission(string networkID)
    {
      var paramNetworkID = new SqlParameter[]
      {
        new SqlParameter(){
          ParameterName = "@UserName",
          SqlDbType = System.Data.SqlDbType.VarChar,
          Direction = System.Data.ParameterDirection.Input,
          Value = networkID
        }
      };

      var userPermission = await _context.uspVSSCMain_SelectAccessInformationFromNSSDResult
        .FromSqlRaw($"execute [Apps].[uspVSSCMain_SelectAccessInformationFromNSSD] @UserName", paramNetworkID)
        .ToListAsync();
      return Ok(userPermission);
    }
  }
}
