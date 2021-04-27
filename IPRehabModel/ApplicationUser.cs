using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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

