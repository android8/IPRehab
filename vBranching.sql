/****** Script for SelectTopNRows command from SSMS  ******/
SELECT b.BranchingID, b.BranchingName, f.questionkey fQ,t.QuestionKey tQ, b.condition 
  FROM [RehabAssessmentCareTool].[app].[tblBranching] b
  inner join app.tblQuestion f
  on b.FromQuestionID = f.QuestionID
  inner join app.tblQuestion t
  on b.ToQuestionID = t.QuestionID

  