select *
from app.tblCodeSet
where codesetparent in(145,367,368) or codesetid in (145, 118, 188, 344, 367,386,389) 
order by codesetparent

select * from app.tblQuestion where QuestionKey like 'J0530%'

--update app.tblCodeSet
--set CodesetParent = 188
--where CodeSetID in (386,389)