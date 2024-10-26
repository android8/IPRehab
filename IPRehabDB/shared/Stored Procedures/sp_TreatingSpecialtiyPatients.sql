-- =============================================
-- Author:		Jonathan sun
-- Create date: 09/11/2022
-- Description:	target facility patient in treading speality
-- =============================================
CREATE PROCEDURE [shared].[sp_TreatingSpecialtiyPatients] 
	/*likeFacility should be %999%* pattern */
	@likeFacilityId varchar(6),
	@minusDays int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

declare @thisDate datetime2 = cast(getdate() - @minusDays as datetime2);

with CTE as (
select distinct
ts.bsta6a, --ltrim(substring(ts.bsta6a,1,3)) as sta3, 
ts.[bedsecn],ts.[admitday], ScrSSNT, ts.scrssn, ts.scrnum, RealSSN
from  vhaausbi13.DMTreatingSpecialty.[dbo].[FactTSAll_Day_CDW_2yrs] as ts
where bsta6a like @likeFacilityId + '%' and @thisDate <= [admitday] 
and [bedsecn] in (22,20,112,64) and C_LOS >= 2 --and [statyp] in (98,40)
) 

select * from CTE
inner join (
	select [Scrssn], [Last_Name],[First_Name],[PatientName],[PatientICN],[DoB]
	from VHAAUSBI13.DMTreatingSpecialty.[dbo].[patient_demographic]
) p
on CTE.scrssn= cast(p.scrssn as int)
order by CTE.bsta6a, p.[Last_Name] 
END

