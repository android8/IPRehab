CREATE view bi.vQuestionAnswerAllScores
as
select FacilityID6, EpisodeOfCareID, realssn, PatientName, [measure name], 
[stage name], QuestionKey,
--select EpisodeOfCareID, realssn, PatientName, questionkey, [stage name],[measure name], 
[question score] = case   
    when main.QuestionKey like 'GG0130%' and codeValue < 7 then codevalue
    when main.QuestionKey like 'GG0130%' and codeValue >= 7 then 1
    when main.QuestionKey like 'GG0170%' and main.QuestionKey = 'GG0170M' and codeValue < 7 then CodeValue
    when main.QuestionKey like 'GG0170%' and main.QuestionKey = 'GG0170M' and codeValue >=7 then 3 
    when main.QuestionKey like 'GG0170%' and (main.QuestionKey <> 'GG0170M' and main.questionkey <> 'GG0170Q' and main.QuestionKey <> 'GG0170R' and main.questionkey <> 'GG0170RR' and main.QuestionKey <> 'GG0170S' and main.questionkey <> 'GG0170SS') and codeValue < 7 then CodeValue
    when main.QuestionKey like 'GG0170%' and (main.QuestionKey <> 'GG0170M' and main.questionkey <> 'GG0170Q' and main.QuestionKey <> 'GG0170R' and main.questionkey <> 'GG0170RR' and main.QuestionKey <> 'GG0170S' and main.questionkey <> 'GG0170SS') and codeValue >= 7 then 1
    else null
end,
[aggregate] = case when questionkey like 'gg0130%' then 'Self Care score' when QuestionKey like 'gg0170%' then 'Mobility score' end,
[total] = sum(case   
    when QuestionKey like 'GG0130%' and codeValue < 7 then codevalue
    when QuestionKey like 'GG0130%' and codeValue >= 7 then 1
    when QuestionKey like 'GG0170%' and QuestionKey = 'GG0170M' and codeValue < 7 then CodeValue
    when QuestionKey like 'GG0170%' and QuestionKey = 'GG0170M' and codeValue >=7 then 3 
    when main.QuestionKey like 'GG0170%' and (main.QuestionKey <> 'GG0170M' and main.questionkey <> 'GG0170Q' and main.QuestionKey <> 'GG0170R' and main.questionkey <> 'GG0170RR' and main.QuestionKey <> 'GG0170S' and main.questionkey <> 'GG0170SS') and codeValue < 7 then CodeValue
    when main.QuestionKey like 'GG0170%' and (main.QuestionKey <> 'GG0170M' and main.questionkey <> 'GG0170Q' and main.QuestionKey <> 'GG0170R' and main.questionkey <> 'GG0170RR' and main.QuestionKey <> 'GG0170S' and main.questionkey <> 'GG0170SS') and codeValue >= 7 then 1
    else null
end) over (partition by realssn, [measure name], substring(QuestionKey,0,6))

from [bi].[vQuestionAnswersMatchNames] main
--where QuestionKey like 'q%'
--where questionkey like 'gg0130%' or QuestionKey like 'gg0170%'
--where QuestionKey like 'GG0130%' --or Questionkey like 'GG0170%'
group by FacilityID6, EpisodeOfCareID, realssn, PatientName, [measure name], [stage name], QuestionKey
,cube( 
    case   
        when main.QuestionKey like 'GG0130%' and codeValue < 7 then codevalue
        when main.QuestionKey like 'GG0130%' and codeValue >= 7 then 1
        when main.QuestionKey like 'GG0170%' and main.QuestionKey = 'GG0170M' and codeValue < 7 then CodeValue
        when main.QuestionKey like 'GG0170%' and main.QuestionKey = 'GG0170M' and codeValue >=7 then 3 
        when main.QuestionKey like 'GG0170%' and (main.QuestionKey <> 'GG0170M' and main.questionkey <> 'GG0170Q' and main.QuestionKey <> 'GG0170R' and main.questionkey <> 'GG0170RR' and main.QuestionKey <> 'GG0170S' and main.questionkey <> 'GG0170SS') and codeValue < 7 then CodeValue
        when main.QuestionKey like 'GG0170%' and (main.QuestionKey <> 'GG0170M' and main.questionkey <> 'GG0170Q' and main.QuestionKey <> 'GG0170R' and main.questionkey <> 'GG0170RR' and main.QuestionKey <> 'GG0170S' and main.questionkey <> 'GG0170SS') and codeValue >= 7 then 1
        else null
    end
)