using System.Collections.Generic;

namespace IPRehabWebAPI2.Models
{
  public class UserFacilityGrant
  {
    public List<string> Facility { get; set; }
    public List<string> District { get; set; }
    public List<string> Division { get; set; }

    public UserFacilityGrant()
    {
      Facility = new List<string>();
      District = new List<string>();
      Division = new List<string>();
    }
  }
}