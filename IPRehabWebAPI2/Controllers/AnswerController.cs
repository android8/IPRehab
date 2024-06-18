using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Helpers;
using IPRehabWebAPI2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        /// the old and new may have the same id, so 
        /// delete old answers first, then edit updated answers, then insert new answers
        /// </summary>
        /// <param name="postbackModel"></param>
        /// <returns></returns>
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

            #region old answers
            //delete old answers to avoid new answers with the same question id are deleted
            if (postbackModel.OldAnswers != null && postbackModel.OldAnswers.Any())
            {
                try
                {
                    await _answerHelper.TransactionalDeleteAsync(postbackModel.EpisodeID, postbackModel.OldAnswers);
                }
                catch (Exception ex)
                {
                    oldAnswerpostBackResponse.ExecptionMsg = $"delete transaction failed and rolled back. {ex.Message}";
                    oldAnswerpostBackResponse.InnerExceptionMsg = ex.InnerException.Message;
                    oldAnswerpostBackResponse.OriginalAnswers = postbackModel.OldAnswers;
                }
            }
            #endregion

            #region updated answers
            if (postbackModel.UpdatedAnswers != null && postbackModel.UpdatedAnswers.Any())
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
            #endregion

            #region new answers
            if (postbackModel.NewAnswers != null && postbackModel.NewAnswers.Any())
            {
                try
                {
                    //add new answers to the existing episode
                    await _answerHelper.TransactionalInsertNewAnswerOnlyAsync(postbackModel.EpisodeID, postbackModel.NewAnswers);

                }
                catch (Exception ex)
                {
                    newAnswerpostBackResponse.ExecptionMsg = $"insert transaction failed and rolled back. {ex.Message}";
                    newAnswerpostBackResponse.InnerExceptionMsg = ex.InnerException.Message;
                    newAnswerpostBackResponse.OriginalAnswers = postbackModel.NewAnswers;
                }
            }
            #endregion

            #region update memory cache
            var listOfRptRehabDetails = await _treatingSpecialtyPatientRepository.FindAll().ToListAsync();
            //update cache for 24 hours
            _memoryCache.Remove(CacheKeys.CacheKeyAllPatients_TreatingSpeciality);
            _memoryCache.Set(CacheKeys.CacheKeyAllPatients_TreatingSpeciality, listOfRptRehabDetails, TimeSpan.FromDays(1));
            #endregion

            if (string.IsNullOrEmpty(exceptionMsg))
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
            if (postbackModel.NewAnswers == null || !postbackModel.NewAnswers.Any())
            {
                return BadRequest();
            }

            try
            {
                //both episode and answers are new
                int newEpisodeID = await _answerHelper.TransactionalInsertNewEpisodeAsync(postbackModel.FacilityID, postbackModel.NewAnswers);
                //return CreatedAtRoute("DefaultApi", new { id = newEpisodeID });
                return StatusCode(StatusCodes.Status201Created, newEpisodeID);
            }
            catch (Exception ex)
            {
                newAnswerPostBackResponse.ExecptionMsg = $"insert transaction failed and rolled back. {ex.Message}";
                newAnswerPostBackResponse.InnerExceptionMsg = ex.InnerException.Message;
                newAnswerPostBackResponse.OriginalAnswers = postbackModel.NewAnswers;
                return StatusCode(StatusCodes.Status500InternalServerError, newAnswerPostBackResponse);
            }
        }
    }
}
