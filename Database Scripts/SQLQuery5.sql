USE [RehabMetricsAndOutcomes]
GO

/****** Object:  View [app].[vCodeSetHierarchy]    Script Date: 4/16/2021 11:06:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER view [app].[vCodeSetHierarchy]
as
select top 100 percent
hierarchy.CodeDescription 'Hierarchy',
child.[CodeSetID] 'CHILD ID', child.codeValue 'CHILD value', child.CodeDescription 'CHILD description',
parent.CodeSetID 'PARENT ID', parent.codeValue 'PARENT value', parent.CodeDescription 'PARENT description', parent.Comment 'PARENT comment',
grand.CodeSetID 'GRAND ID', grand.CodeValue 'GRAND value', grand.CodeDescription 'GRAND description',
greatGrand.CodeSetID 'GREAT ID', greatGrand.CodeValue 'GREAT value', greatGrand.CodeDescription 'GREAT description',
ancestor.CodeSetID 'ANCESTOR ID', ancestor.CodeValue 'ANCESTOR value', ancestor.CodeDescription 'ANCESTOR description',
antiquity.CodeSetID 'ANTIQUITY ID', antiquity.CodeValue 'ANTIQUITY value', antiquity.CodeDescription 'ANTIQUITY description'
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

GO


