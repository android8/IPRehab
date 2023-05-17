using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserModel;

namespace IPRehabWebAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterReportsUserController : ControllerBase
    {
        private readonly IUserPatientCacheHelper _userPatientCacheHelper;

        public MasterReportsUserController(IUserPatientCacheHelper userPatientCacheHelper)
        {
            _userPatientCacheHelper = userPatientCacheHelper;
        }

        /// <summary>
        /// get user by executing stored procedure at MasterReports.uspVSSCMain_SelectAccessInformationFromNSSD
        /// </summary>
        /// <param name="networkID"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{*networkID}")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<MastUserDTO>>> GetUserPermission(string networkID)
        {
            var userAccessLevels = await _userPatientCacheHelper.GetUserAccessLevels(networkID);
            if (userAccessLevels == null || !userAccessLevels.Any())
            {
                return NotFound(@"You don't have any access level (access denied)");
            }

            return Ok(userAccessLevels);
        }
    }
}
