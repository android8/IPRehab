using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Models
{
  public class MastUserDTO
  {
    public int UserID { get; set; }
    public int UserIdentity { get; set; }
    public string NTDomain { get; set; }
    public string NTUserName { get; set; }
    public string VISN { get; set; }
    public string Facility { get; set; }
    public string LName { get; set; }
    public string FName { get; set; }
    public string AppID { get; set; }
    public string AcclevID { get; set; }
    public string CPRSnssd { get; set; }
    public string Sunsetdat { get; set; }
  }
}
