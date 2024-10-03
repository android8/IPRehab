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
            if (!ModelState.IsValid)
                return BadRequest();

            PostbackResponseDTO deleteAnswersPostbackResponseDTO = null, updateAnswersPostbackResponseDTO = null, insertAnswersPostbackResponseDTO = null;

            #region old answers
            if (postbackModel.DeleteAnswers != null && postbackModel.DeleteAnswers.Any())
            {
                try
                {
                    //delete old answers first to prevent answer keys violation
                    await _answerHelper.TransactionalDeleteAsync(postbackModel.EpisodeID, postbackModel.DeleteAnswers);
                }
                catch (Exception ex)
                {
                    deleteAnswersPostbackResponseDTO = new PostbackResponseDTO
                    {
                        ExecptionMsg = "(DELETE) EXISTING Answers failed and rolled back the transaction.",
                        InnerExceptionMsg = ex.InnerException.Message,
                        OriginalAnswers = postbackModel.DeleteAnswers
                    };
                }
            }
            #endregion

            #region update answers
            if (postbackModel.UpdateAnswers != null && postbackModel.UpdateAnswers.Any())
            {
                try
                {
                    await _answerHelper.TransactionalUpdateAnswerAsync(postbackModel.EpisodeID, postbackModel.UpdateAnswers);
                }
                catch (Exception ex)
                {
                    updateAnswersPostbackResponseDTO = new PostbackResponseDTO
                    {
                        ExecptionMsg = "(UPDATE) EXISTING Answers failed and rolled back the transaction.",
                        InnerExceptionMsg = ex.InnerException.Message,
                        OriginalAnswers = postbackModel.UpdateAnswers
                    };
                }
            }
            #endregion

            #region new answers
            if (postbackModel.InsertAnswers != null && postbackModel.InsertAnswers.Any())
            {
                try
                {
                    await _answerHelper.TransactionalInsertNewAnswerOnlyAsync(postbackModel.EpisodeID, postbackModel.InsertAnswers);
                }
                catch (Exception ex)
                {
                    insertAnswersPostbackResponseDTO = new PostbackResponseDTO
                    {
                        ExecptionMsg = "(INSERT) NEW Answers failed and rolled back the transaction.",
                        InnerExceptionMsg = ex.InnerException.Message,
                        OriginalAnswers = postbackModel.InsertAnswers,
                    };
                }
            }
            #endregion

            if (deleteAnswersPostbackResponseDTO is null && updateAnswersPostbackResponseDTO is null && insertAnswersPostbackResponseDTO is null)
                return StatusCode(StatusCodes.Status200OK);
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new
                    {
                        DeleteException = $"{JsonSerializer.Serialize(deleteAnswersPostbackResponseDTO)}",
                        UpdateException = $"{JsonSerializer.Serialize(updateAnswersPostbackResponseDTO)}",
                        InsertException = $"{JsonSerializer.Serialize(insertAnswersPostbackResponseDTO)}"
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

            if (!ModelState.IsValid || postbackModel.InsertAnswers == null || !postbackModel.InsertAnswers.Any())
                return StatusCode(StatusCodes.Status400BadRequest);

            try
            {
                int newEpisodeID = await _answerHelper.TransactionalInsertNewEpisodeAsync(postbackModel.FacilityID, postbackModel.InsertAnswers);
                //return CreatedAtRoute("DefaultApi", new { id = newEpisodeID });
                return StatusCode(StatusCodes.Status201Created, newEpisodeID);
            }
            catch (Exception ex)
            {
                newAnswerPostBackResponse.ExecptionMsg = $"(INSERT) NEW EPISODE failed and rolled back the transaction.";
                newAnswerPostBackResponse.InnerExceptionMsg = ex.InnerException.Message;
                newAnswerPostBackResponse.OriginalAnswers = postbackModel.InsertAnswers;
                return StatusCode(StatusCodes.Status400BadRequest, $"{JsonSerializer.Serialize(newAnswerPostBackResponse)}");
            }
        }
    }
}
