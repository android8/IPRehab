declare @StartDate datetime ='2021-06-01'
declare @EndDate datetime ='2021-09-30'
declare @facility varchar(6) = '648%'
select [inpatientsid],[bsta6a],[bedsecn],[discharge],
[bsinday],[bsoutday],[disday],[admitdatetime],[dischargedatetime],[bsindatetime],[bsoutdatetime],[bsindaynew],[bsoutdaynew]
,inpatientsid,scrssnt, realssn,scrssn
from [dbo].[FactTSAll_Day_CDW_2yrs]
where (admission = 1 or transin = 1) 
and bedsecn in (20,112,64) 
and C_LOS>2 and statyp in (98,40)
and bsta6a like @facility
--and bsinday between dateadd(month, -4, @endDate)+2 and @EndDate
--and bsinday <= @EndDate

order by bsinday desc --admitdatetime desc, dischargedatetime desc

