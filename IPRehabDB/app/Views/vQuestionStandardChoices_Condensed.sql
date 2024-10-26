



CREATE view [app].[vQuestionStandardChoices_Condensed]
as
select top 100 percent 
--c.[CHILD value] 'Form', d.[CHILD value] 'Section', 
c.[CHILD description] Measure, d.[CHILD description] Stage,
a.QuestionKey, a.[order], a.Question, a.AnswerCodeSetFK 'Code Set', b.[CHILD value] 'Choice Code', b.[CHILD description] 'Valid Choice', b.[Child comment] 'Code Value Comment', b.[PARENT comment], b.[GRAND comment], b.[GREAT comment], b.[ANTIQUITY Comment]
from [app].[tblQuestion] a
left join [app].[vCodeSetHierarchy] b
on a.AnswerCodeSetFK = b.[Parent id]
left join [app].tblQuestionMeasure m
on a.QuestionID = m.QuestionIDFK
left join [app].[vCodeSetHierarchy] c
on m.MeasureCodeSetIDFK = c.[CHILD ID]
left join [app].[vCodeSetHierarchy] d
on m.StageFK = d.[CHILD ID]
order by 
--case
--	when c.[child value] = 'form-basicinfo' then 1
--	when c.[child value] = 'form-admission' then 2
--	when c.[child value] = 'form-discharge' then 3
--end,
a.[Order], a.QuestionKey, c.[CHILD description]--,d.[child value]
