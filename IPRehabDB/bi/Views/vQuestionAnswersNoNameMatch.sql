
/* data dump for facility 678, change the where clause for other facility*/
CREATE view [bi].[vQuestionAnswersNoNameMatch]
as
select distinct e.PatientICNFK --e.EpisodeOfCareID, , convert(varchar, a.LastUpdate, 101) lastUpdate
FROM app.tblEpisodeOfCare e inner join 
app.tblAnswer a on a.EpsideOfCareIDFK = e.EpisodeOfCareID inner join 
app.tblQuestion q on q.QuestionID = a.QuestionIDFK inner join 
app.tblCodeSet answerCodeset on a.AnswerCodeSetFK = answerCodeset.CodeSetID left join 
app.tblQuestionMeasure measure on q.QuestionID = measure.QuestionIDFK and a.MeasureIDFK = measure.Id inner join 
app.tblCodeSet stageCodeset on measure.StageFK = stageCodeset.CodeSetID left join 
app.tblcodeset measureCodeset on measure.MeasureCodeSetIDFK = measureCodeset.CodeSetID left join 
--[shared].[vTreatingSpecialtyRecent3Yrs] P
[vhaausbi13].[DMTreatingSpecialty].[dbo].[rptRehabDetails] P on 
    e.FacilityID6 = p.bsta6a and e.AdmissionDate = p.admitday
    and e.PatientICNFK = P.scrssnt --150
    -- or e.PatientICNFK = trim(cast(P.scrnum as varchar)) --307
    -- or e.PatientICNFK = trim(cast(p.RealSSN as varchar)) -- 307
    -- or e.PatientICNFK = p.PatientICN --307
   where p.PatientName is null /*231 pat, 16247 ans*/ and e.FacilityID6 not like '648%'
   --where p.PatientName is null /*162 pat, 16152 ans*/ and e.FacilityID6 not like '648%'
  --FacilityID6 like '678%' --and c.EpisodeOfCareID in 
  --(select EpsideOfCareIDFK from app.tblAnswer where QuestionIDFK = 526 )
  --and EpisodeOfCareID = 1265

--group by e.PatientICNFK--, convert(varchar, a.LastUpdate, 101)
--order by convert(varchar, a.LastUpdate, 101) desc, e.PatientICNFK --, EpisodeOfCareID, q.[Order] 
