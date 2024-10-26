

CREATE view [app].[vQuestionStandardChoices]
as
select c.[CHILD value] 'Form', d.[CHILD value] 'Section', a.*, b.*
from [app].[tblQuestion] a
left join [app].[vCodeSetHierarchy] b
on a.AnswerCodeSetFK = b.[Parent id]
left join [app].[vCodeSetHierarchy] c
on a.FormFK = c.[CHILD ID]
left join [app].[vCodeSetHierarchy] d
on a.FormSectionFK = d.[CHILD ID]
