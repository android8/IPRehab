select q.questionkey, q.QuestionID, a.* from [app].[tblAnswer] a
inner join [app].tblQuestion q
on a.QuestionIDFK = q.QuestionID
where questionkey like 'q%'
order by EpsideOfCareIDFK, questionidfk 