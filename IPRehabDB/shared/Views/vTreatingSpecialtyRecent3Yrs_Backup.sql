









/*
List of patients from Treating Specialty cube and join to Patient Demographic in BI1.DWWorkload for
those in bedsection 22, 20, 112, 64 and Length of Stay (C_LOS) equal to or longer than 2 days
*/
CREATE view [shared].[vTreatingSpecialtyRecent3Yrs_Backup]
as
SELECT distinct p.[Last_Name],p.[First_Name],p.[PatientName],  p.[PatientICN],p.[DoB],
ts.scrssn,ts.bsta6a,ts.[bedsecn],ts.[admitday]
from  vhaausbi13.DMTreatingSpecialty.[dbo].[FactTSAll_Day_CDW_2yrs] ts
--inner join (
--	select [Scrssn], [Last_Name],[First_Name],[PatientName],[PatientICN],[DoB]
--	from VHAAUSBI13.DMTreatingSpecialty.[dbo].[patient_demographic]
--) p
inner join VHAAUSBI13.DMTreatingSpecialty.[dbo].[patient_demographic] p
on ts.scrssn = cast(p.scrssn as int)
where 
[bedsecn] in (22,20,112,64) and C_LOS >= 2 --and [statyp] in (98,40)
and (
getdate() - 180 <= ts.[admitday])
