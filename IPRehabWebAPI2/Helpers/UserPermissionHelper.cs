using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UserModel;

namespace IPRehabWebAPI2.Helpers
{
  public class UserPermissionHelper
  {
    private IMemoryCache _cache;
    private readonly string cacheKey = "UserAccessLevels";
    
    public UserPermissionHelper(IMemoryCache memoryCache)
    {
      _cache = memoryCache;
    }

    public async Task<List<MastUserDTO>> GetUserAccessLevels(MasterreportsContext context, string networkID)
    {
      
      string cachedUserAccessLevels = string.Empty;
      List<MastUserDTO> userAccessLevels = new List<MastUserDTO>();

      if (_cache.TryGetValue(cacheKey, out var userAccessLevelsFromSession))
      {
        userAccessLevels = (List<MastUserDTO>)userAccessLevelsFromSession;
        return userAccessLevels;
      }
      else
      {
        string networkName = networkID;
        if (!string.IsNullOrEmpty(networkName) && (networkName.Contains('\\') || networkName.Contains("%2F")))
        {
          String[] separator = { "\\", "%2F" };
          var networkNameWithDomain = networkName.Split(separator,StringSplitOptions.RemoveEmptyEntries);

          if (networkNameWithDomain.Length > 0)
            networkName = networkNameWithDomain[1];
          else
            networkName = networkNameWithDomain[0];
        }
        else
        {
          return null;
        }

        SqlParameter[] paramNetworkID = new SqlParameter[]
        {
          new SqlParameter(){
            ParameterName = "@UserName",
            SqlDbType = System.Data.SqlDbType.VarChar,
            Direction = System.Data.ParameterDirection.Input,
            Value = networkName
          }
        };

        //use dbContext extension method
        var userPermission = await context.SqlQueryAsync<uspVSSCMain_SelectAccessInformationFromNSSDResult>(
          $"execute [Apps].[uspVSSCMain_SelectAccessInformationFromNSSD] @UserName", paramNetworkID);

        foreach (var item in userPermission)
        {
          var userDTO = HydrateDTO.HydrateUser(item);
          userAccessLevels.Add(userDTO);
        }

        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
        {
          AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300), // cache will expire in 300 seconds or 5 minutes
          SlidingExpiration = TimeSpan.FromSeconds(60) // cache will expire if inactive for 60 seconds
        };

        _cache.Set<List<MastUserDTO>>(cacheKey, userAccessLevels, options);
      }
    return userAccessLevels;
    }
 }
}
