using IPRehabModel;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IPRehabWebAPI2.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class QuestionController : ControllerBase
  {
    private readonly IQuestionRepository _questionRepository;
    private readonly IEpisodeOfCareRepository _episodeRepository;
    private readonly IAnswerRepository _answerRepository;
    private readonly IQuestionStageRepository _questionStageRepository;

    public QuestionController(IQuestionRepository questionRepository, IEpisodeOfCareRepository episodeRepository, IAnswerRepository answerRepository,
      IQuestionStageRepository questionStageRepository)
    {
      _questionRepository = questionRepository;
      _episodeRepository = episodeRepository;
      _answerRepository = answerRepository;
      _questionStageRepository = questionStageRepository;
    }

    [Route("GetAll")]
    [HttpGet()]
    public ActionResult GetAll(bool includeAnswer)
    {
      var questions = _questionRepository.FindByCondition(x => x.Active.Value != false ||
        x.TblQuestionStage.Any(
          s => s.StageFkNavigation.CodeValue == "All" && s.QuestionIdFkNavigation.FormFkNavigation.CodeValue == "Form-Discharge"
        ))
        .OrderBy(x => x.FormFkNavigation.SortOrder).ThenBy(x => x.Order).ThenBy(x => x.QuestionKey)
        .Select(q => HydrateDTO.HydrateQuestion(q, string.Empty)
      ).ToList();
      if (includeAnswer)
      {
        //populate answers
      }
      return Ok(questions);
    }

    [Route("GetBaseForm")]
    [HttpGet()]
    public ActionResult GetBaseForm()
    {
      var questions = _questionRepository.FindByCondition(x => x.FormFkNavigation.CodeValue == "Form-BasicInfo")
         .Select(q => HydrateDTO.HydrateQuestion(q, string.Empty))
                           .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey); ;

      ApiResponseModel model = new ApiResponseModel()
      {
        Result = questions,
        RecordCount = questions.Count()
      };
      return Ok(model);
    }

    [Route("GetStageAsync/{stageName}")]
    [HttpGet()]
    public async Task<IActionResult> GetStageAsync(string stageName, bool includeAnswer, int episodeID)
    {
      stageName = stageName.Trim().ToUpper();
      List<QuestionDTO> questions = null;
      try
      {
        questions = _questionRepository.FindByCondition(q =>
          q.Active.Value != false &&
          q.TblQuestionStage.Any(s =>
            s.StageFkNavigation.CodeValue.Trim().ToUpper() == stageName)
        ) //.OrderBy(x => x.FormFkNavigation.SortOrder).ThenBy(x => x.Order).ThenBy(x => x.QuestionKey)
        .Select(q => HydrateDTO.HydrateQuestion(q, stageName))
        .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey)
        .ToList();

        if (episodeID == -1 /* new episode hoist Q12, Q23 to top of the list*/)
        {
          var episodeKeyQuestions = questions.Where(x => x.QuestionKey == "Q12" || x.QuestionKey == "Q23");
          if (episodeKeyQuestions != null)
          {
            var hoisted = questions;
            episodeKeyQuestions = episodeKeyQuestions.ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);
            hoisted = questions.Except(episodeKeyQuestions).ToList();
             /* hoist OnsetDate question to the top of the list */
            hoisted.InsertRange(0, episodeKeyQuestions);
            questions = hoisted;            
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Unable to get questions in the specified stage. {ex.Message}");
        throw;
      }
      if (includeAnswer)
      {
        try
        {
          //populate answers
          foreach (var q in questions)
          {
            var thisEpisode = await _episodeRepository.FindByCondition(episode =>
              episode.EpisodeOfCareId == episodeID).FirstOrDefaultAsync();

            var thisQuestionAnswers = thisEpisode.TblAnswer.Where(a => a.QuestionIdfk == q.QuestionID)
              .Select(a => HydrateDTO.HydrateAnswer(a)).ToList();

            if (thisQuestionAnswers.Any())
            {
              q.Answers = thisQuestionAnswers;
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Unable to find answers or episode. {ex.Message}");
          throw;
        }
      }
      return Ok(questions);
    }

    /// <summary>
    /// get initial intake questions (required or not)
    /// </summary>
    /// <returns></returns>
    [Route("GetInitialStage")]
    [HttpGet()]
    public IActionResult GetInitStage()
    {
      var questions = _questionRepository.FindByCondition(x => x.Active.Value != false &&
                       x.TblQuestionStage.Any(
                         s => s.QuestionIdFk == x.QuestionId &&
                             s.StageFkNavigation.CodeValue == "Initial"
                       )
                     )
                     .Select(
                        q => HydrateDTO.HydrateQuestion(q, "Initial")
                     )
                     .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);

      //var generatedSql = ((System.Data.Entity.Core.Objects.ObjectQuery)questions).ToTraceString();

      //ApiResponseModel model = new ApiResponseModel()
      //{
      //   Result = questions,
      //   RecordCount = questions.Count()
      //};
      return Ok(questions);
    }

    /// <summary>
    /// get interim stage questions (both required and not)
    /// </summary>
    /// <returns></returns>
    [Route("GetInterimStage")]
    [HttpGet()]
    public IActionResult GetInterimStage()
    {
      var questions = _questionRepository.FindByCondition(
                        x => x.Active.Value != false &&
                        (!"2. Discharge Goal".Contains(x.GroupTitle) || string.IsNullOrEmpty(x.GroupTitle)) &&
                        x.TblQuestionStage.Any(
                           s => s.QuestionIdFk == x.QuestionId &&
                           s.StageFkNavigation.CodeValue == "Interim"
                        ))
                        .Select(q => HydrateDTO.HydrateQuestion(q, "Interim"))
                        .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);

      return Ok(questions);
    }

    /// <summary>
    /// get discharge stage questions (required must be answer before discharge)
    /// </summary>
    /// <returns></returns>
    [Route("GetDischargeStage")]
    [HttpGet()]
    public IActionResult GetDischargeStage()
    {
      var questions = _questionRepository.FindByCondition(x =>
                        x.Active.Value != false &&
                        (!"1. Admission Performance".Contains(x.GroupTitle) || string.IsNullOrEmpty(x.GroupTitle)) &&
                        x.TblQuestionStage.Any(
                       s => s.QuestionIdFk == x.QuestionId &&
                           s.StageFkNavigation.CodeValue == "Discharge"))
                           .Select(q => HydrateDTO.HydrateQuestion(q, "Discharge"))
                           .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);

      return Ok(questions);
    }

    /// <summary>
    /// get follow up stage questions
    /// </summary>
    /// <returns></returns>
    [Route("GetFollowupStage")]
    [HttpGet()]
    public IActionResult GetFollowupStage()
    {
      var questions = _questionRepository.FindByCondition(x => x.Active.Value != false &&
                     x.TblQuestionStage.Any(
                       s => s.QuestionIdFk == x.QuestionId &&
                           s.StageFkNavigation.CodeValue == "Followup"))
                           .Select(q => HydrateDTO.HydrateQuestion(q, "Followup"))
                           .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);

      return Ok(questions);
    }

    /// <summary>
    /// get question in the specified {form} paramter
    /// </summary>
    /// <returns></returns>
    // GET: api/<QuestionController>/GetAdmissionForm     [Route("api/[controller]/GetAdmissionForm")]
    [HttpGet()]
    public IActionResult GetAdmissionForm()
    {
      var questions = _questionRepository.FindByCondition(x => x.FormFkNavigation.CodeValue == "Form-Admission")
                        .Select(q => HydrateDTO.HydrateQuestion(q, string.Empty))
                        .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);

      return Ok(questions);
    }

    [Route("GetDischargeForm")]
    [HttpGet()]
    public IActionResult GetDischargeForm()
    {
      var questions = _questionRepository.FindByCondition(x => x.FormFkNavigation.CodeValue == "Form-Discharge")
                        .Select(q => HydrateDTO.HydrateQuestion(q, string.Empty))
                        .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);

      return Ok(questions);
    }

    // GET: api/<ValuesController>/ID
    [HttpGet("{ID}")]
    public IActionResult Get(int ID)
    {
      var question = _questionRepository.FindByCondition(x => x.QuestionId == ID)
                        .Select(q => HydrateDTO.HydrateQuestion(q, string.Empty)).FirstOrDefault()
      ;
      return Ok(question);
    }

    // POST api/<ValuesController>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<TblQuestion>> Post([FromBody] TblQuestion question)
    {
      await _questionRepository.CreateAsync(question);

      return CreatedAtAction("GetAsync", new { question.QuestionId }, question);
    }

    // PUT api/<ValuesController>/5
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id}")]
    public async Task<ActionResult<TblQuestion>> Put(int id, [FromBody] TblQuestion question)
    {
      TblQuestion questionToUpdate = _questionRepository.FindByCondition(x => x.QuestionId == id).FirstOrDefault();
      questionToUpdate = question;
      await _questionRepository.UpdateAsync(questionToUpdate);
      return Ok(questionToUpdate);
    }

    // DELETE api/<ValuesController>/5
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<TblQuestion>> DeleteAsync(int id)
    {
      var question = _questionRepository.FindByCondition(x => x.QuestionId == id).FirstOrDefault();
      await _questionRepository.DeleteAsync(question);
      return Ok(question);
    }
  }
}
