USE [TutorConnect]
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (1, N'Student')
INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (2, N'Tutor')
INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (3, N'Manager')
INSERT [dbo].[Roles] ([RoleId], [RoleName]) VALUES (4, N'Admin')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'hungnono2511', 1, N'$2a$11$yKZ8AAd4czDpx/zCkoc/UOwYJUpnjNXV22DBfaqoaIk9BVDQOnYdC', N'hungnono2511@gmail.com', N'Ngô Văn Hưng', N'0607689171', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739801674/wjz3vdclbggat8cwfbcy.jpg', CAST(N'2003-02-17T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-17T21:14:37.1802489' AS DateTime2), 3, NULL, NULL)
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'hungnvse170516', 3, N'$2a$11$6apQE0hhTIFmiaedr9qdieOpeNxBgijghcHX1ErADkJi6z//JwDCm', N'hungnvse170516@fpt.edu.vn', N'Ngô Van Hung', N'0123456789', N'null avatar', CAST(N'2001-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-14T11:49:57.4300000' AS DateTime2), 3, NULL, NULL)
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'manager1', 3, N'$2a$11$yKZ8AAd4czDpx/zCkoc/UOwYJUpnjNXV22DBfaqoaIk9BVDQOnYdC', N'manager1@example.com', N'Manager One', N'0901234567', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739801674/wjz3vdclbggat8cwfbcy.jpg', CAST(N'1990-12-05T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-18T21:40:21.6700000' AS DateTime2), 3, NULL, NULL)
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'ngohung2511', 2, N'$2a$11$6apQE0hhTIFmiaedr9qdieOpeNxBgijghcHX1ErADkJi6z//JwDCm', N'hungpitit@gmail.com', N'Ngo Van Hung', N'0241621309', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739508361/kiar6ax8liaaseurtx7b.jpg', CAST(N'2003-02-14T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-14T11:46:05.2270173' AS DateTime2), 3, NULL, NULL)
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'ngohung26583', 4, N'$2a$11$6apQE0hhTIFmiaedr9qdieOpeNxBgijghcHX1ErADkJi6z//JwDCm', N'ngohung26583@gmail.com', N'Ngô Hung', N'0987654321', N'null avatar', CAST(N'2000-05-20T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-14T11:49:57.4300000' AS DateTime2), 3, NULL, NULL)
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'student1', 1, N'$2a$11$yKZ8AAd4czDpx/zCkoc/UOwYJUpnjNXV22DBfaqoaIk9BVDQOnYdC', N'student1@example.com', N'Student One', N'0123456789', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739801674/wjz3vdclbggat8cwfbcy.jpg', CAST(N'2000-05-15T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-18T21:40:21.6700000' AS DateTime2), 3, NULL, NULL)
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'student2', 1, N'$2a$11$yKZ8AAd4czDpx/zCkoc/UOwYJUpnjNXV22DBfaqoaIk9BVDQOnYdC', N'student2@example.com', N'Student Two', N'0123456790', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739801674/wjz3vdclbggat8cwfbcy.jpg', CAST(N'2001-06-20T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-18T21:40:21.6700000' AS DateTime2), 3, NULL, NULL)
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'student3', 1, N'$2a$11$yKZ8AAd4czDpx/zCkoc/UOwYJUpnjNXV22DBfaqoaIk9BVDQOnYdC', N'student3@example.com', N'Student Three', N'0123456788', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739801674/wjz3vdclbggat8cwfbcy.jpg', CAST(N'2002-03-25T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-18T21:40:21.6700000' AS DateTime2), 3, NULL, NULL)
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'student4', 1, N'$2a$11$yKZ8AAd4czDpx/zCkoc/UOwYJUpnjNXV22DBfaqoaIk9BVDQOnYdC', N'student4@example.com', N'Student Four', N'0123456777', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739801674/wjz3vdclbggat8cwfbcy.jpg', CAST(N'2003-01-10T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-18T21:40:21.6700000' AS DateTime2), 3, NULL, NULL)
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'tutor1', 2, N'$2a$11$yKZ8AAd4czDpx/zCkoc/UOwYJUpnjNXV22DBfaqoaIk9BVDQOnYdC', N'tutor1@example.com', N'Tutor One', N'0987654321', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739801674/wjz3vdclbggat8cwfbcy.jpg', CAST(N'1995-03-10T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-18T21:40:21.6700000' AS DateTime2), 3, NULL, NULL)
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'tutor2', 2, N'$2a$11$yKZ8AAd4czDpx/zCkoc/UOwYJUpnjNXV22DBfaqoaIk9BVDQOnYdC', N'tutor2@example.com', N'Tutor Two', N'0978654321', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739801674/wjz3vdclbggat8cwfbcy.jpg', CAST(N'1994-07-22T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-18T21:40:21.6700000' AS DateTime2), 3, NULL, NULL)
INSERT [dbo].[Users] ([UserName], [RoleId], [Password], [Email], [FullName], [PhoneNumber], [Avatar], [DOB], [CreatedDate], [Status], [VerificationCode], [VerificationCodeExpiry]) VALUES (N'tutor3', 2, N'$2a$11$yKZ8AAd4czDpx/zCkoc/UOwYJUpnjNXV22DBfaqoaIk9BVDQOnYdC', N'tutor3@example.com', N'Tutor Three', N'0968654321', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739801674/wjz3vdclbggat8cwfbcy.jpg', CAST(N'1993-05-30T00:00:00.0000000' AS DateTime2), CAST(N'2025-02-18T21:40:21.6700000' AS DateTime2), 3, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[FavoriteInstructors] ON 

INSERT [dbo].[FavoriteInstructors] ([FavoriteInstructorId], [UserName]) VALUES (2, N'hungnono2511')
INSERT [dbo].[FavoriteInstructors] ([FavoriteInstructorId], [UserName]) VALUES (1, N'ngohung2511')
SET IDENTITY_INSERT [dbo].[FavoriteInstructors] OFF
GO
SET IDENTITY_INSERT [dbo].[Languagues] ON 

INSERT [dbo].[Languagues] ([LanguageId], [LanguageName], [Description], [UserName]) VALUES (1, N'Math', N'Basic Mathematics', N'ngohung26583')
INSERT [dbo].[Languagues] ([LanguageId], [LanguageName], [Description], [UserName]) VALUES (2, N'Physics', N'Mechanics and Thermodynamics', N'ngohung26583')
INSERT [dbo].[Languagues] ([LanguageId], [LanguageName], [Description], [UserName]) VALUES (3, N'Chemistry', N'Organic Chemistry', N'ngohung26583')
INSERT [dbo].[Languagues] ([LanguageId], [LanguageName], [Description], [UserName]) VALUES (4, N'Biology', N'Cell Biology', N'tutor2')
INSERT [dbo].[Languagues] ([LanguageId], [LanguageName], [Description], [UserName]) VALUES (5, N'English', N'Grammar and Composition', N'ngohung26583')
INSERT [dbo].[Languagues] ([LanguageId], [LanguageName], [Description], [UserName]) VALUES (6, N'Computer Science', N'Programming Basics', N'ngohung26583')
INSERT [dbo].[Languagues] ([LanguageId], [LanguageName], [Description], [UserName]) VALUES (7, N'History', N'World History', N'ngohung26583')
INSERT [dbo].[Languagues] ([LanguageId], [LanguageName], [Description], [UserName]) VALUES (8, N'Geography', N'Physical Geography', N'ngohung26583')
INSERT [dbo].[Languagues] ([LanguageId], [LanguageName], [Description], [UserName]) VALUES (9, N'Economics', N'Micro and Macro Economics', N'ngohung26583')
INSERT [dbo].[Languagues] ([LanguageId], [LanguageName], [Description], [UserName]) VALUES (10, N'Psychology', N'Human Behavior', N'ngohung26583')
SET IDENTITY_INSERT [dbo].[Languagues] OFF
GO
SET IDENTITY_INSERT [dbo].[Lessons] ON 

INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (2, N'ngohung2511', 0, N'string1', N'string', 2, N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739804011/sggm4qxllagvxkbonq1n.jpg', N'string', CAST(N'2025-02-17T14:52:43.2640000' AS DateTime2), 0, N'string')
INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (3, N'ngohung2511', 0, N'string', N'string', 0, N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739804244/b0apq6w2oom7tddurrpu.jpg', N'string', CAST(N'2025-02-17T14:56:06.8630000' AS DateTime2), 0, N'string')
INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (5, N'tutor1', 1, N'Basic Algebra', N'Introduction to Algebraic Concepts', 60, N'https://example.com/algebra.jpg', N'Understand basic algebraic expressions', CAST(N'2025-02-18T21:45:34.2033333' AS DateTime2), 1, N'Algebra_Basics.pdf')
INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (6, N'tutor2', 1, N'Physics Mechanics', N'Fundamentals of Mechanics', 90, N'https://example.com/mechanics.jpg', N'Grasp Newton’s Laws of Motion', CAST(N'2025-02-18T21:45:34.2033333' AS DateTime2), 1, N'Physics_Mechanics.pdf')
INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (7, N'tutor3', 1, N'Computer Programming', N'Introduction to Python Programming', 120, N'https://example.com/python.jpg', N'Learn Python basics and syntax', CAST(N'2025-02-18T21:45:34.2033333' AS DateTime2), 1, N'Python_Intro.pdf')
INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (8, N'tutor1', 1, N'Basic Geometry', N'Understanding Shapes and Angles', 60, N'https://example.com/geometry.jpg', N'Recognize different geometric shapes', CAST(N'2025-02-18T21:45:34.2033333' AS DateTime2), 1, N'Geometry_Basics.pdf')
INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (9, N'tutor2', 1, N'Organic Chemistry', N'Study of Organic Compounds', 90, N'https://example.com/organic.jpg', N'Understand carbon-based compounds', CAST(N'2025-02-18T21:45:34.2033333' AS DateTime2), 1, N'Organic_Chemistry.pdf')
INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (10, N'tutor3', 1, N'History of World War II', N'Deep dive into WWII events', 120, N'https://example.com/ww2.jpg', N'Analyze causes and consequences of WWII', CAST(N'2025-02-18T21:45:34.2033333' AS DateTime2), 1, N'WW2_History.pdf')
INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (11, N'tutor1', 1, N'Basic Economics', N'Understanding Supply and Demand', 60, N'https://example.com/economics.jpg', N'Explain basic economic principles', CAST(N'2025-02-18T21:45:34.2033333' AS DateTime2), 1, N'Economics_Basics.pdf')
INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (12, N'tutor2', 1, N'Human Biology', N'Introduction to Human Anatomy', 90, N'https://example.com/biology.jpg', N'Identify human body systems', CAST(N'2025-02-18T21:45:34.2033333' AS DateTime2), 1, N'Human_Biology.pdf')
INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (13, N'tutor3', 1, N'Artificial Intelligence', N'Machine Learning Basics', 120, N'https://example.com/ai.jpg', N'Understand AI algorithms', CAST(N'2025-02-18T21:45:34.2033333' AS DateTime2), 1, N'AI_Basics.pdf')
INSERT [dbo].[Lessons] ([LessonId], [Instructor], [Level], [Title], [Description], [Duration], [ImageUrl], [LearningObjectives], [CreateAt], [Status], [Material]) VALUES (14, N'tutor1', 1, N'Public Speaking', N'Building Confidence in Speaking', 60, N'https://example.com/speaking.jpg', N'Improve public speaking skills', CAST(N'2025-02-18T21:45:34.2033333' AS DateTime2), 1, N'Public_Speaking.pdf')
SET IDENTITY_INSERT [dbo].[Lessons] OFF
GO
SET IDENTITY_INSERT [dbo].[RefreshTokens] ON 

INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (1, N'13151650-f673-47e4-8797-31e180558f5b', N'ngohung26583', CAST(N'2025-02-23T22:41:51.9444416' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (2, N'12cf780a-83de-4174-aeee-1b4cc8bfa649', N'ngohung26583', CAST(N'2025-02-23T22:51:24.0512712' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (3, N'8f1b3d4d-bf69-430c-a18c-e2b072961992', N'ngohung2511', CAST(N'2025-02-24T09:39:09.8967434' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (4, N'094c4927-8078-4bbe-b1a9-75281cccf3ec', N'ngohung2511', CAST(N'2025-02-24T16:11:43.0879752' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (5, N'3cf14323-2ae7-4cbc-9801-6938d37b4892', N'ngohung2511', CAST(N'2025-02-24T16:20:22.9068164' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (6, N'ce5f19ad-948d-4f7c-be64-ef12377c2d41', N'ngohung2511', CAST(N'2025-02-24T16:24:50.5586894' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (7, N'5a0d7f48-b5da-4e76-b02e-99aeb6563f6e', N'hungnono2511', CAST(N'2025-02-24T21:19:40.1414434' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (8, N'5abd5175-a65f-48e0-89ec-2b0918f5027d', N'hungnono2511', CAST(N'2025-02-24T21:41:32.7760128' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (9, N'd318f77d-96d6-404c-b9cd-427f9b1d3b6e', N'ngohung2511', CAST(N'2025-02-24T21:47:06.0038490' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (10, N'658f6cd4-d5fa-40e7-86aa-f7f8e45f1486', N'ngohung2511', CAST(N'2025-02-24T22:09:19.8640573' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (11, N'634745a8-556b-49a8-8db4-f5b54b5cf6fa', N'ngohung2511', CAST(N'2025-02-24T22:15:14.6799126' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (12, N'f85bb5cc-eb2d-4597-aaec-fbbaefa64ea4', N'ngohung2511', CAST(N'2025-02-24T22:20:32.4624476' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (13, N'8bfb5708-c984-4859-a75e-cda17e97398e', N'hungnono2511', CAST(N'2025-02-25T22:34:12.6328989' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (14, N'025c9599-5164-49f7-a6ca-99b6b038f339', N'ngohung26583', CAST(N'2025-02-26T08:52:47.7183692' AS DateTime2))
INSERT [dbo].[RefreshTokens] ([RId], [Token], [UserName], [Expires]) VALUES (15, N'adadcb6d-61c9-47d1-910a-6eacafa1c5a0', N'ngohung2511', CAST(N'2025-02-26T11:11:39.7844463' AS DateTime2))
SET IDENTITY_INSERT [dbo].[RefreshTokens] OFF
GO
SET IDENTITY_INSERT [dbo].[TutorAvailabilities] ON 

INSERT [dbo].[TutorAvailabilities] ([TutorAvailabilityId], [Instructor], [DayOfWeek], [StartTime], [EndTime], [Status]) VALUES (2, N'tutor1', 2, CAST(N'1900-01-01T08:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T10:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[TutorAvailabilities] ([TutorAvailabilityId], [Instructor], [DayOfWeek], [StartTime], [EndTime], [Status]) VALUES (3, N'tutor1', 4, CAST(N'1900-01-01T14:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T16:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[TutorAvailabilities] ([TutorAvailabilityId], [Instructor], [DayOfWeek], [StartTime], [EndTime], [Status]) VALUES (4, N'tutor2', 3, CAST(N'1900-01-01T09:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T11:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[TutorAvailabilities] ([TutorAvailabilityId], [Instructor], [DayOfWeek], [StartTime], [EndTime], [Status]) VALUES (5, N'tutor2', 6, CAST(N'1900-01-01T15:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T17:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[TutorAvailabilities] ([TutorAvailabilityId], [Instructor], [DayOfWeek], [StartTime], [EndTime], [Status]) VALUES (6, N'tutor3', 7, CAST(N'1900-01-01T10:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T12:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[TutorAvailabilities] ([TutorAvailabilityId], [Instructor], [DayOfWeek], [StartTime], [EndTime], [Status]) VALUES (7, N'tutor3', 7, CAST(N'1900-01-01T16:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T18:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[TutorAvailabilities] ([TutorAvailabilityId], [Instructor], [DayOfWeek], [StartTime], [EndTime], [Status]) VALUES (8, N'tutor1', 8, CAST(N'1900-01-01T09:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T11:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[TutorAvailabilities] ([TutorAvailabilityId], [Instructor], [DayOfWeek], [StartTime], [EndTime], [Status]) VALUES (9, N'tutor2', 8, CAST(N'1900-01-01T12:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T14:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[TutorAvailabilities] ([TutorAvailabilityId], [Instructor], [DayOfWeek], [StartTime], [EndTime], [Status]) VALUES (10, N'tutor3', 8, CAST(N'1900-01-01T15:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T17:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[TutorAvailabilities] ([TutorAvailabilityId], [Instructor], [DayOfWeek], [StartTime], [EndTime], [Status]) VALUES (11, N'tutor1', 7, CAST(N'1900-01-01T18:00:00.0000000' AS DateTime2), CAST(N'1900-01-01T20:00:00.0000000' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[TutorAvailabilities] OFF
GO
SET IDENTITY_INSERT [dbo].[upgradeRequests] ON 

INSERT [dbo].[upgradeRequests] ([Id], [UserName], [DocumentUrl], [Status], [RequestedAt]) VALUES (1, N'ngohung2511', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739511701/ytgaaghc3jnlbyzprkhc.jpg', 1, CAST(N'2025-02-14T12:41:44.4433127' AS DateTime2))
INSERT [dbo].[upgradeRequests] ([Id], [UserName], [DocumentUrl], [Status], [RequestedAt]) VALUES (2, N'hungnono2511', N'https://res.cloudinary.com/dpuim19bu/image/upload/v1739802050/zafhucm0gavfkjubnpjp.jpg', 0, CAST(N'2025-02-17T21:20:52.1533951' AS DateTime2))
SET IDENTITY_INSERT [dbo].[upgradeRequests] OFF
GO
SET IDENTITY_INSERT [dbo].[Wallet] ON 

INSERT [dbo].[Wallet] ([WalletId], [Balance], [TransactionTime], [UserName]) VALUES (1, 0, CAST(N'2025-02-14T11:46:05.4264205' AS DateTime2), N'ngohung2511')
INSERT [dbo].[Wallet] ([WalletId], [Balance], [TransactionTime], [UserName]) VALUES (2, 0, CAST(N'2025-02-17T21:14:37.3100056' AS DateTime2), N'hungnono2511')
INSERT [dbo].[Wallet] ([WalletId], [Balance], [TransactionTime], [UserName]) VALUES (3, 100, CAST(N'2025-02-18T21:40:45.2933333' AS DateTime2), N'student1')
INSERT [dbo].[Wallet] ([WalletId], [Balance], [TransactionTime], [UserName]) VALUES (4, 150, CAST(N'2025-02-18T21:40:45.2933333' AS DateTime2), N'student2')
INSERT [dbo].[Wallet] ([WalletId], [Balance], [TransactionTime], [UserName]) VALUES (5, 120, CAST(N'2025-02-18T21:40:45.2933333' AS DateTime2), N'student3')
INSERT [dbo].[Wallet] ([WalletId], [Balance], [TransactionTime], [UserName]) VALUES (6, 130, CAST(N'2025-02-18T21:40:45.2933333' AS DateTime2), N'student4')
INSERT [dbo].[Wallet] ([WalletId], [Balance], [TransactionTime], [UserName]) VALUES (7, 200, CAST(N'2025-02-18T21:40:45.2933333' AS DateTime2), N'tutor1')
INSERT [dbo].[Wallet] ([WalletId], [Balance], [TransactionTime], [UserName]) VALUES (8, 250, CAST(N'2025-02-18T21:40:45.2933333' AS DateTime2), N'tutor2')
INSERT [dbo].[Wallet] ([WalletId], [Balance], [TransactionTime], [UserName]) VALUES (9, 300, CAST(N'2025-02-18T21:40:45.2933333' AS DateTime2), N'tutor3')
INSERT [dbo].[Wallet] ([WalletId], [Balance], [TransactionTime], [UserName]) VALUES (10, 350, CAST(N'2025-02-18T21:40:45.2933333' AS DateTime2), N'manager1')
SET IDENTITY_INSERT [dbo].[Wallet] OFF
GO
INSERT [dbo].[FavoriteInstructorDetails] ([FavoriteInstructorId], [tutor], [Status]) VALUES (2, N'ngohung2511', 0)
INSERT [dbo].[FavoriteInstructorDetails] ([FavoriteInstructorId], [tutor], [Status]) VALUES (2, N'tutor1', 0)
GO
SET IDENTITY_INSERT [dbo].[Profile] ON 

INSERT [dbo].[Profile] ([ProfileId], [LanguageId], [UserName], [Address], [Price], [Country], [TeachingExperience], [Education], [TutorStatus]) VALUES (1, 1, N'ngohung2511', N'Ha Tinh', CAST(50.00 AS Decimal(18, 2)), N'', N'stringstringstringstringstringstringstringstringst', N'stringstringstringstringstringstringstringstringst', 0)
INSERT [dbo].[Profile] ([ProfileId], [LanguageId], [UserName], [Address], [Price], [Country], [TeachingExperience], [Education], [TutorStatus]) VALUES (2, 2, N'hungnono2511', N'Ha Tinh', CAST(50.00 AS Decimal(18, 2)), N'Viet Nam', N'stringstringstringstringstringstringstringstringst', N'stringstringstringstringstringstringstringstringst', 0)
INSERT [dbo].[Profile] ([ProfileId], [LanguageId], [UserName], [Address], [Price], [Country], [TeachingExperience], [Education], [TutorStatus]) VALUES (11, 3, N'tutor1', N'123 Tutor St', CAST(50.00 AS Decimal(18, 2)), N'USA', N'5 years', N'MSc in Math', 0)
INSERT [dbo].[Profile] ([ProfileId], [LanguageId], [UserName], [Address], [Price], [Country], [TeachingExperience], [Education], [TutorStatus]) VALUES (12, 4, N'tutor3', N'789 Tutor Rd', CAST(60.00 AS Decimal(18, 2)), N'UK', N'7 years', N'MSc in Computer Science', 0)
INSERT [dbo].[Profile] ([ProfileId], [LanguageId], [UserName], [Address], [Price], [Country], [TeachingExperience], [Education], [TutorStatus]) VALUES (13, 5, N'tutor2', N'666 Tutor Ln', CAST(55.00 AS Decimal(18, 2)), N'Canada', N'6 years', N'MSc in Chemistry', 0)
SET IDENTITY_INSERT [dbo].[Profile] OFF
GO
SET IDENTITY_INSERT [dbo].[Bookings] ON 

INSERT [dbo].[Bookings] ([BookingId], [customer], [LessonId], [AvailabilityId], [Price], [Status], [Note], [Created]) VALUES (3, N'student1', 5, 2, 50, 1, N'First lesson', CAST(N'2025-02-18T21:50:30.6366667' AS DateTime2))
INSERT [dbo].[Bookings] ([BookingId], [customer], [LessonId], [AvailabilityId], [Price], [Status], [Note], [Created]) VALUES (4, N'student2', 10, 3, 40, 1, N'Intro session', CAST(N'2025-02-18T21:50:30.6366667' AS DateTime2))
INSERT [dbo].[Bookings] ([BookingId], [customer], [LessonId], [AvailabilityId], [Price], [Status], [Note], [Created]) VALUES (5, N'student3', 6, 4, 30, 1, N'Advanced session', CAST(N'2025-02-18T21:50:30.6366667' AS DateTime2))
INSERT [dbo].[Bookings] ([BookingId], [customer], [LessonId], [AvailabilityId], [Price], [Status], [Note], [Created]) VALUES (6, N'student4', 11, 5, 35, 1, N'Exam preparation', CAST(N'2025-02-18T21:50:30.6366667' AS DateTime2))
INSERT [dbo].[Bookings] ([BookingId], [customer], [LessonId], [AvailabilityId], [Price], [Status], [Note], [Created]) VALUES (7, N'student1', 7, 6, 45, 1, N'General study', CAST(N'2025-02-18T21:50:30.6366667' AS DateTime2))
INSERT [dbo].[Bookings] ([BookingId], [customer], [LessonId], [AvailabilityId], [Price], [Status], [Note], [Created]) VALUES (8, N'student2', 12, 7, 55, 1, N'Homework help', CAST(N'2025-02-18T21:50:30.6366667' AS DateTime2))
INSERT [dbo].[Bookings] ([BookingId], [customer], [LessonId], [AvailabilityId], [Price], [Status], [Note], [Created]) VALUES (9, N'student3', 8, 8, 60, 1, N'Assignment review', CAST(N'2025-02-18T21:50:30.6366667' AS DateTime2))
INSERT [dbo].[Bookings] ([BookingId], [customer], [LessonId], [AvailabilityId], [Price], [Status], [Note], [Created]) VALUES (10, N'student4', 9, 9, 70, 1, N'Research guidance', CAST(N'2025-02-18T21:50:30.6366667' AS DateTime2))
INSERT [dbo].[Bookings] ([BookingId], [customer], [LessonId], [AvailabilityId], [Price], [Status], [Note], [Created]) VALUES (11, N'student1', 10, 10, 65, 1, N'Essay writing', CAST(N'2025-02-18T21:50:30.6366667' AS DateTime2))
INSERT [dbo].[Bookings] ([BookingId], [customer], [LessonId], [AvailabilityId], [Price], [Status], [Note], [Created]) VALUES (12, N'student2', 14, 11, 75, 1, N'Presentation skills', CAST(N'2025-02-18T21:50:30.6366667' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Bookings] OFF
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250214044130_14_02', N'8.0.12')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250214154053_14_02_toi', N'8.0.12')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250216151215_16_02', N'8.0.12')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250217031304_17_02', N'8.0.12')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250218052118_18_02', N'8.0.12')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250218083213_18_02_ver2', N'8.0.12')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250218154443_updatesss', N'8.0.12')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250218181258_update', N'8.0.12')
GO
