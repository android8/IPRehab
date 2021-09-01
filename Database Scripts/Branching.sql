--update app.tblBranching set Condition = 'if Q44C = 1' where BranchingID in (7)
insert app.tblBranching
select FromQuestionID=47, ToQuestionID=49, Condition='if Q44D = 1' 
--from app.tblQuestion
--where BranchToQuestionID  is not null

  select * from app.tblQuestion where questionkey in ('Q44D','Q45') --47,49

SELECT [BranchingID],[FromQuestionID],fromq.QuestionKey,[ToQuestionID],toq.[QuestionKey], b.Condition
--,i.Instruction
  FROM [RehabAssessmentCareTool].[app].[tblBranching] b
  inner join app.tblQuestion fromQ
  on b.FromQuestionID = fromQ.QuestionID
  inner join app.tblQuestion toQ
  on b.ToQuestionID = toQ.QuestionID
  --left join app.tblQuestionInstruction i
  --on fromq.QuestionID = i.QuestionIDFK
  order by fromq.[order], fromq.questionkey
