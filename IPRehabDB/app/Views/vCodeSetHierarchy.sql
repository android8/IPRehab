

CREATE view [app].[vCodeSetHierarchy]
as
select top 100 percent
hierarchy.CodeDescription 'Hierarchy',
child.[CodeSetID] 'CHILD ID', child.codeValue 'CHILD value', child.CodeDescription 'CHILD description', child.Comment 'CHILD comment',
parent.CodeSetID 'PARENT ID', parent.codeValue 'PARENT value', parent.CodeDescription 'PARENT description', parent.Comment 'PARENT comment',
grand.CodeSetID 'GRAND ID', grand.CodeValue 'GRAND value', grand.CodeDescription 'GRAND description', grand.Comment 'GRAND comment',
greatGrand.CodeSetID 'GREAT ID', greatGrand.CodeValue 'GREAT value', greatGrand.CodeDescription 'GREAT description', greatGrand.Comment 'GREAT comment',
ancestor.CodeSetID 'ANCESTOR ID', ancestor.CodeValue 'ANCESTOR value', ancestor.CodeDescription 'ANCESTOR description', ancestor.Comment 'ANCESTER comment',
antiquity.CodeSetID 'ANTIQUITY ID', antiquity.CodeValue 'ANTIQUITY value', antiquity.CodeDescription 'ANTIQUITY description', ANTIQUITY.Comment 'ANTIQUITY comment'
from app.tblCodeSet child
left join app.tblCodeSet parent
on child.CodeSetParent = parent.CodeSetID
left join app.tblCodeSet grand
on parent.CodeSetParent = grand.codesetId
left join app.tblCodeSet greatGrand
on grand.CodeSetParent = greatGrand.codesetId
left join app.tblCodeSet ancestor
on greatGrand.CodeSetParent = ancestor.CodeSetID
left join app.tblCodeSet antiquity
on ancestor.CodeSetParent = antiquity.CodeSetID
left join app.tblCodeSet hierarchy
on hierarchy.CodeSetID = child.hierarchytype

order by greatGrand.CodeValue,grand.CodeValue,parent.CodeValue, child.CodeValue

