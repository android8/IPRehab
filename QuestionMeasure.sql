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

select --a.AnswerID, count(*)
'a'a,a.*,'m'm,m.*,'q'q,q.questionid, q.questionkey
--'m'm,m.id,m.QuestionIDFK 'qidfk',m.stagefk 'qstage',m.MeasureFK 'mfk', 
--'stage'cs, cs.CodeSetID, cs.CodeValue, cs.CodeDescription,
--'measure'cm,cm.codesetid, cm.CodeValue, cm.CodeDescription
from app.tblQuestionMeasure m
--inner join app.tblCodeSet cS
--on m.StageFK = cs.codesetid
inner join app.tblquestion q
on m.QuestionIDFK = q.QuestionID
--left join app.tblCodeset cM
--on m.MeasureFK = cM.CodeSetID
left join app.tblAnswer a
on a.QuestionIDFK = q.QuestionID --and a.MeasureIDFK = m.Id
where a.answerid is not null and a.MeasureIDFK is null
--where /*q.questionkey = 'q12' and a.EpsideOfCareIDFK = 30 and*/ a.StageIDFK = 421 and m.StageFK =421
--where m.StageFK in (421)
--where questionkey not like 'q%' --or questionkey like 'm0300%' or questionkey in ('bb0700','bb0800','h0350','h0400','j0510','j0520','j0530','m0210')
--group by a.answerid having count(*) > 1
order by a.AnswerID, a.EpsideOfCareIDFK, [order], QuestionKey

--select 'q'q,q.*,'m'm,m.* from app.tblQuestionMeasure m
--inner join app.tblquestion q
--on m.QuestionIDFK = q.QuestionID --where MeasureFK is null 
----and QuestionKey like 'q%' or QuestionKey like 'assess%'
----and q.QuestionID in (
----	select QuestionIDFK from app.tblQuestionMeasure m
----	inner join app.tblquestion q
----	on m.QuestionIDFK = q.QuestionID
----	group by QuestionIDFK, QuestionKey having count(*) > 1 and QuestionKey like 'q%'
----) 
--order by [order],q.questionkey