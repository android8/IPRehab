﻿

CREATE view [app].[vQuestionAnswers]
as
SELECT c.EpisodeOfCareID, c.PatientICNFK, q.[Order], q.QuestionID, q.QuestionKey, q.Question,
s.CodeDescription 'stage name', mc.CodeDescription 'measure description', 
a.AnswerID, a.AnswerCodeSetFK, 
answerCodeset.CodeValue, answerCodeset.codeDescription 'answer description', 
a.[Description] 'non-coded text',
c.FacilityID6, a.AnswerByUserID, a.LastUpdate
  FROM app.tblQuestion q
  inner join app.tblAnswer a
  on q.QuestionID = a.QuestionIDFK 
  inner join app.tblEpisodeOfCare c
  on a.EpsideOfCareIDFK = c.EpisodeOfCareID
  inner join app.tblCodeSet answerCodeset
  on a.AnswerCodeSetFK = answerCodeset.CodeSetID
  left join app.tblQuestionMeasure m
  on q.QuestionID = m.QuestionIDFK and a.MeasureIDFK = m.Id
  inner join app.tblCodeSet s
  on m.StageFK = s.CodeSetID
  left join app.tblcodeset mc
  on m.MeasureCodeSetIDFK = mc.CodeSetID 
  --where FacilityID6 like '595%' and c.EpisodeOfCareID in 
  --(select EpsideOfCareIDFK from app.tblAnswer where QuestionIDFK = 526 )
  --order by EpisodeOfCareID, q.[Order] 