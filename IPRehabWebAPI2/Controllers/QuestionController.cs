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
  [Produces("application/json")]
  [Route("api/[controller]")]
  [ApiController]
  public class QuestionController : ControllerBase
  {
    private readonly IQuestionRepository _questionRepository;
    private readonly IEpisodeOfCareRepository _episodeRepository;
    private readonly ICodeSetRepository _codeSetRepository;

    public QuestionController(IQuestionRepository questionRepository, IEpisodeOfCareRepository episodeRepository, ICodeSetRepository codeSetRepository)
    {
      _questionRepository = questionRepository;
      _episodeRepository = episodeRepository;
      _codeSetRepository = codeSetRepository;
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("GetAll")]
    [HttpGet()]
    public async Task<IActionResult> GetAll(bool includeAnswer, int episodeID)
    {
      int stageID = _codeSetRepository.FindByCondition(x => x.CodeValue == "All").FirstOrDefault().CodeSetID;
      var questions = _questionRepository.FindByCondition(x => x.Active.Value != false)
        .OrderBy(x => x.Order).ThenBy(x => x.QuestionKey).ThenBy(x => x.QuestionKey)
        .Select(q => HydrateDTO.HydrateQuestion(q, "All", stageID)
      ).ToList();
      if (includeAnswer)
      {
        //the answer keys are contained in the episode then use episodeID to find all the answers to each question
        foreach (var q in questions)
        {
          var thisEpisode = await _episodeRepository.FindByCondition(episode =>
            episode.EpisodeOfCareID == episodeID).FirstOrDefaultAsync();

          var thisQuestionAnswers = thisEpisode?.tblAnswer?.Where(a => a.QuestionIDFK == q.QuestionID)
            .Select(a => HydrateDTO.HydrateAnswer(a, thisEpisode)).ToList();

          if (thisQuestionAnswers == null)
          {
            //ToDo: create app.tblQuestionDependency
            //find the question dependencies
            //if determining question has enabling answers then set the q.Enabled = true, otherwise, false
            q.Enabled = false;
          }
          else
          {
            q.Enabled = true;
            if (thisQuestionAnswers.Any())
            {
              q.Answers = thisQuestionAnswers;
            }
          }
        }

      }
      return Ok(questions);
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("GetStageAsync/{stageName}")]
    [HttpGet()]
    public async Task<IActionResult> GetStageAsync(string stageName, bool includeAnswer, int episodeID)
    {
      int stageID = _codeSetRepository.FindByCondition(x => x.CodeValue == stageName).FirstOrDefault().CodeSetID;
      stageName = stageName.Trim().ToUpper();
      
      List<QuestionDTO> questions = null;

      questions = _questionRepository.FindByCondition(q =>
        q.Active.Value != false &&
        q.tblQuestionStage.Any(s =>
          s.StageFKNavigation.CodeValue.Trim().ToUpper() == stageName)
      ) //.OrderBy(x => x.FormFkNavigation.SortOrder).ThenBy(x => x.Order).ThenBy(x => x.QuestionKey)
      .Select(q => HydrateDTO.HydrateQuestion(q, stageName, stageID))
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
      else
      {
        if (includeAnswer)
        {
          //the answer keys are contained in the episode then use episodeID to find all the answers to each question
          foreach (var q in questions)
          {
            var thisEpisode = await _episodeRepository.FindByCondition(episode =>
              episode.EpisodeOfCareID == episodeID).FirstOrDefaultAsync();

            var thisQuestionAnswers = thisEpisode?.tblAnswer?.Where(a => a.QuestionIDFK == q.QuestionID && a.StageIDFKNavigation.CodeSetID == stageID)
              .Select(a => HydrateDTO.HydrateAnswer(a, thisEpisode)).ToList();

            if (thisQuestionAnswers == null)
            {
              //ToDo: create app.tblQuestionDependency
              //find the question dependencies
              //if determining question has enabling answers then set the q.Enabled = true, otherwise, false
              q.Enabled = false;
            }
            else
            {
              q.Enabled = true;
              if (thisQuestionAnswers.Any())
              {
                q.Answers = thisQuestionAnswers;
              }
            }
          }
        }
      }
      return Ok(questions);
    }

    // GET: api/<ValuesController>/ID
    [HttpGet("{ID}")]
    public IActionResult Get(int ID)
    {
      int stageIrrelevant = -1;
      var question = _questionRepository.FindByCondition(x => x.QuestionID == ID)
                        .Select(q => HydrateDTO.HydrateQuestion(q, "Specific Question", stageIrrelevant)).FirstOrDefault();
      return Ok(question);
    }

    // POST api/<ValuesController>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<tblQuestion>> Post([FromBody] tblQuestion question)
    {
      await _questionRepository.CreateAsync(question);

      return CreatedAtAction("GetAsync", new { question.QuestionID }, question);
    }

    // PUT api/<ValuesController>/5
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id}")]
    public async Task<ActionResult<tblQuestion>> Put(int id, [FromBody] tblQuestion question)
    {
      tblQuestion questionToUpdate = _questionRepository.FindByCondition(x => x.QuestionID == id).FirstOrDefault();
      questionToUpdate = question;
      await _questionRepository.UpdateAsync(questionToUpdate);
      return Ok(questionToUpdate);
    }

    // DELETE api/<ValuesController>/5
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<tblQuestion>> DeleteAsync(int id)
    {
      var question = _questionRepository.FindByCondition(x => x.QuestionID == id).FirstOrDefault();
      await _questionRepository.DeleteAsync(question);
      return Ok(question);
    }
  }
}
