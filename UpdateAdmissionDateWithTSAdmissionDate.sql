use RehabAssessmentCareTool
go
with detail as (SELECT distinct p.[Last_Name],p.[First_Name],p.[PatientName],p.[DoB],p.[PatientICN],p.PatientSSN, ts.scrssn, ts.RealSSN, h.ptfssn, e.PatientICNFK
,ts.bsta6a,max(ts.[admitday]) as MostRecent--, e.AdmissionDate
--,ts.[beddate],ts.[C_LOS],ts.[los],ts.statyp
from  vhaausbi13.DMTreatingSpecialty.[dbo].[FactTSAll_Day_CDW_2yrs] ts
--inner join VHAAUSBI1.DWWorkload.[dbo].[Patient_Demographic] p
inner join VHAAUSBI13.DMTreatingSpecialty.[dbo].[patient_demographic] p
on ts.scrssn = p.scrssn 
inner join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY22Q3 h
on h.ptfssn = p.patientSSN
inner join app.tblEpisodeOfCare e 
on h.PTFSSN = e.PatientICNFK or h.[FSODSSN] = e.PatientICNFK

where 
--ts.bsta6a like '648%' and 
ts.[bedsecn] in (22,20,112,64) and C_LOS >= 2 --and [statyp] in (98,40)
group by p.[Last_Name],p.[First_Name],p.[PatientName],p.[DoB],p.[PatientICN],p.PatientSSN, ts.scrssn, ts.RealSSN,  h.ptfssn, e.PatientICNFK, ts.bsta6a--, e.AdmissionDate
)

select * from detail

--update app.tblEpisodeOfCare
--set admissionDate = MostRecent from detail
--where detail.PatientICN = tblEpisodeOfCare.PatientICNFK 
GO