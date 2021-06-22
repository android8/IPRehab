using IPRehab.Models; //model folder
using IPRehabModel; //model project
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace PCC_FIT.Helpers
{
  public static class StateManager
  {
    static UserManager<ApplicationUser> _userManager = new UserManager<ApplicationUser>();

    public static UserProfile GetUserProfileCookie(IHttpContextAccessor _httpContextAccessor, string key)
    {
      //settor injection of HttpContextAccessor
      string userProfileCookie = _httpContextAccessor.HttpContext.Request.Cookies[key];

      if (string.IsNullOrEmpty(userProfileCookie))
      {
        return null;
      }
      else
      {
        UserProfile userProfile = new UserProfile();
        userProfile = System.Text.Json.JsonSerializer.Deserialize<UserProfile>(userProfileCookie);

        return userProfile;
      }
    }

    public static async Task<UserProfile> CreateUserProfileAsync(IHttpContextAccessor _httpContextAccessor, ClaimsPrincipal principal)
    {
      UserProfile thisUserProfile = new UserProfile();
      thisUserProfile.LastName = string.Empty;
      thisUserProfile.FirstName = string.Empty;
      thisUserProfile.NetworkName = string.Empty;
      thisUserProfile.Email = string.Empty;
      thisUserProfile.UserID = 0;

      //attemp 1: use CORE UserManager to find user by pricipal
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      /// No need to iterate the principal.Identities if the GetIdentityFromPrincipal(userManager, principal)     /////
      /// could not find the identity, likely because the identities are null or the names are null.              /////
      /// By default, the UserManager.FindByNameAsync() searches the AspNetUsers.NormalizedUserName column.       /////
      /// Change the AutoMapper's mapping proifle to store NetworkID in the NormalizedUserName column at the      /////
      /// user registration                                                                                       /////
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      ApplicationUser customIdentityUser = await GetIdentityFromPrincipal(principal);

      if (customIdentityUser != null)
      {
        thisUserProfile.LastName = customIdentityUser.LastName?.ToUpper();
        thisUserProfile.FirstName = customIdentityUser.FirstName?.ToUpper();
        thisUserProfile.NetworkName = customIdentityUser.UserName?.ToUpper();
        thisUserProfile.Email = customIdentityUser.Email?.ToUpper();
        thisUserProfile.UserID = customIdentityUser.Id;
      }
      else
      {
        //attemp 2: repository lookup directly using Widnows Identity
        //WindowsIdentity wi = GetIdentityFromWindowsIdentity();
        //if (wi != null && !string.IsNullOrEmpty(wi.Name))
        //{
        //thisUserProfile.LastName = wi.GetUserId().?.ToUpper();
        //thisUserProfile.FirstName = mappedStaff.StaffFirstName?.ToUpper();
        //thisUserProfile.NetworkName = mappedStaff.StaffNetworkId?.ToUpper();
        //thisUserProfile.Email = mappedStaff.StaffEmail?.ToUpper();
        //thisUserProfile.UserID = mappedStaff.StaffStaffId;
        //thisUserProfile.FoundBy = UserFoundByEnum.Windows;
        //userFoundBy = UserFoundByEnum.Windows;
        //breakForeach = true;  //break out foreach
        //}
      }
      return thisUserProfile;
    }

    public static void CreateUserProfileCookie(IHttpContextAccessor _httpContextAccessor, string key, UserProfile userProfile)
    {
      string serializedProfile = System.Text.Json.JsonSerializer.Serialize(userProfile);

      CookieOptions options = new CookieOptions()
      {
        //Expires = DateTime.Now.AddDays(1),
        IsEssential = true,
        Secure = true
      };
      _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
      _httpContextAccessor.HttpContext.Response.Cookies.Append(key, serializedProfile, options);
    }

    public static async Task<ApplicationUser> GetIdentityFromPrincipal(ClaimsPrincipal principal)
    {
      ApplicationUser customIdentityUser = await _userManager.GetUserAsync(principal);

      return customIdentityUser;
    }

    /// <summary>
    /// IIdentity is implemented by ChlaimsIdentity then inherited by WindowsIdentity
    /// </summary>
    /// <param name="_httpContextAccessor"></param>
    /// <returns></returns>
    public static IIdentity GetIdentityFromHttpContextAccessor(IHttpContextAccessor _httpContextAccessor)
    {
      IIdentity thisIdentity = null;
      if (_httpContextAccessor.HttpContext != null)
      {
        thisIdentity = _httpContextAccessor.HttpContext.User.Identity;
      }
      return thisIdentity;
    }

    /// <summary>
    /// WindowsIdentity inherits ClaimsIdentity implments IIdentity
    /// </summary>
    /// <returns></returns>
    public static WindowsIdentity GetIdentityFromWindowsIdentity()
    {
      WindowsIdentity thisIdentity = WindowsIdentity.GetCurrent();
      return thisIdentity;
    }

    public static ApplicationUser GetIdentityFromUserManager()
    {
      ApplicationUser thisIdentity = null;
      WindowsIdentity wi = GetIdentityFromWindowsIdentity();
      if (wi != null && !string.IsNullOrEmpty(wi.Name))
      {
        thisIdentity = _userManager.Users.Where(x => x.Id == StripDomain(wi.Name)).FirstOrDefault();
      }
      return thisIdentity;
    }


    public static string StripDomain(string thisNetworkName)
    {
      if (thisNetworkName.IndexOf('\\') != -1)
      {
        return thisNetworkName.Split('\\')[1];
      }

      return thisNetworkName;
    }

    public static string GetDomain(string thisNetworkName)
    {
      if (thisNetworkName.IndexOf('\\') != -1)
      {
        return thisNetworkName.Split('\\')[0];
      }

      return thisNetworkName;
    }
  }
}
