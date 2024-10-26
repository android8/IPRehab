CREATE proc bi.spQuestionAnswerNameScores
@facility varchar(6)
as
select distinct * from [bi].[vQuestionAnswersMatchNames] p 
left join bi.vQuestionAnswerAllScores s
on 
p.FacilityID6 = s.facilityID6 and p.EpisodeOfCareID = s.EpisodeOfCareID and p.RealSSN = s.realssn and 
p.[measure name] = s.[measure name] and
p.QuestionKey = s.questionkey 
where p.FacilityID6 like @facility + '%'
order by p.realssn, p.PatientName,p.[measure name], p.[stage name], p.QuestionKey