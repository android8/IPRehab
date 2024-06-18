--select 'e' e, e.*, 'q' q, q.*, 'a' a, a.*--, 's', s.*, 'm' m, m.*
select 'e' e, e.EpisodeOfCareID, e.FacilityID6, e.PatientICNFK, 'q' q, q.[order], q.QuestionKey, 
'a' a, a.EpsideOfCareIDFK, a.AnswerID, a.AnswerCodeSetFK, a.QuestionIDFK, a.LastUpdate, a.AnswerByUserID
from app.tblEpisodeOfCare e inner join app.tblAnswer a
on e.EpisodeOfCareID = a.EpsideOfCareIDFK inner join app.tblQuestion q
on a.QuestionIDFK = q.QuestionID --inner join app.tblQuestionMeasure m
-- on a.MeasureIDFK = m.Id INNER join app.tblCodeSet s
-- on a.AnswerCodeSetFK = s.CodeSetID
where a.lastupdate >= getdate()-2 --and  EpisodeOfCareID = 1637 
order by a.LastUpdate, e.FacilityID6, q.[Order] desc
--where e.EpisodeOfCareID = 1562 --and q.questionkey ='AssessmentCompleted'
--where e.FacilityID6 like '512%'and q.questionkey ='AssessmentCompleted'
-- and e.PatientICNFK in (
-- select e.PatientICNFK from  app.tblEpisodeOfCare e
-- where e.FacilityID6 like '512%'
-- group by e.PatientICNFK
-- having count(*)>1
-- )
--order by q.[order], q.QuestionKey
--order by a.QuestionIDFK

select * from app.tblEpisodeOfCare
where PatientICNFK in (
select e.PatientICNFK from  app.tblEpisodeOfCare e
--where e.FacilityID6 like '512%'
where e.EpisodeOfCareID in (1560)
group by e.PatientICNFK
having count(*)>1
)

select * from app.tblQuestion
where AnswerCodeSetFK in (83)

