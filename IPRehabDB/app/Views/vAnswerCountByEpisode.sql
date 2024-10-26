/****** Script for SelectTopNRows command from SSMS  ******/
create view app.vAnswerCountByEpisode
as
SELECT EpisodeOfCareID, PatientICNFK, count(EpisodeOfCareID) 'AnswerCount'
--[EpisodeOfCareID]
--      ,[PatientICNFK]
--      ,[QuestionKey]
--      ,[Question]
--      ,[stage name]
--      ,[CodeDescription]
--      ,[AnswerID]
--      ,[AnswerCodeSetFK]
--      ,[coded answer]
--      ,[non-coded text]
--      ,[AnswerByUserID]
  FROM [RehabAssessmentCareTool].[app].[vQuestionAnswers]
  group by EpisodeOfCareID, PatientICNFK
--order by EpisodeOfCareID