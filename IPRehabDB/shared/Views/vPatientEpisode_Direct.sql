
/* 
update: 10/25/2024 C. Jonathan Sun
directly use materialized patient_demographic and rptRehabDetails to list admitted patients with only two joins
*/

CREATE view [shared].[vPatientEpisode_Direct]
as 
SELECT 'p' p, p.[Last_Name],p.[First_Name],p.[PatientName],p.[PatientICN],p.[DoB],p.[scrssn]
, 'd' d, d.bsta6a, d.sta3, d.bedsecn, d.admitday, d.scrSSNT, d.scrnum, d.RealSSN, d.admission
, 'e' e, e.EpisodeOfCareID, e.OnsetDate, e.AdmissionDate, e.PatientICNFK, e.FacilityID6, e.LastUpdate, e.Completed
FROM  [app].[tblEpisodeOfCare] e 
left join [vhaausbi13].[DMTreatingSpecialty].[dbo].rptRehabDetails d on e.PatientICNFK = d.scrssn
left join [vhaausbi13].[DMTreatingSpecialty].[dbo].[patient_demographic] P 
on p.scrssn = e.PatientICNFK --and d.bsta6a = e.FacilityID6
