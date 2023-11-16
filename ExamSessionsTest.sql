/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [ExamSessionID]
      ,[CourseID]
      ,[ExamFormat]
      ,[ExamDate]
      ,[ShiftID]
      ,[RoomID]
      ,[StudentsEnrolled]
      ,[TeacherID]
      ,[StaffID]
      ,[IsPassed]
      ,[IsPaid]
  FROM [dbo].[ExamSessions]

DELETE FROM ExamSessions WHERE ExamSessionID = ''
SELECT * FROM [sys].[triggers] WHERE [name] = 'trg_StudentEnrollmentAfterInsert'
SELECT * FROM [sys].[triggers] WHERE [name] = 'trg_StudentEnrollmentAfterDelete'
DROP TRIGGER [trg_StudentEnrollmentAfterDelete]; 