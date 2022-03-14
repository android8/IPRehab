using System.Collections.Generic;

namespace IPRehab.Helpers
{
  public static class EpisodeCommandButtonSettings
  {
    public static Dictionary<string, CommandBtnConfig> CommandBtnConfigDictionary = new Dictionary<string, CommandBtnConfig>()
    {
      { "New", new(){ ButtonTitle="New", ButtonCss="actionBtnNew", ButtonTooltip="Create new episode with Base form"} },
      //{ "Full", "actionBtnFull"},
      { "Base", new(){ ButtonTitle="Base", ButtonCss="actionBtnBase", ButtonTooltip="Create Base questions"}},
      //{ "Initial", "actionBtnInitial"},
      { "Interim", new(){ ButtonTitle="Interim", ButtonCss="actionBtnInterim", ButtonTooltip="Create Interim questions"}},
      //{ "Discharge", "actionBtnDischarge"},
      { "Followup", new(){ ButtonTitle="Follow Up", ButtonCss="actionBtnFollowup", ButtonTooltip="Create Follow Up questions"}},
      { "Patient", new(){ ButtonTitle="Patient", ButtonCss="actionBtnPatientList", ButtonTooltip="Patient List"}}
    };
  }

  public class CommandBtnConfig {
    public string ButtonTitle { get; set; }
    public string ButtonCss { get; set; }
    public string ButtonTooltip { get; set;}
  }
}
