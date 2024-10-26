CREATE function dbo.func_GetPatientDemographic()
returns table 
as
return
SELECT distinct p.scrssn, p.[Last_Name],p.[First_Name],p.[PatientName],  p.[PatientICN],p.[DoB]
from VHAAUSBI13.DMTreatingSpecialty.[dbo].[patient_demographic] p
