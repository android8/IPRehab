

/*
List of patients from Treating Specialty cube and join to Patient Demographic in BI1.DWWorkload for
those in bedsection 22, 20, 112, 64 and Length of Stay (C_LOS) equal to or longer than 2 days
*/
CREATE view [shared].[vTreatingSpecialtyRecent3Yrs]
as
SELECT [Last_Name],[First_Name],[PatientName], [DoB], '' [PatientICN],
bsta6a, sta3, 
[bedsecn],[admitday], 
ScrSSNT, scrssn, scrnum, RealSSN, [admission]

from  vhaausbi13.DMTreatingSpecialty.[dbo].[vTreatingSpecialtyRecent3Yrs] ts

