﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehab.Models
{
  public class InputViewComponenViewModel
  {
    public int ControlCounter { get; set; }
    public string UserID { get; set; }
    public int EpisodeID { get; set; }
    public int QuestionID { get; set; }
    public string QuestionKey { get; set; }
    public string StageTitle { get; set; }
    public string StageTitleNormalized { get; set; }
    public int StageID { get; set; }
    public bool MultipleChoices { get; set; }
    public bool Required { get; set; }
    public bool DisplayStageHeader { get; set; }
    public string StageHeaderBorderCssClass { get; set; }
    public string ContainerCssClass { get; set; }

    /// <summary>
    /// default code set id for therapy length in hours only used for DropDownPT view component template  
    /// </summary>
    public int TherapyHoursCodeSetID { get; set; }
    public string TherapyHoursCodeSetValue { get; set; }
  }
}