using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserModel;

namespace IPRehabWebAPI2.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class MasterReportsUserController : ControllerBase
  {
    private readonly MasterreportsContext _masterReportsContext;

    public MasterReportsUserController(MasterreportsContext context)
    {
      _masterReportsContext = context;
    }

    /// <summary>
    /// get user by executing stored procedure at MasterReports.uspVSSCMain_SelectAccessInformationFromNSSD
    /// </summary>
    /// <param name="networkID"></param>
    /// <returns></returns>
    [Route("{*networkID}")]
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<MastUserDTO>>> GetUserPermission(string networkID)
    {
      var helper = new CacheHelper();
      var userAccessLevels = await helper.GetUserAccessLevels(_masterReportsContext, networkID);
      return Ok(userAccessLevels);
    }
  }
}
