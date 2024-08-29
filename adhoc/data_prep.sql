SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






/****** Script for SelectTopNRows command from SSMS  ******/
/* 
prepare questionnaire for the new FY by inserting the current FY questionnaire 
then set [QuestionnaireFY] = @newFY, [EffectiveDate] = @effectiveDate
Last Update: 06/05/2023 added question for target facility, VISN, or both
*/

alter proc [admin].[spPrepareNewFyQuestionnaire](
@newFY int)
as
declare @newFY int = 2025
declare @priorFY int = @newFY-1 --datepart(yy, getdate())
declare @effectiveDate date = getdate()
declare @active bit = 0 /* only set 1 when ready for production */
declare @newFYQuestionnairCount int = 0
declare @newFYSectionCount int = 0
declare @newFYCodeSetCount int = 0
declare @errorMsg varchar(200)
declare @error int
declare @rowAffected int

/***** Check new FY is greater than current FY, exist if not greater ****/
if @newFY - @priorFY <> 1
begin
	set @errorMsg = 'New FY ' + cast(@newFY as varchar(4)) + ' is not greater than current FY ' + cast(@priorFY as varchar(4)) + ' by 1'
	raisError(@errorMsg, 0,1)
	return 
end
else
	print 'New FY ' + cast(@newFY as varchar(4)) + ' is later than current FY ' + cast(@priorFY as varchar(4)) + ' so proceed next'

/***** check existance new FY question settting, exist if exists *****/
select @newFYQuestionnairCount = count(*) 
from [PCC_FIT].[app].[tblQuestionnaire] 
where [QuestionnaireFY] = @newFY

if (@newFYQuestionnairCount <> 0)
begin
	set @errorMsg = 'New FY ' + cast(@newFY as varchar(4)) + ' already has ' + cast(@newFYQuestionnairCount as varchar(4)) + ' questions in [app].tblQuestionnaire'
	raisError(@errorMsg, 0,1)
	return
end
else
	print 'No question in [app].tblQuestionnaire for ' + cast(@newFY as varchar(4)) + ' yet so proceed next'

/***** check existance new FY sections setting, exist if exists *****/
select @newFYSectionCount = count(*)
from [app].tblSection
where fy = @newFY

if (@newFYSectionCount <> 0)
begin
	set @errorMsg = 'New FY section already exists in [app].tblSection'
	raisError(@errorMsg, 0,1)
	return
end
else
	print 'No section in [app].tblSection for ' + cast(@newFY as varchar(4)) + ' yet so proceed next'  

select @newFYCodeSetCount = count(*) from [admin].tblCodeSet where fy = @newFY

if @newFYCodeSetCount <> 0
BEGIN
	set @errorMsg = 'New FY code set already exists in [admin].tblCodeSet'
	raisError(@errorMsg, 0,1)
	return
END
ELSE
	print 'No code set code value for ' + cast(@newFY as varchar(4)) + ' in [admin].tblCodeSet yet so proceed next'


/* proceed transactional inserts */
begin tran;
	begin try
	/* new choices for Q39 starting FY2025, subsequent FY does not need to change the choice */
		if (@newFY = 2025)
		BEGIN
			/* insert new parent code value */
			insert admin.tblCodeSet
			select valuekey, [value], CodeSetIDRef, null, HierarchyCodeSetIDRef, @newFY 
			from admin.tblCodeSet where valuekey = 'ChoiceType12'
			
			if @@ERROR <> 0
			begin
				set @errorMsg = 'insert code set parent failed'
				RAISERROR(@errorMsg, 0, 1)
			end
			ELSE
				print 'new ChoiceType12 identiy ' + @@identity

			/* insert new Facility child code set code value */
			insert admin.tblCodeSet
			select ValueKey +'.Facility' as ValueKey, 
			case when ValueKey = 'L1' then 'Level 1 - Information sharing only - one way communication with no interactive communication (Usually via email, report sharing, and blanket announcements).'
				when ValueKey = 'L2' then 'Level 2 - Ad-hoc/Occasional and unscheduled interactive communication and/or FIT response to inquiries made by facility POCs related to Whole Health.'
				when ValueKey = 'L3' then 'Level 3 - Regular dialogue with the facility POCs that occurs at least monthly, may include time limited and specific consultation with a project.'
				when ValueKey = 'L4' then 'Level 4 - Active and regular FIT consultation in service of Whole Health system implementation occurring multiple times per month. Facility is designing and executing implementation plans to support integration of whole health concepts and tools and/or formal adoption of one or more elements of the Whole Health System.'
				when ValueKey = 'L5' then 'Level 5 - Ongoing and comprehensive FIT consultation occurring weekly on average requiring the Primary Consultant to communicate with and/or coordinate multiple activities and/or input from internal SMEs in in service of Whole Health System implementation. Facility is actively designing and executing implementation plans for two or more components of the Whole Health System and working on sustainment of whole health strategic priorities.'
			end as [Value],
			@@IDENTITY as CodeSetIDRef, 
			c.ValueOrder, c.HierarchyCodeSetIDRef, @newFY
			from admin.tblCodeSet c
			where CodeSetIDRef = @@IDENTITY and ValueKey not like '%VISN%'

			if @@ERROR <> 0
			begin
				set @errorMsg = 'insert code set Facility child for Level of FIT Engagement failed'
				RAISERROR(@errorMsg, 0, 1)
			end
			else
				print 'New Facility Levels are inserted'

			/* insert new VISN child code set code value */
			insert admin.tblCodeSet
			select ValueKey +'.VISN' as ValueKey, 
			case when ValueKey = 'L1' then 'Level 1 - Information sharing only - one way communication with no interactive communication (Usually via email, report sharing, and blanket announcements).'
				when ValueKey = 'L2' then 'evel 2 - Ad-hoc/Occasional and unscheduled interactive communication and/or FIT response to inquiries made by VISN Whole Health Coordinator related to Whole Health.'
				when ValueKey = 'L3' then 'Level 3 - Regular dialogue with the VISN Whole Health Coordinator and/or VISN Whole Health Team members that occurs at least monthly, may include time limited and specific consultation with plans for network-wide implementation efforts.'
				when ValueKey = 'L4' then 'Level 4 - Active and regular FIT consultation with the VISN Whole Health Coordinator and/or other VISN Whole Health Team members in service of Whole Health system implementation across the network occurring multiple times per month.  VISN is designing and executing plans to support implementation of the Whole Health System across multiple facilities within the VISN.'
				when ValueKey = 'L5' then 'Level 5 - Ongoing and comprehensive FIT consultation occurring weekly on average with the VISN Whole Health Coordinator and/or other VISN Whole Health Team members on average requiring the Primary Consultant to communicate with and/or coordinate multiple activities and/or input from internal SMEs in service of Whole Health System implementation across the network. VISN is designing and executing plans to support implementation of the Whole Health System across all facilities within the VISN.'
			end as [Value],
			(select top 1 id from admin.tblCodeSet where fy =@newFY and valuekey = 'Choicetype12') as CodeSetIDRef
			, c.ValueOrder, c.HierarchyCodeSetIDRef, @newFY
			from admin.tblCodeSet c
			where CodeSetIDRef =(select top 1 id from admin.tblCodeSet where fy <> @newFY and valuekey = 'Choicetype12') and ValueKey not like '%Facility%'
		
			if @@ERROR <> 0
			begin
				set @errorMsg = 'insert code set VISN child for Level of FIT Engagement failed'
				RAISERROR(@errorMsg, 0, 1)
			end
			ELSE
				print 'New Visn levels are inserted'
		end

		/***** copy current FY sections *****/
		insert app.tblSection( [FY], [Section], [SectionTitle], [Target])
		SELECT @newFY, [Section] ,[SectionTitle], [Target]
			FROM [PCC_FIT].[app].[tblSection]
			where fy = @priorFY

		if @@ERROR <> 0
		begin
			set @errorMsg = 'insert sections for question failed'
			RAISERROR(@errorMsg, 0, 1)
		end
		ELSE
			print 'New sections are inserted'

		/***** copy current FY questionnaire */
		insert [PCC_FIT].[app].[tblQuestionnaire](
				[QuestionKey]
				,[Question]
				,[QuestionnaireFY]
				,[Order]
				,[ChoiceTypeCodeSetIDRef]
				,[EffectiveDate]
				,[Active]
				,[UiStyleCodeSetIDRef]
				,[LeadQuestionHeader]
				,[MultiplicityCodeSetIdref]
				,[Instruction]
				,[Page]
				,[QuestionSet]
				,[Target])
		SELECT 
				[QuestionKey]
				,[Question]
				,[QuestionnaireFY] = @newFY
				,[Order]
				,[ChoiceTypeCodeSetIDRef] = (select top 1 id from admin.tblCodeSet where fy=@newFY and valuekey = 'ChoiceType12')
				,[EffectiveDate] = @effectiveDate
				,[Active] = @active
				,[UiStyleCodeSetIDRef] 
				,[LeadQuestionHeader]
				,[MultiplicityCodeSetIdref]
				,[Instruction]
				,[Page]
				,[QuestionSet]
				,[Target]
				-- starting FY 2024 facility and VISN sheet ask different questions
				-- ,[Target] = case 
				-- 	when @newFY >= 2024 and [QuestionKey] in ('Q3','Q3a','Q62','Q64','Q65','Q66','Q67') then 'Facility'
				-- 	when @newFY >= 2024 and [QuestionKey] in ('Q2','Q2b-Desc','Q2c-Desc','Q6','Q8','Q9','Q33','Q36','Q36a') then 'VISN'
				-- 	when @newFY >= 2024 and [QuestionKey] in ('Q9a','Q32','Q34','Q35','Q39','Q72','Q72-DT') then 'Both' 
				-- end

		FROM [PCC_FIT].[app].[tblQuestionnaire]
		where QuestionnaireFY = @priorFY
		order by [order]

		if @@ERROR <> 0
		begin
			set @errorMsg = 'insert ' + cast(@newFY as varchar) + ' questionnaire failed'
			RAISERROR(@errorMsg, 0, 1)
		end
		else
			print 'new questions are inserted'

		/* patch the FY in answers in case they are 0 */
		update [PCC_FIT].[app].[tblUserAnswer]
		set FY = priorFY.QuestionnaireFY
		--case 
		--when priorFY.QuestionnaireFY = 2023 then 2023
		--when priorFY.QuestionnaireFY = 2022 then 2022
		--when priorFY.QuestionnaireFY = 2021 then 2021
		--end 
		FROM [PCC_FIT].[app].[tblUserAnswer] a
		inner join [app].[tblQuestionnaire] priorFY
		on a.QuestionnaireQuestionIDRef = priorFY.QuestionID
		where a.fy = @priorFY

		if @@ERROR <> 0
		begin
			set @errorMsg = 'update FY' + cast(@priorFY as varchar) + ' answers.FY failed'
			RAISERROR(@errorMsg, 0, 1)
		end	
		ELSE
			print cast(@newFY as varchar) + 'answers are preped'
		/* 
		insert the previous FY answers as the new FY answers with 
		question ID of current FY questionnaire ID,
		and the questionKey in the filter range that might change since FY2024 , 
		last update date with today's date
		fy = current FY
		*/
		insert [PCC_FIT].[app].[tblUserAnswer]
		SELECT 
			--[UserAnswerID],
			[FacilityIDxRef]
			,[StaffFacilityRelationIDRef]
			,[QuestionnaireQuestionIDRef]= NewFY.QuestionID
			,[AnswerCodeSetIDRef]
			,[OtherDescription]
			,[lastUpdate] = getdate()
			,[AnswerSetId]
			,FY = @newFY
            --, priorFY.QuestionnaireFY, priorFY.Questionid, newFY.QuestionnaireFY, newFY.Questionid

		FROM [PCC_FIT].[app].[tblUserAnswer] a
		inner join [app].[tblQuestionnaire] priorFY
			on a.QuestionnaireQuestionIDRef = priorFY.QuestionID and priorFY.QuestionnaireFY = @newFY -1 /* get prior FY answers */
		inner join [app].[tblQuestionnaire] newFY 
			on priorFY.QuestionKey = newFY.QuestionKey and newFY.QuestionnaireFY = @newFY /* get new FY question ID */

		commit tran

	end try
	begin catch
		rollback
		print 'Transaction 2 roll back'
	end catch

GO
