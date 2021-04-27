--INSERT APP.tblQuestion
SELECT REPLACE(QUESTIONKEY,'ADMISSION','DISCHARGE'), NULL, QuestionTitle, QUESTION, NULL, 147, 206, 372, NULL, NULL
FROM   app.tblQuestion
WHERE (QuestionKey LIKE 'o%')

--UPDATE APP.tblQuestion
--SET QuestionKey = QuestionKey + '-ADMISSION'
--select * from app.tblQuestion
WHERE (QuestionKey LIKE 'O%')

--update app.tblQuestion
--set GroupTitle = '3. Discharge Performance'
SELECT *
FROM   app.tblQuestion
WHERE (QuestionKey IN ('J0510-ADMISSION', 'J0520-ADMISSION', 'J0530-ADMISSION'))
ORDER BY FormFK, QuestionKey