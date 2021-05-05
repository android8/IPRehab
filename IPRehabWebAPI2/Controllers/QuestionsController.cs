using IPRehabModel;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IPRehabWebAPI2.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class QuestionsController : ControllerBase
   {
      private readonly IQuestionRepository _questionRepository;

      public QuestionsController(IQuestionRepository questionRepository)
      {
         _questionRepository = questionRepository;
      }

      [Route("GetBaseFormAsync")]
      [HttpGet()]
      public ActionResult GetBaseForm()
      {
         var questions = _questionRepository.FindByCondition(x => x.FormFkNavigation.CodeValue == "Form-BasicInfo");
         
         ApiResponseModel model = new ApiResponseModel()
         {
            Result = questions,
            RecordCount = questions.Count()
         };
         return Ok(model);
      }

      /// <summary>
      /// get initial intake questions (required or not)
      /// </summary>
      /// <returns></returns>
      [Route("GetInitialStage")]
      [HttpGet()]
      public ActionResult GetInitStage()
      {
         var questions = _questionRepository.FindByCondition(x => x.TblQuestionStage
                        .Where(s => s.QuestionIdFk == x.QuestionId &&
                              s.StageFkNavigation.CodeValue == "Initial").Any());

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
      public ActionResult GetInterimStage()
      {
         var questions = _questionRepository.FindByCondition(x => x.TblQuestionStage
                        .Where(s => s.QuestionIdFk == x.QuestionId &&
                              s.StageFkNavigation.CodeValue == "Interim").Any());

         ApiResponseModel model = new ApiResponseModel()
         {
            Result = questions,
            RecordCount = questions.Count()
         };
         return Ok(model);
      }

      /// <summary>
      /// get discharge stage questions (required must be answer before discharge)
      /// </summary>
      /// <returns></returns>
      [Route("GetDischargeStage")]
      [HttpGet()]
      public ActionResult GetDischargeStage()
      {
         var questions = _questionRepository.FindByCondition(x => x.TblQuestionStage
                        .Where(s => s.QuestionIdFk == x.QuestionId &&
                              s.StageFkNavigation.CodeValue == "Discharge").Any());

         ApiResponseModel model = new ApiResponseModel()
         {
            Result = questions,
            RecordCount = questions.Count()
         };
         return Ok(model);
      }

      /// <summary>
      /// get follow up stage questions
      /// </summary>
      /// <returns></returns>
      [Route("GetFollowupStage")]
      [HttpGet()]
      public ActionResult GetFollowupStage()
      {
         var questions = _questionRepository.FindByCondition(x => x.TblQuestionStage
                        .Where(s => s.QuestionIdFk == x.QuestionId &&
                              s.StageFkNavigation.CodeValue == "Followup").Any());

         ApiResponseModel model = new ApiResponseModel()
         {
            Result = questions,
            RecordCount = questions.Count()
         };
         return Ok(model);
      }
      /// <summary>
      /// get question in the specified {form} paramter
      /// </summary>
      /// <param name="form">Form-BasicInfo, Form-Admission, Form-Discharge</param>
      /// <returns></returns>
      // GET: api/<QuestionController>/GetBaseFormAsync     [Route("api/[controller]/GetAdmissionForm")]
      [HttpGet()]
      public ActionResult GetAdmissionForm()
      {
         var questions = _questionRepository.FindByCondition(x => x.FormFkNavigation.CodeValue == "Form-Admission");

         ApiResponseModel model = new ApiResponseModel()
         {
            Result = questions,
            RecordCount = questions.Count()
         };
         return Ok(model);
      }

      [Route("GetDischargeForm")]
      [HttpGet()]
      public ActionResult GetDischargeForm()
      {
         var questions = _questionRepository.FindByCondition(x => x.FormFkNavigation.CodeValue == "Form-Discharge");

         ApiResponseModel model = new ApiResponseModel()
         {
            Result = questions,
            RecordCount = questions.Count()
         };
         return Ok(model);
      }

      // GET: api/<ValuesController>/ID
      [HttpGet("{ID}")]
      public ActionResult Get(int ID)
      {
         var question = _questionRepository.FindByCondition(x => x.QuestionId == ID).FirstOrDefault();
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
