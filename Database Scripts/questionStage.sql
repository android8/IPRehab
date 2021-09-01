select 'q->',q.*, 'c->',c.*,  's->',s.*
from app.tblQuestionStage s
inner join app.tblquestion q
on s.QuestionIdFK = q.QuestionID
inner join app.tblCodeSet c
on s.StageFK = c.CodeSetID
where stagefk in (413) and questionkey like 'gg0130%' 
order by [order], QuestionKey 

select * from app.tblQuestion
left join app.tblQuestionStage
on questionid = questionidfk
left join app.tblcodeset
on stagefk = CodeSetID
where codesetid = 421
order by QuestionKey