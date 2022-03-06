--update app.tblAnswer
--set tblAnswer.MeasureIDFK = m.Id
--select * 
from app.tblAnswer a
inner join app.tblQuestion q
on a.QuestionIDFK = q.QuestionID
inner join app.tblQuestionMeasure m
on m.QuestionIDFK = q.QuestionID
where m.StageFK = 414 and m.MeasureFK =459
order by a.answerid