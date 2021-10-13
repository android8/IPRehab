using IPRehabModel;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IPRehabWebAPI2.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AnswerController : ControllerBase
  {
    private readonly AnswerHelper _answerHelper;

    /// <summary>
    /// inject AnserHelper
    /// </summary>
    /// <param name="answerHelper"></param>
    public AnswerController(AnswerHelper answerHelper)
    {
      _answerHelper = answerHelper;
    }

    // POST api/<AnswerController>
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] PostbackModel postbackModel)
    {
      if(!ModelState.IsValid)
      {
        return BadRequest();
      }
      
      string exceptionMsg = string.Empty;

      if (postbackModel.NewAnswers.Any())
      {
        try
        {
          if (postbackModel.EpisodeID == -1)
          {
            //both episode and answers are new
            await _answerHelper.TransactionalInsertAsync(postbackModel.NewAnswers);
          }
          else
          {
            //existing episode but new answers
            await _answerHelper.TransactionalInsertAnswerOnlyAsync(postbackModel.EpisodeID, postbackModel.NewAnswers);
          }
        }
        catch (Exception ex)
        {
          exceptionMsg += $"insert transaction failed and rolled back. {Environment.NewLine} {ex.Message} {Environment.NewLine}{Environment.NewLine} Inner Exception: {ex.InnerException.Message} {Environment.NewLine}{Environment.NewLine} Failed Insert Model: {JsonSerializer.Serialize(postbackModel.NewAnswers)}";
        }
      }

      if (postbackModel.OldAnswers.Any())
      {
        try
        {
          //delete old answers and keep existing episode
          await _answerHelper.TransactionalDeleteAsync(postbackModel.EpisodeID, postbackModel.OldAnswers);
        }
        catch (Exception ex)
        {
          exceptionMsg += $"delete transaction failed and rolled back.  {Environment.NewLine} {ex.Message} {Environment.NewLine}{Environment.NewLine} Inner Exception: {ex.InnerException.Message} {Environment.NewLine}{Environment.NewLine} Failed Delete Model: {JsonSerializer.Serialize(postbackModel.OldAnswers)}";
        }
      }

      if (postbackModel.UpdatedAnswers.Any())
      {
        try
        {
          //update existing episode and existing answers
          await _answerHelper.TransactionalUpdateAnswerAsync(postbackModel.EpisodeID, postbackModel.UpdatedAnswers);
        }
        catch (Exception ex)
        {
          exceptionMsg += $"update transaction failed and rolled back. {Environment.NewLine} {ex.Message} {Environment.NewLine}{Environment.NewLine} Inner Exception: {ex.InnerException.Message} {Environment.NewLine}{Environment.NewLine} Failed Update Model: {JsonSerializer.Serialize(postbackModel.UpdatedAnswers)}";
        }
      }
      
      if(string.IsNullOrEmpty(exceptionMsg))
      {
        //return BadRequest();
        return StatusCode(StatusCodes.Status200OK);
      }
      else
      {
        return StatusCode(StatusCodes.Status400BadRequest, exceptionMsg);
      }
    }
  }
}
