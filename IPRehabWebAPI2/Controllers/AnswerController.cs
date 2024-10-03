using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IPRehabWebAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly AnswerHelper _answerHelper;
        private readonly IMemoryCache _memoryCache;
        private readonly ITreatingSpecialtyDirectPatientRepository _treatingSpecialtyPatientRepository;

        /// <summary>
        /// inject AnswerHelper
        /// </summary>
        /// <param name="answerHelper"></param>
        /// <param name="memoryCache"></param>
        /// <param name="treatingSpecialtyPatientRepository"></param>
        public AnswerController(AnswerHelper answerHelper, IMemoryCache memoryCache, ITreatingSpecialtyDirectPatientRepository treatingSpecialtyPatientRepository)
        {
            _answerHelper = answerHelper;
            _treatingSpecialtyPatientRepository = treatingSpecialtyPatientRepository;
            _memoryCache = memoryCache;
        }
        /// <summary>
        /// the updated answer may have the same id, so 
        /// delete old answers first, then edit updated answers, then insert new answers
        /// </summary>
        /// <param name="postbackModel"></param>
        /// <returns></returns>
        // POST api/<AnswerController>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] PostbackModel postbackModel)
        {
            PostbackResponseDTO insertAnswersPostback = new();
            PostbackResponseDTO deleteAnswersPostback = new();
            PostbackResponseDTO updateAnswersPostback = new();

            if (!ModelState.IsValid)
                return BadRequest();

            #region old answers
            //delete old answers to avoid new answers with the same question id are deleted
            if (postbackModel.DeleteAnswers != null && postbackModel.DeleteAnswers.Any())
            {
                try
                {
                    await _answerHelper.TransactionalDeleteAsync(postbackModel.EpisodeID, postbackModel.DeleteAnswers);
                }
                catch (Exception ex)
                {
                    deleteAnswersPostback.ExecptionMsg = $"delete transaction failed and rolled back. {ex.Message}";
                    deleteAnswersPostback.InnerExceptionMsg = ex.InnerException.Message;
                    deleteAnswersPostback.OriginalAnswers = postbackModel.DeleteAnswers;
                }
            }
            #endregion

            #region update answers
            if (postbackModel.UpdateAnswers != null && postbackModel.UpdateAnswers.Any())
            {
                try
                {
                    //update existing episode and existing answers
                    await _answerHelper.TransactionalUpdateAnswerAsync(postbackModel.EpisodeID, postbackModel.UpdateAnswers);
                }
                catch (Exception ex)
                {
                    updateAnswersPostback.ExecptionMsg = $"update transaction failed and rolled back. {ex.Message}";
                    updateAnswersPostback.InnerExceptionMsg = ex.InnerException.Message;
                    updateAnswersPostback.OriginalAnswers = postbackModel.UpdateAnswers;
                }
            }
            #endregion

            #region new answers
            if (postbackModel.InsertAnswers != null && postbackModel.InsertAnswers.Any())
            {
                try
                {
                    //add new answers to the existing episode
                    await _answerHelper.TransactionalInsertNewAnswerOnlyAsync(postbackModel.EpisodeID, postbackModel.InsertAnswers);

                }
                catch (Exception ex)
                {
                    insertAnswersPostback.ExecptionMsg = $"insert transaction failed and rolled back. {ex.Message}";
                    insertAnswersPostback.InnerExceptionMsg = ex.InnerException.Message;
                    insertAnswersPostback.OriginalAnswers = postbackModel.InsertAnswers;
                }
            }
            #endregion

            if (string.IsNullOrEmpty(deleteAnswersPostback.ExecptionMsg) && string.IsNullOrEmpty(insertAnswersPostback.ExecptionMsg) && string.IsNullOrEmpty(updateAnswersPostback.ExecptionMsg))
            {
                return StatusCode(StatusCodes.Status200OK);
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new
                    {
                        insertException = $"{JsonSerializer.Serialize(insertAnswersPostback.ExecptionMsg)}",
                        deleteException = $"{JsonSerializer.Serialize(deleteAnswersPostback.ExecptionMsg)}",
                        updateException = $"{JsonSerializer.Serialize(updateAnswersPostback.ExecptionMsg)}"
                    }
                );
            }
        }

        /// <summary>
        /// Create new episode when episode id is -1.  Only look into PostbackModel.NewAnswers collection.
        /// </summary>
        /// <param name="postbackModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PostNewEpisodeAsync([FromBody] PostbackModel postbackModel)
        {
            PostbackResponseDTO newAnswerPostBackResponse = new();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (postbackModel.InsertAnswers == null || !postbackModel.InsertAnswers.Any())
            {
                return BadRequest();
            }

            try
            {
                //both episode and answers are new
                int newEpisodeID = await _answerHelper.TransactionalInsertNewEpisodeAsync(postbackModel.FacilityID, postbackModel.InsertAnswers);
                //return CreatedAtRoute("DefaultApi", new { id = newEpisodeID });
                return StatusCode(StatusCodes.Status201Created, newEpisodeID);
            }
            catch (Exception ex)
            {
                newAnswerPostBackResponse.ExecptionMsg = $"insert transaction failed and rolled back. {ex.Message}";
                newAnswerPostBackResponse.InnerExceptionMsg = ex.InnerException.Message;
                newAnswerPostBackResponse.OriginalAnswers = postbackModel.InsertAnswers;
                return StatusCode(StatusCodes.Status500InternalServerError, newAnswerPostBackResponse);
            }
        }
    }
}
