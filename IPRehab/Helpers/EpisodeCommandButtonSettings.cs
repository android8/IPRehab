using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Helpers
{
  public static class EpisodeCommandButtonSettings
  {
    public static Dictionary<string, string> stageType = new Dictionary<string, string>() {
      { "New", "Create new episode with Base form"},
      //{ "Full", "Create new episode with Full IRF-PAI form"},
      { "Base", "Create new episode with Base questions"},
      //{ "Initial", "Create new episode with Initial questions"},
      { "Interim", "Create new episode with Interim questions"},
      //{ "Discharge", "Create new episode with Discharge questions"},
      { "Followup", "Create new episode with Follow up questions"},
      { "Patient", "Patient List"}
    };

    public static Dictionary<string, string> actionBtnColor = new Dictionary<string, string>()
    {
      { "New", "actionBtnNew"},
      //{ "Full", "actionBtnFull"},
      { "Base", "actionBtnBase"},
      //{ "Initial", "actionBtnInitial"},
      { "Interim", "actionBtnInterim"},
      //{ "Discharge", "actionBtnDischarge"},
      { "Followup", "actionBtnFollowup"},
      { "Patient", "actionBtnPatientList"},
    };
  }
}
