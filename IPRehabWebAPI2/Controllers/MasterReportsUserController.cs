using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
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
    private readonly IUserPatientCacheHelper _userPatientCacheHelper;

    public MasterReportsUserController(MasterreportsContext context, IUserPatientCacheHelper userPatientCacheHelper)
    {
      _masterReportsContext = context;
      _userPatientCacheHelper = userPatientCacheHelper;
    }

    /// <summary>
    /// get user by executing stored procedure at MasterReports.uspVSSCMain_SelectAccessInformationFromNSSD
    /// </summary>
    /// <param name="networkID"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("{*networkID}")]
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<MastUserDTO>>> GetUserPermission(string networkID)
    {
      var userAccessLevels = await _userPatientCacheHelper.GetUserAccessLevels(networkID);
      return Ok(userAccessLevels);
    }
  }
}
