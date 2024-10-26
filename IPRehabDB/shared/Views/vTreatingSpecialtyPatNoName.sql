


CREATE view [shared].[vTreatingSpecialtyPatNoName]
as
SELECT distinct t.scrnum ,t.scrssn, t.ScrSSNT ,t.RealSSN, t.bsta6a,t.[bedsecn],t.C_LOS,[admitday]
from  vhaausbi13.DMTreatingSpecialty.[dbo].[FactTSAll_Day_CDW_2yrs] as t
where 
[bedsecn] in (22,20,112,64) and C_LOS >= 2 --and [statyp] in (98,40)
and getdate() - 180 <= [admitday]
