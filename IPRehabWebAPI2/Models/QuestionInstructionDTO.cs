﻿namespace IPRehabWebAPI2.Models
{
    public class QuestionInstructionDTO
    {
        public int InstructionId { get; set; }
        public int QuestionIDFK { get; set; }
        public string Instruction { get; set; }
        public string DisplayLocation { get; set; }
    }
}