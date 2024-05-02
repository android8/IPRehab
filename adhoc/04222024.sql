-- select bsta6a, sta3, scrssnT, scrssn, scrnum, realssn, * 
-- from [vhaausbi13].[DMTreatingSpecialty].[dbo].[rptRehabDetails]
-- where trim(realssn) in
-- (
select 
--'c', c.*, 'r', r.*
--[EpisodeOfCareID]
c.PatientICNFK --, c.FacilityID6--, 
from [app].[tblEpisodeOfCare] c
left join [vhaausbi13].[DMTreatingSpecialty].[dbo].[rptRehabDetails] r
--on c.FacilityID6 = r.sta3 and c.PatientICNFK = r.realssn /*32543 blank rows*/
on trim(c.FacilityID6) = trim(r.sta3) and 
c.PatientICNFK = r.scrssnt  /*2725 blank rows*/
--on c.PatientICNFK = cast(r.scrssn as varchar(9)) /*5137 unmatched */
where r.realssn is null /*2725 rows*/
group by PatientICNFK--, FacilityID6
order by c.PatientICNFK--,c.FacilityID6
--)
