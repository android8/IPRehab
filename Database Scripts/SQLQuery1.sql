/****** Script for SelectTopNRows command from SSMS  ******/
SELECT a.*
  FROM [RehabMetricsAndOutcomes].[app].[vQuestionStandardChoices_Condensed] a
  where [Code Set] in (118)