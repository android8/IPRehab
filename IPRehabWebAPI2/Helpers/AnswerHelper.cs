using IPRehabModel;
using IPRehabRepository;
using IPRehabRepository.Contracts;
using IPRehabWebAPI2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserModel;

namespace IPRehabWebAPI2.Helpers
{
  public class AnswerHelper
  {
    private IPRehabContext _ipRehabContext;
    private readonly IAnswerRepository _answerRepository;

    /// <summary>
    /// constructor injection of IPRehabContext in order to use _ipRehabContext transaction
    /// </summary>
    /// <param name="iPRehabContext"></param>
    /// <param name="answerRepository"></param>
    public AnswerHelper(IPRehabContext iPRehabContext, IAnswerRepository answerRepository)
    {
      _ipRehabContext = iPRehabContext;
      _answerRepository = answerRepository;
    }

    /// <summary>
    /// add new episode and new answers
    /// </summary>
    /// <param name="newAnswers"></param>
    /// <returns></returns>
    public async Task TransactionalInsertAsync(List<UserAnswer> newAnswers)
    {
      using var transaction = _ipRehabContext.Database.BeginTransaction();
      try
      {
        var firstNewAnswer = newAnswers.First();
        tblEpisodeOfCare thisEpisode = new()
        {
          OnsetDate = firstNewAnswer.OnsetDate,
          AdmissionDate = firstNewAnswer.AdmissionDate,
          PatientICNFK = firstNewAnswer.PatientID,
          LastUpdate = firstNewAnswer.LastUpdate
        };

        _ipRehabContext.tblEpisodeOfCare.Add(thisEpisode);
        await _ipRehabContext.SaveChangesAsync(); //must SaveChangesAsync() to get new ID

        foreach (UserAnswer ans in newAnswers)
        {
          tblAnswer thisAnswer = new()
          {
            EpsideOfCareIDFK = thisEpisode.EpisodeOfCareID, //new Episode
            AnswerByUserID = ans.AnswerByUserID,
            AnswerCodeSetFK = ans.AnswerCodeSetID,
            AnswerSequenceNumber = ans.AnswerSequenceNumber,
            Description = ans.Description,
            QuestionIDFK = ans.QuestionID,
            StageIDFK = ans.StageID,
            LastUpdate = ans.LastUpdate
          };
          _ipRehabContext.tblAnswer.Add(thisAnswer);
        }
        
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
    /// add only new answers to exisitng episode
    /// </summary>
    /// <param name="thisEpisodeID"></param>
    /// <param name="newAnswers"></param>
    /// <returns></returns>
    public async Task TransactionalInsertAnswerOnlyAsync(int thisEpisodeID, List<UserAnswer> newAnswers)
    {
      using var transaction = _ipRehabContext.Database.BeginTransaction();
      try
      {
        foreach (UserAnswer ans in newAnswers)
        {
          tblAnswer thisAnswer = new()
          {
            EpsideOfCareIDFK = thisEpisodeID, //existing Episode ID
            AnswerByUserID = ans.AnswerByUserID,
            AnswerCodeSetFK = ans.AnswerCodeSetID,
            AnswerSequenceNumber = ans.AnswerSequenceNumber,
            Description = ans.Description,
            QuestionIDFK = ans.QuestionID,
            StageIDFK = ans.StageID,
            LastUpdate = ans.LastUpdate
          };
          _ipRehabContext.tblAnswer.Add(thisAnswer);
        }
        
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
    /// <param name="udatedAnswers"></param>
    /// <returns></returns>
    public async Task TransactionalUpdateAnswerAsync(int thisEpisodeID, List<UserAnswer> udatedAnswers)
    {
      using var transaction = _ipRehabContext.Database.BeginTransaction();
      try
      {
        var firstNewAnswer = udatedAnswers.First();
        var thisEpisode = await _ipRehabContext.tblEpisodeOfCare.FindAsync(thisEpisodeID);
        thisEpisode.OnsetDate = firstNewAnswer.OnsetDate;
        thisEpisode.AdmissionDate = firstNewAnswer.AdmissionDate;
        thisEpisode.PatientICNFK = firstNewAnswer.PatientID;
        thisEpisode.LastUpdate = firstNewAnswer.LastUpdate;

        _ipRehabContext.tblEpisodeOfCare.Update(thisEpisode);
        //await _ipRehabContext.SaveChangesAsync();

        foreach (UserAnswer ans in udatedAnswers)
        {
          var thisAnswer = _answerRepository.FindByCondition(x => x.AnswerID == ans.AnswerID).FirstOrDefault();
          if (thisAnswer != null)
          {
            thisAnswer.AnswerByUserID = ans.AnswerByUserID;
            thisAnswer.AnswerCodeSetFK = ans.AnswerCodeSetID;
            thisAnswer.AnswerSequenceNumber = ans.AnswerSequenceNumber;
            thisAnswer.Description = ans.Description;
            thisAnswer.QuestionIDFK = ans.QuestionID;
            thisAnswer.StageIDFK = ans.StageID;
            thisAnswer.LastUpdate = ans.LastUpdate;
          }
          _ipRehabContext.tblAnswer.Update(thisAnswer);
        }
        
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
    /// only delete answers, do not delte episode
    /// </summary>
    /// <param name="thisEpisodeID"></param>
    /// <param name="oldAnswers"></param>
    /// <returns></returns>
    public async Task TransactionalDeleteAsync(int thisEpisodeID, List<UserAnswer> oldAnswers)
    {
      using var transaction = _ipRehabContext.Database.BeginTransaction();
      try
      {
        foreach (UserAnswer ans in oldAnswers)
        {
          var thisAnswer = _answerRepository.FindByCondition(x => x.AnswerID == ans.AnswerID).FirstOrDefault();
          if (thisAnswer != null)
          {
            _ipRehabContext.tblAnswer.Remove(thisAnswer);
            //await _ipRehabContext.SaveChangesAsync();
          }
        }

        if (!_answerRepository.FindByCondition(x => x.EpsideOfCareIDFK == thisEpisodeID).Any())
        {
          var thisEpisode = await _ipRehabContext.tblEpisodeOfCare.FindAsync(thisEpisodeID);
          _ipRehabContext.tblEpisodeOfCare.Remove(thisEpisode);
        }
        
        await _ipRehabContext.SaveChangesAsync();
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
