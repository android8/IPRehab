using System;
using System.ComponentModel.DataAnnotations;

namespace IPRehabWebAPI2.Models
{
    public class TestColor
    {
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        public string Name { get; set; }
        public int RBG { get; set; }
    }
}
