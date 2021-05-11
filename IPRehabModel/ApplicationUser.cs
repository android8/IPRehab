using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPRehabModel
{
   //[Keyless]
   public class ApplicationUser: IdentityUser
   {
      [PersonalData]
      [Column(TypeName="varchar(30)")]
      public string FirstName { get; set; }

      [PersonalData]
      [Column(TypeName = "varchar(30)")]
      public string LastName { get; set; }
   }
}

