--update app.tblQuestionMeasure
--set MeasureFK = (
--case when StageFK = 421 then 456 
--When stagefk = 416 then 457
--when stagefk = 414 then 459
--when stagefk =415 then 461
--end)
--where QuestionIDFK in (
--select questionid from app.tblQuestion
--where questionkey like 'gg%' or questionkey like 'm0300%' or (questionkey in ('bb0700','bb0800','h0350','h0400','j0510','j0520','j0530','m0210')
--))

--select * from app.tblcodeset where CodeDescription like '%measure%' or codesetparent in (455)
--or codesetparent in (412) or codesetid in (412)

select 'm'm,m.*,
--'m'm,m.id,m.QuestionIDFK 'qidfk',m.stagefk 'qstage',m.MeasureFK 'mfk', 
'q'q,q.questionkey,
'stage'cs, cs.CodeSetID, cs.CodeValue,
'measure'cm,cm.codesetid, cm.CodeValue, cm.CodeDescription
from app.tblQuestionMeasure m
inner join app.tblCodeSet cS
on m.StageFK = cs.codesetid
inner join app.tblquestion q
on m.QuestionIDFK = q.QuestionID
left join app.tblCodeset cM
on m.MeasureFK = cM.CodeSetID
where m.StageFK in (421)
--where questionkey not like 'q%' --or questionkey like 'm0300%' or questionkey in ('bb0700','bb0800','h0350','h0400','j0510','j0520','j0530','m0210')
order by [order], QuestionKey