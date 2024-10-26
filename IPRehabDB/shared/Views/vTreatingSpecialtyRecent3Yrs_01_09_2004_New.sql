
/*
List of patients from Treating Specialty cube and join to Patient Demographic in BI1.DWWorkload for
those in bedsection 22, 20, 112, 64 and Length of Stay (C_LOS) equal to or longer than 2 days
*/
CREATE view [shared].[vTreatingSpecialtyRecent3Yrs_01_09_2004_New]
as
SELECT distinct --RealSSN, count(*), min(ts.admission)
p.[Last_Name],p.[First_Name],p.[PatientName], p.[DoB], 
ts.bsta6a, ltrim(substring(ts.bsta6a,1,3)) as sta3, 
ts.[bedsecn],ts.[admitday], 
ts.ScrSSNT, ts.scrssn, ts.scrnum, ts.RealSSN,  
p.patientSSN, p.[PatientICN], min(ts.admission) [admission]

from  vhaausbi13.DMTreatingSpecialty.[dbo].[FactTSAll_Day_CDW_2yrs] ts
inner join (
	select [patientssn],[Scrssn], [Last_Name],[First_Name],[PatientName],[PatientICN],[DoB]
	from VHAAUSBI13.DMTreatingSpecialty.[dbo].[patient_demographic]
) p
on trim(cast(ts.scrssn as varchar)) = p.scrssn 
--or trim(cast(ts.scrnum as varchar)) =  p.scrnum 
-- or trim(cast(ts.RealSSN as varchar)) = p.patientssn 
-- or trim(ts.scrssnt) = p.PatientSSN
and [bedsecn] in (22,20,112,64) and C_LOS >= 2 
and ts.admitday >= '10/01/2022'
group by 
p.[Last_Name],p.[First_Name],p.[PatientName], p.[DoB], 
ts.bsta6a, ts.[bedsecn],ts.[admitday], ts.ScrSSNT, ts.scrssn, ts.scrnum, ts.RealSSN, p.[patientssn], p.[PatientICN]

