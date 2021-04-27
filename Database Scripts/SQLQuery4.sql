USE [RehabMetricsAndOutcomes]
GO

/****** Object:  View [app].[vQuestionStandardChoices_Condensed]    Script Date: 4/16/2021 11:02:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER view [app].[vQuestionStandardChoices_Condensed]
as
select top 100 percent 
a.QuestionID, a.QuestionKey, a.[order], a.Question, a.AnswerCodeSetFK 'Code Set', b.[CHILD description] 'Valid Choice', b.[CHILD value] 'Choice Code', 
b.[Parent comment] 'Code Set Comment'
from [app].[tblQuestion] a
left join [app].[vCodeSetHierarchy] b
on a.AnswerCodeSetFK = b.[Parent id]
order by a.[order]
GO


