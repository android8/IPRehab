declare @startdate datetime 
declare @enddate datetime
set @startdate = '06/01/2021'
set @enddate = '07/13/2021'
select a.*, 'w->', w.*
from vhaausbi13.DMTreatingSpecialty.dbo.factfy21daycdw a
left join vhaausbi1.dwworkload.dbo.patient_demographic w
on a.scrssn = w.[ScrSSN]
where (a.admission = 1 or a.transin = 1)  
and a.bedsecn in (20,112,64) 
and a.C_LOS>2 and a.statyp in (98,40,30)
and a.bsinday between @StartDate and @EndDate

select  bedsecn
from  vhaausbi13.DMTreatingSpecialty.dbo.factfy21daycdw 
group by bedsecn
