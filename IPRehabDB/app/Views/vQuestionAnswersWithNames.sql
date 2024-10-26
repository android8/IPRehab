
/* data dump for facility 678, change the where clause for other facility*/
CREATE view [app].[vQuestionAnswersWithNames]
as
SELECT c.EpisodeOfCareID, c.PatientICNFK, q.[Order], q.QuestionID, q.QuestionKey, q.Question,
s.CodeDescription 'stage name', mc.CodeDescription, 
a.AnswerID, a.AnswerCodeSetFK, 
answerCodeset.CodeValue, answerCodeset.codeDescription 'coded answer', 
a.[Description] 'non-coded text',
c.FacilityID6, a.AnswerByUserID, a.LastUpdate,
p.*
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
  left join 
	[shared].[vTreatingSpecialtyRecent3Yrs] P
on p.bsta6a = FacilityID6 and c.PatientICNFK = P.Scrssn or c.PatientICNFK = P.ScrssnT or c.PatientICNFK = P.scrnum or c.PatientICNFK = p.RealSSN or c.PatientICNFK = p.PatientICN
  where FacilityID6 like '678%' --and c.EpisodeOfCareID in 
  --(select EpsideOfCareIDFK from app.tblAnswer where QuestionIDFK = 526 )
  --order by EpisodeOfCareID, q.[Order] 
