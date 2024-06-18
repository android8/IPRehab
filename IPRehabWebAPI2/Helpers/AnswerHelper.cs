using AutoMapper;
using IPRehabModel;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPRehabWebAPI2.Helpers
{
    public class AnswerHelper
    {
        private readonly IPRehabContext _ipRehabContext;
        private readonly IAnswerRepository _answerRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// constructor injection of IPRehabContext in order to use _ipRehabContext transaction
        /// </summary>
        /// <param name="iPRehabContext"></param>
        /// <param name="answerRepository"></param>
        /// <param name="mapper"></param>
        public AnswerHelper(IPRehabContext iPRehabContext, IAnswerRepository answerRepository, IMapper mapper)
        {
            _ipRehabContext = iPRehabContext;
            _answerRepository = answerRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// add new episode and new answers
        /// </summary>
        /// <param name="FacilityID"></param>
        /// <param name="newAnswers"></param>
        /// <returns></returns>
        public async Task<int> TransactionalInsertNewEpisodeAsync(string FacilityID, List<UserAnswer> newAnswers)
        {
            using var transaction = _ipRehabContext.Database.BeginTransaction();
            try
            {
                #region insert the first answer to tblEpisodeOfCare to get new episode ID for all new answer of this episode
                var firstNewAnswer = newAnswers.First();
                tblEpisodeOfCare thisEpisode = new()
                {
                    OnsetDate = firstNewAnswer.OnsetDate,
                    AdmissionDate = firstNewAnswer.AdmissionDate,
                    PatientICNFK = firstNewAnswer.PatientID,
                    FacilityID6 = FacilityID,
                    LastUpdate = firstNewAnswer.LastUpdate
                };

                _ipRehabContext.tblEpisodeOfCare.Add(thisEpisode);
                await _ipRehabContext.SaveChangesAsync(); //must SaveChangesAsync() to get new ID
                #endregion

                #region insert tblAnswer each new answer with the new inserted episode ID
                foreach (UserAnswer ans in newAnswers)
                {
                    //tblAnswer thisAnswer = new()
                    //{
                    //    EpsideOfCareIDFK = thisEpisode.EpisodeOfCareID, //new Episode ID
                    //    QuestionIDFK = ans.QuestionID,
                    //    MeasureIDFK = ans.MeasureID,
                    //    AnswerCodeSetFK = ans.AnswerCodeSetID,
                    //    AnswerSequenceNumber = ans.AnswerSequenceNumber,
                    //    Description = ans.Description,
                    //    AnswerByUserID = ans.AnswerByUserID,
                    //    LastUpdate = ans.LastUpdate
                    //};
                    //_ipRehabContext.tblAnswer.Add(thisAnswer);

                    ans.EpisodeID = thisEpisode.EpisodeOfCareID;
                }
                _ipRehabContext.tblAnswer.AddRange(_mapper.Map<IEnumerable<tblAnswer>>(newAnswers));

                await _ipRehabContext.SaveChangesAsync();
                #endregion

                transaction.Commit();

                return thisEpisode.EpisodeOfCareID;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// add only new answers to existing episode
        /// </summary>
        /// <param name="thisEpisodeID"></param>
        /// <param name="newAnswers"></param>
        /// <returns></returns>
        public async Task TransactionalInsertNewAnswerOnlyAsync(int thisEpisodeID, List<UserAnswer> newAnswers)
        {
            using var transaction = _ipRehabContext.Database.BeginTransaction();
            try
            {
                #region insert tblAnswers with existing episode ID
                _ipRehabContext.tblAnswer.AddRange(_mapper.Map<IEnumerable<tblAnswer>>(newAnswers));

                //foreach (UserAnswer newAans in newAnswers)
                //{
                //    tblAnswer thisAnswer = new()
                //    {
                //        EpsideOfCareIDFK = thisEpisodeID, //existing Episode ID
                //        QuestionIDFK = newAans.QuestionID,
                //        MeasureIDFK = newAans.MeasureID,
                //        AnswerCodeSetFK = newAans.AnswerCodeSetID,
                //        AnswerSequenceNumber = newAans.AnswerSequenceNumber,
                //        Description = newAans.Description,
                //        AnswerByUserID = newAans.AnswerByUserID,
                //        LastUpdate = newAans.LastUpdate
                //    };
                //    _ipRehabContext.tblAnswer.Add(thisAnswer);
                //}
                #endregion

                await _ipRehabContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// update both existing episode and answers
        /// </summary>
        /// <param name="thisEpisodeID"></param>
        /// <param name="updatedAnswers"></param>
        /// <returns></returns>
        public async Task TransactionalUpdateAnswerAsync(int thisEpisodeID, List<UserAnswer> updatedAnswers)
        {
            using var transaction = _ipRehabContext.Database.BeginTransaction();
            try
            {
                #region update tblEpisodeOfCare
                var firstNewAnswer = updatedAnswers.First();
                var thisEpisode = await _ipRehabContext.tblEpisodeOfCare.FindAsync(thisEpisodeID);
                thisEpisode.OnsetDate = firstNewAnswer.OnsetDate;
                thisEpisode.AdmissionDate = firstNewAnswer.AdmissionDate;
                thisEpisode.PatientICNFK = firstNewAnswer.PatientID;
                thisEpisode.LastUpdate = firstNewAnswer.LastUpdate;

                _ipRehabContext.tblEpisodeOfCare.Update(thisEpisode);
                await _ipRehabContext.SaveChangesAsync();
                #endregion

                #region update tblAnswer
                _ipRehabContext.tblAnswer.UpdateRange(_mapper.Map<IEnumerable<tblAnswer>>(updatedAnswers));

                //foreach (UserAnswer updAns in updatedAnswers)
                //{
                //    var thisAnswer = _answerRepository.FindByCondition(x => x.AnswerID == updAns.AnswerID).FirstOrDefault();
                //    if (thisAnswer != null)
                //    {
                //        thisAnswer.QuestionIDFK = updAns.QuestionID;
                //        thisAnswer.MeasureIDFK = updAns.MeasureID;
                //        thisAnswer.AnswerCodeSetFK = updAns.AnswerCodeSetID;
                //        thisAnswer.AnswerSequenceNumber = updAns.AnswerSequenceNumber;
                //        thisAnswer.Description = updAns.Description;
                //        thisAnswer.AnswerByUserID = updAns.AnswerByUserID;
                //        thisAnswer.LastUpdate = updAns.LastUpdate;
                //    }
                //    _ipRehabContext.tblAnswer.Update(thisAnswer);
                //}
                #endregion
                await _ipRehabContext.SaveChangesAsync();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// delete answers, and delete this episode when all answers are deleted
        /// </summary>
        /// <param name="thisEpisodeID"></param>
        /// <param name="oldAnswers"></param>
        /// <returns></returns>
        public async Task TransactionalDeleteAsync(int thisEpisodeID, List<UserAnswer> oldAnswers)
        {
            using var transaction = _ipRehabContext.Database.BeginTransaction();
            try
            {
                #region delete tblAnswer
                //batch delete

                //method 1:
                //_ipRehabContext.tblAnswer.RemoveRange(oldAnswers.Select(_mapper.Map<tblAnswer>));
                /*method 2:*/
                _ipRehabContext.tblAnswer.RemoveRange(_mapper.Map<IEnumerable<tblAnswer>>(oldAnswers));
                //method 3:
                //_ipRehabContext.tblAnswer.RemoveRange(_mapper.Map<IEnumerable<UserAnswer>,IEnumerable<tblAnswer>>(oldAnswers));

                await _ipRehabContext.SaveChangesAsync();

                //loop delete
                //foreach (UserAnswer oldAns in oldAnswers)
                //{
                //    //remove answer
                //    var thisAnswer = _answerRepository.FindByCondition(x => x.AnswerID == oldAns.AnswerID).FirstOrDefault();
                //    if (thisAnswer != null)
                //    {
                //        _ipRehabContext.tblAnswer.Remove(thisAnswer);
                //        await _ipRehabContext.SaveChangesAsync();
                //    }
                //}

                #endregion

                #region delete tblEpisodeOfCare if no answer left
                var remainingAnswers = _answerRepository.FindByCondition(x => x.EpsideOfCareIDFK == thisEpisodeID);
                if (remainingAnswers == null || !remainingAnswers.Any())
                {
                    var thisEpisode = await _ipRehabContext.tblEpisodeOfCare.FindAsync(thisEpisodeID);
                    _ipRehabContext.tblEpisodeOfCare.Remove(thisEpisode);
                    await _ipRehabContext.SaveChangesAsync();
                }
                #endregion

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
