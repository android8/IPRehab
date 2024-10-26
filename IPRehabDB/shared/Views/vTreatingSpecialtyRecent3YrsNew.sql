
CREATE view [shared].[vTreatingSpecialtyRecent3YrsNew]
as 
select p.Last_Name, p.First_Name, p.PatientName, p.DoB
, ts.bsta6a, ltrim(substring(ts.bsta6a,1,3)) as sta3, ts.bedsecn, ts.admitday, ts.scrssn, ts.scrnum, ts.RealSSN, ts.ScrSSNT, p.PatientICN
from shared.vTreatingSpecialtyPatNoName ts
inner join (select [Scrssn], [Last_Name],[First_Name],[PatientName],[PatientICN],[DoB]
	from VHAAUSBI13.DMTreatingSpecialty.[dbo].[patient_demographic]
	) p
on ts.scrssn = cast(p.scrssn as int)
--or ts.scrnum = cast(p.[SCRNUM] as int)
and 
[bedsecn] in (22,20,112,64) and ts.C_LOS >= 2 --and [statyp] in (98,40)
