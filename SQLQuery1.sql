select *
from app.tblQuestion q
where q.questionkey like 'AssessmentCompleted%' order by QuestionKey

select * from app.tblQuestionStage
where QuestionIDFK in (select QuestionID
from app.tblQuestion q
where q.questionkey like 'AssessmentCompleted%') order by QuestionIDFK

select * from app.tblQuestionInstruction i
inner join app.tblQuestion q
on i.QuestionIDFK = q.QuestionID
where q.questionkey like 'AssessmentCompleted%' order by QuestionID

select * from app.tblcodeset where codevalue like '%checked%' or codesetparent in (367)
