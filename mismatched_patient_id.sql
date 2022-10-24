/****** Script for SelectTopNRows command from SSMS  ******/
SELECT [EpisodeOfCareID]
      ,[OnsetDate]
      ,[AdmissionDate], ts.admitDay
      ,[PatientICNFK]
      ,[FacilityID6]
      ,[LastUpdate]
      ,[Completed]
	  , h2203.ptfssn as h2203_ptfssn
	  , h2202.ptfssn as h2202_ptfssn
	  , h2201.ptfssn as h2201_ptfssn
	  , h2104.ptfssn as h2104_ptfssn
	  , h2103.ptfssn as h2103_ptfssn
	  , h2102.ptfssn as h2102_ptfssn
	  , h2101.ptfssn as h2101_ptfssn
	  , h2004.ptfssn as h2004_ptfssn
	  , h2003.ptfssn as h2003_ptfssn
	  , h2002.ptfssn as h2002_ptfssn
	  --, h2001.ptfssn as h2001_ptfssn
	  --, p.scrssn as p_scrssn, p.PatientSSN  as p_PatientSSN
	  , ts.scrssn as ts_scrssn, ts.[REALSSN] as ts_realSSN
  FROM [RehabAssessmentCareTool].[app].[tblEpisodeOfCare] e
  left join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY22Q3 h2203 on h2203.ptfssn = e.PatientICNFK
  left join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY22Q2 h2202 on h2202.ptfssn = e.PatientICNFK
left join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY22Q1 h2201 on h2201.ptfssn = e.PatientICNFK
left join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY21Q4 h2104 on h2104.ptfssn = e.PatientICNFK
left join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY21Q3 h2103 on h2103.ptfssn = e.PatientICNFK
left join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY21Q2 h2102 on h2102.ptfssn = e.PatientICNFK
left join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY21Q1 h2101 on h2101.ptfssn = e.PatientICNFK
left join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY20Q4 h2004 on h2004.ptfssn = e.PatientICNFK
left join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY20Q3 h2003 on h2003.ptfssn = e.PatientICNFK
left join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY20Q2 h2002 on h2002.ptfssn = e.PatientICNFK
--left join VHAAUSBI25.DMHealthFactors.FSOD.FSODPatientDetailFY20Q1 h2001 on h2001.ptfssn = e.PatientICNFK
left join VHAAUSBI13.DMTreatingSpecialty.[dbo].[FactTSAll_Day_CDW_2yrs] ts on ts.[REALSSN] = e.PatientICNFK
--left join VHAAUSBI13.DMTreatingSpecialty.[dbo].[patient_demographic] p on /* p.scrssn = e.PatientICNFK or*/  p.PatientSSN = ts.[REALSSN]
and ts.[bedsecn] in (22,20,112,64) and C_LOS >= 2 --and ts.admitDay >= getdate() - 90
order by PatientICNFK, FacilityID6

select * from VHAAUSBI13.DMTreatingSpecialty.[dbo].[patient_demographic]
where scrssn = 595246380

select * from app.tblEpisodeOfCare a
where a.FacilityID6 like '%648%'
