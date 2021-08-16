using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Models
{
  public class QuestionInstructionDTO
  {
    public int InstructionId { get; set; }
    public int QuestionIdfk { get; set; }
    public string Instruction { get; set; }
    public string DisplayLocation { get; set; }
  }
}
