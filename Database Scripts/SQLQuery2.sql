select *
from app.tblcodeset
where codesetid not in (select distinct codesetparent from app.tblCodeSet where CodeSetParent is not null)
and len(rtrim(codevalue))>2

