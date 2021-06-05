using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class TestColorController : ControllerBase
  {
    private static readonly string[] Colors = new[]
    {
            "Red", "Orange", "Blue", "Green", "Purple", "Indigo", "Black", "White"
        };

    private readonly ILogger<TestColorController> _logger;

    public TestColorController(ILogger<TestColorController> logger)
    {
      _logger = logger;
    }

    [HttpGet]
    public IEnumerable<TestColor> Get()
    {
      var rng = new Random();
      return Enumerable.Range(1, 5).Select(index => new TestColor
      {
        Date = DateTime.Now.AddDays(index),
        Name = Colors[index],
        RBG = Color.FromName(Colors[index].ToString()).ToArgb()
      })
      .ToArray();
    }
  }
}
