declare @StartDate datetime ='2020-10-01'
declare @EndDate datetime ='2021-06-30'
declare @facility varchar(6) = '%648%'

--SELECT  distinct [VISN]
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
--	  , TS.REALSSN
--from VHAAUSBI25.[DMHealthFactors].[FSOD].[FSODPatientDetailFY21Q4]  HF
----where HF.facility like @facility
--left join 
--(
select realssn, [inpatientsid],[bsta6a],[bedsecn],[discharge],
[bsinday],[bsoutday],[disday],[admitdatetime],[dischargedatetime],[bsindatetime],[bsoutdatetime],[bsindaynew],[bsoutdaynew]
from [dbo].[FactTSAll_Day_CDW_2yrs]
where (admission = 1 or transin = 1) 
and bedsecn in (20,112,64) 
and C_LOS>2 and statyp in (98,40)
and bsta6a like @facility
and bsinday between @StartDate and @EndDate
--order by bsinday
--admitdatetime desc, dischargedatetime desc
--) TS
--on HF.[PTFSSN]  = TS.realssn
--where HF.facility like @facility 
--and TS.REALSSN is null
