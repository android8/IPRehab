




/****** Script for SelectTopNRows command from SSMS  ******/
CREATE view [shared].[vFSODPatientDetail]
as
SELECT 
[VISN],[Facility],[District],[Division],[ADMParent_Key],[Sta6aKey],[bedsecn],[Name],[PTFSSN],[FSODSSN],[FiscalPeriod],[FiscalPeriodInt]
  FROM Openquery(VHAausbi25,
	'
		select [VISN],[Facility],[District],[Division],[ADMParent_Key],[Sta6aKey],[bedsecn],[Name],[PTFSSN],[FSODSSN],[FiscalPeriod],[FiscalPeriodInt]
		from [DMHealthFactors].[FSOD].[FSODPatientDetailFY22Q3]
		--where len(a.stationnumber) = 3
		order by visn,Facility
	'
	) as RemoteQuery

	/* remote query xml example*/
