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
      PostbackResponseDTO newAnswerpostBackResponse = new();
      PostbackResponseDTO oldAnswerpostBackResponse = new();
      PostbackResponseDTO updatedAnswerpostBackResponse = new();

      if (!ModelState.IsValid)
      {
        return BadRequest();
      }
      
      string exceptionMsg = string.Empty;

      if (postbackModel.NewAnswers.Any())
      {
        try
        {
          if (postbackModel.EpisodeID <= 0)
          {
            //both episode and answers are new
            int newEpisodeID = await _answerHelper.TransactionalInsertNewEpisodeAsync(postbackModel.NewAnswers);
          }
          else
          {
            //existing episode but new answers
            await _answerHelper.TransactionalInsertNewAnswerOnlyAsync(postbackModel.EpisodeID, postbackModel.NewAnswers);
          }
        }
        catch (Exception ex)
        {
          newAnswerpostBackResponse.ExecptionMsg = $"insert transaction failed and rolled back. {ex.Message}";
          newAnswerpostBackResponse.InnerExceptionMsg = ex.InnerException.Message;
          newAnswerpostBackResponse.OriginalAnswers = postbackModel.NewAnswers;
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
          oldAnswerpostBackResponse.ExecptionMsg = $"delete transaction failed and rolled back. {ex.Message}";
          oldAnswerpostBackResponse.InnerExceptionMsg = ex.InnerException.Message;
          oldAnswerpostBackResponse.OriginalAnswers = postbackModel.OldAnswers;
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

          updatedAnswerpostBackResponse.ExecptionMsg = $"update transaction failed and rolled back. {ex.Message}";
          updatedAnswerpostBackResponse.InnerExceptionMsg = ex.InnerException.Message;
          updatedAnswerpostBackResponse.OriginalAnswers = postbackModel.UpdatedAnswers;
        }
      }
      
      if(string.IsNullOrEmpty(exceptionMsg))
      {
        return StatusCode(StatusCodes.Status200OK);
      }
      else
      {
        return StatusCode(StatusCodes.Status400BadRequest, 
          $"{JsonSerializer.Serialize(newAnswerpostBackResponse)} {Environment.NewLine} " +
          $"{JsonSerializer.Serialize(oldAnswerpostBackResponse)} {Environment.NewLine} " +
          $"{JsonSerializer.Serialize(updatedAnswerpostBackResponse)}");
      }
    }
  }
}
