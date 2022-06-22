/* use [VHAAusBI13.vha.med.va.gov].DMTreatingSpecialty */
use DMTreatingSpecialty
go

/* old quaterly update */
/* use [VHAAUSBI25.vha.med.va.gov].[DMHealthFactors]*/
--go

declare @StartDate datetime ='2021-10-01'
declare @EndDate datetime ='2022-06-30'
declare @facility varchar(6) = '%648%'
--declare @admitdatetime datetime ='2021-10-01'
--declare @dischargedatetime datetime ='2021-12-31'
--SELECT  --distinct 
--HF.*,
--[VISN]
--      ,[Facility]
--     -- ,[District]
--     -- ,[Division]
--     -- ,[ADMParent_Key]
--     --,[Sta6aKey]
--     -- ,HF.[bedsecn]
--     -- ,[Name]
--      ,[PTFSSN]
--      --,[FSODSSN]
--      --,[FiscalPeriod]
--      --,[FiscalPeriodInt]
--	  --, TS.REALSSN
--from VHAAUSBI25.[DMHealthFactors].[FSOD].[FSODPatientDetailFY22Q1]  HF
--where HF.facility like @facility
--order by hf.bedsecn
--left join 
--(

select realssn, [inpatientsid],[bsta6a],[bedsecn],[discharge],
[bsinday],[bsoutday],[disday],[admitdatetime],[dischargedatetime],[bsindatetime],[bsoutdatetime],[bsindaynew],[bsoutdaynew]
from VHAAusBI13.DMTreatingSpecialty.[dbo].[FactTSAll_Day_CDW_2yrs]
where (admission = 1 or transin = 1) 
and bedsecn in (20,112,64) 
and C_LOS>2 and statyp in (98,40)
and bsta6a like @facility
and bsinday between @StartDate and @EndDate
order by realssn
--admitdatetime desc, dischargedatetime desc
--) TS
--on HF.[PTFSSN]  = TS.realssn
--where HF.facility like @facility 
--and TS.REALSSN is null
