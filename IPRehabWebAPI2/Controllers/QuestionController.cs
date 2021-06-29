using IPRehabModel;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IPRehabWebAPI2.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class QuestionController : ControllerBase
  {
    private readonly IQuestionRepository _questionRepository;

    public QuestionController(IQuestionRepository questionRepository)
    {
      _questionRepository = questionRepository;
    }

    [Route("GetAll")]
    [HttpGet()]
    public ActionResult GetAll(bool includeAnswer)
    {
      var questions = _questionRepository.FindByCondition(x => x.Active.Value != false)
                        .OrderBy(x => x.Order).ThenBy(x => x.QuestionKey)
                        .Select(q => HydrateDTO.HydrateQuestion(q)
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
         .Select(q => HydrateDTO.HydrateQuestion(q))
                           .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey); ;

      ApiResponseModel model = new ApiResponseModel()
      {
        Result = questions,
        RecordCount = questions.Count()
      };
      return Ok(model);
    }

    [Route("GetStage")]
    [HttpGet()]
    public IActionResult GetStage(string stageName, bool includeAnswer)
    {
      switch (stageName)
      {
        case "Interim":
          {
            var questions = _questionRepository.FindByCondition(
              x => x.Active.Value != false && !"2. Discharge Goal".Contains(x.GroupTitle) &&
              x.TblQuestionStage.Where(
                 s => s.QuestionIdFk == x.QuestionId &&
                 s.StageFkNavigation.CodeValue == stageName
              ).Any())
              .Select(q => HydrateDTO.HydrateQuestion(q))
              .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);
            if (includeAnswer)
            {
              //populate answers
            }
            return Ok(questions);
          }
        default:
          {
            var questions = _questionRepository.FindByCondition(x => x.Active.Value != false &&
              x.TblQuestionStage.Where(
                s => s.QuestionIdFk == x.QuestionId &&
                    s.StageFkNavigation.CodeValue == stageName
              ).Any()
            )
            .Select(
               q => HydrateDTO.HydrateQuestion(q)
            )
            .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);
            if (includeAnswer)
            {
              //populate answers
            }
            return Ok(questions);
          }
      }
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
                       x.TblQuestionStage.Where(
                         s => s.QuestionIdFk == x.QuestionId &&
                             s.StageFkNavigation.CodeValue == "Initial"
                       ).Any()
                     )
                     .Select(
                        q => HydrateDTO.HydrateQuestion(q)
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
                        x.TblQuestionStage.Where(
                           s => s.QuestionIdFk == x.QuestionId &&
                           s.StageFkNavigation.CodeValue == "Interim"
                        ).Any())
                        .Select(q => HydrateDTO.HydrateQuestion(q))
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
                        x.TblQuestionStage.Where(
                       s => s.QuestionIdFk == x.QuestionId &&
                           s.StageFkNavigation.CodeValue == "Discharge").Any())
                           .Select(q => HydrateDTO.HydrateQuestion(q))
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
                     x.TblQuestionStage.Where(
                       s => s.QuestionIdFk == x.QuestionId &&
                           s.StageFkNavigation.CodeValue == "Followup").Any())
                           .Select(q => HydrateDTO.HydrateQuestion(q))
                           .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);

      return Ok(questions);
    }
    /// <summary>
    /// get question in the specified {form} paramter
    /// </summary>
    /// <param name="form">Form-BasicInfo, Form-Admission, Form-Discharge</param>
    /// <returns></returns>
    // GET: api/<QuestionController>/GetBaseFormAsync     [Route("api/[controller]/GetAdmissionForm")]
    [HttpGet()]
    public IActionResult GetAdmissionForm()
    {
      var questions = _questionRepository.FindByCondition(x => x.FormFkNavigation.CodeValue == "Form-Admission")
                        .Select(q => HydrateDTO.HydrateQuestion(q))
                        .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);

      return Ok(questions);
    }

    [Route("GetDischargeForm")]
    [HttpGet()]
    public IActionResult GetDischargeForm()
    {
      var questions = _questionRepository.FindByCondition(x => x.FormFkNavigation.CodeValue == "Form-Discharge")
                        .Select(q => HydrateDTO.HydrateQuestion(q))
                        .ToList().OrderBy(o => o.DisplayOrder).ThenBy(o => o.QuestionKey);

      return Ok(questions);
    }

    // GET: api/<ValuesController>/ID
    [HttpGet("{ID}")]
    public IActionResult Get(int ID)
    {
      var question = _questionRepository.FindByCondition(x => x.QuestionId == ID)
                        .Select(q => HydrateDTO.HydrateQuestion(q)).FirstOrDefault()
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
