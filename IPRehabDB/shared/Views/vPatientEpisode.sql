
/*
	Using [RehabAssessmentCareTool].[shared].[vTreatingSpecialtyRecent3Yrs] to list patients with or without episode. 
    The [shared].[vPatientEpisode_Direct] is a replacement and preferred where no joins are needed.
*/

CREATE view [shared].[vPatientEpisode]
as 
SELECT [Last_Name],[First_Name],[PatientName],[PatientICN],[DoB],[scrssn],[bsta6a],[bedsecn],[admitday]
,e.EpisodeOfCareID, e.OnsetDate, e.AdmissionDate, e.PatientICNFK, e.FacilityID6, e.LastUpdate, e.Completed
  FROM [RehabAssessmentCareTool].[shared].[vTreatingSpecialtyRecent3Yrs] p
  left join [app].[tblEpisodeOfCare] e
  on p.scrssn = e.PatientICNFK and p.bsta6a = e.FacilityID6
  --where p.bsta6a like '648%' or (p.bsta6a like '648%' and e.PatientICNFK is not null)
  --order by p.scrssn
