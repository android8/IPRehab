/* connect to [VHAAusBI13.vha.med.va.gov].DMTreatingSpecialty */
--use DMTreatingSpecialty
--go

declare @StartDate datetime =getdate()-365
declare @EndDate datetime = getdate() 

select @StartDate 'Start Date', @EndDate 'End Date'
declare @facility varchar(6) = '%648%'
--declare @admitdatetime datetime ='2021-10-01'
--declare @dischargedatetime datetime ='2021-12-31'
select *
--select realssn, [inpatientsid],[bsta6a],[bedsecn],[discharge],[bsinday],[bsoutday],[disday],[admitdatetime],[dischargedatetime],[bsindatetime],[bsoutdatetime],[bsindaynew],[bsoutdaynew]
from VHAAusBI13.DMTreatingSpecialty.[dbo].[FactTSAll_Day_CDW_2yrs]
where (admission = 1 or transin = 1) 
and bedsecn in (22,20,112,64) 
and C_LOS>2 and statyp in (98,40)
and bsta6a like @facility
and bsinday between @StartDate and @EndDate
--and bsindatetime <> Admitdatetime
