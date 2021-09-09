using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class MDSubmitButtonViewModel
  {
    public int FacilityID { get; set; }
    public int FiscalYear { get; set; }
    public bool Cloning { get; set; }
    public int QuestionPageID { get; set; }
    public int StaffFacilityRelationID { get; set; }
  }
}
