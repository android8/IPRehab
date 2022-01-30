using IPRehabWebAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class QuestionHierarchy
  {
    public bool ReadOnly { get; set; }
    public int EpisodeID { get; set; }
    public string PatientID { get; set; }
    public string PatientName { get; set;}
    public List<PatientEpisodeAndCommandVM> EpisodeBtnConfig { get; set; }
    public string StageTitle { get; set; }
    public string StageCode { get; set; }
    public string CurrentAction { get; set; }
    public string WebApiBaseUrl { get; set; }
    public string ModeColorCssClass { get; set; }
    public List<SectionInfo> Sections { get; set; }

    public QuestionHierarchy()
    {
      EpisodeBtnConfig = new();
      Sections = new();
    }
  }
}
