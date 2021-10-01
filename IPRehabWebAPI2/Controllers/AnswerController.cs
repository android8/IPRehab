using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IPRehabWebAPI2.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AnswerController : ControllerBase
  {
    // GET: api/<AnswerController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
      return new string[] { "value1", "value2" };
    }

    // GET api/<AnswerController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    // POST api/<AnswerController>
    [HttpPost]
    public IActionResult Post([FromBody] PostbackModel postbackModel)
    {
      if(!ModelState.IsValid)
      {
        return BadRequest();
      }

      var newAnswers = postbackModel.NewAnswers;
      var oldAnswers = postbackModel.OldAnswers;
      var updatedAnswers = postbackModel.UpdatedAnswers;

      //Transaction.Current.TransactionCompleted();

      //begin transaction
      //  
      //end transaction
      return Ok();
    }

    // PUT api/<AnswerController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<AnswerController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
