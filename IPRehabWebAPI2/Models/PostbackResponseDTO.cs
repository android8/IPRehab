using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Models
{
  public class PostbackResponseDTO
  {
    public string ExecptionMsg { get; set; }
    public string InnerExceptionMsg { get; set; }
    public List<UserAnswer> OriginalAnswers { get; set; }
  }
}
