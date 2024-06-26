USE [master]
GO
/****** Object:  Database [RehabMetricsAndOutcomes]    Script Date: 4/16/2021 10:04:26 AM ******/
CREATE DATABASE [RehabMetricsAndOutcomes]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'RehabAssessmentCareTool', FILENAME = N'R:\SQLData\RehabAssessmentCareTool.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'RehabAssessmentCareTool_log', FILENAME = N'R:\SQLlogs\RehabAssessmentCareTool_log.ldf' , SIZE = 16896KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RehabMetricsAndOutcomes].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET ARITHABORT OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET  DISABLE_BROKER 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET RECOVERY FULL 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET  MULTI_USER 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'RehabMetricsAndOutcomes', N'ON'
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET QUERY_STORE = OFF
GO
USE [RehabMetricsAndOutcomes]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [RehabMetricsAndOutcomes]
GO
/****** Object:  User [VHAMASTER\VSSC Business Apps]    Script Date: 4/16/2021 10:04:27 AM ******/
CREATE USER [VHAMASTER\VSSC Business Apps] FOR LOGIN [VHAMASTER\VSSC Business Apps]
GO
/****** Object:  User [VHAMASTER\VA-NSOC-DB-Scan]    Script Date: 4/16/2021 10:04:27 AM ******/
CREATE USER [VHAMASTER\VA-NSOC-DB-Scan] FOR LOGIN [VHAMASTER\VA-NSOC-DB-Scan] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [AAC\vaoiscsocdbscan]    Script Date: 4/16/2021 10:04:27 AM ******/
CREATE USER [AAC\vaoiscsocdbscan] FOR LOGIN [AAC\vaoiscsocdbscan] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [AAC\SEC00E3APPD]    Script Date: 4/16/2021 10:04:27 AM ******/
CREATE USER [AAC\SEC00E3APPD] FOR LOGIN [AAC\SEC00E3APPD] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  DatabaseRole [VAOISCSOCDBSCAN_role]    Script Date: 4/16/2021 10:04:27 AM ******/
CREATE ROLE [VAOISCSOCDBSCAN_role]
GO
/****** Object:  DatabaseRole [vansocscan_role]    Script Date: 4/16/2021 10:04:27 AM ******/
CREATE ROLE [vansocscan_role]
GO
/****** Object:  DatabaseRole [vacsocitc_role]    Script Date: 4/16/2021 10:04:28 AM ******/
CREATE ROLE [vacsocitc_role]
GO
ALTER ROLE [db_owner] ADD MEMBER [VHAMASTER\VSSC Business Apps]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [VHAMASTER\VSSC Business Apps]
GO
ALTER ROLE [vansocscan_role] ADD MEMBER [VHAMASTER\VA-NSOC-DB-Scan]
GO
ALTER ROLE [vacsocitc_role] ADD MEMBER [AAC\vaoiscsocdbscan]
GO
/****** Object:  Schema [app]    Script Date: 4/16/2021 10:04:28 AM ******/
CREATE SCHEMA [app]
GO
/****** Object:  Table [app].[tblCodeSet]    Script Date: 4/16/2021 10:04:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblCodeSet](
	[CodeSetID] [int] IDENTITY(1,1) NOT NULL,
	[CodeSetParent] [int] NULL,
	[CodeValue] [varchar](30) NOT NULL,
	[CodeDescription] [varchar](400) NOT NULL,
	[HierarchyType] [int] NULL,
	[Active] [bit] NULL,
	[FyConstraint] [int] NULL,
	[SortOrder] [int] NULL,
	[Comment] [varchar](200) NULL,
 CONSTRAINT [PK_tblCodeSet] PRIMARY KEY CLUSTERED 
(
	[CodeSetID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [app].[vCodeSetHierarchy]    Script Date: 4/16/2021 10:04:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [app].[vCodeSetHierarchy]
as
select top 100 percent
hierarchy.CodeDescription 'Hierarchy',
child.[CodeSetID] 'CHILD ID', child.codeValue 'CHILD value', child.CodeDescription 'CHILD description',
parent.CodeSetID 'PARENT ID', parent.codeValue 'PARENT value', parent.CodeDescription 'PARENT description',
grand.CodeSetID 'GRAND ID', grand.CodeValue 'GRAND value', grand.CodeDescription 'GRAND description',
greatGrand.CodeSetID 'GREAT ID', greatGrand.CodeValue 'GREAT value', greatGrand.CodeDescription 'GREAT description',
ancestor.CodeSetID 'ANCESTOR ID', ancestor.CodeValue 'ANCESTOR value', ancestor.CodeDescription 'ANCESTOR description',
antiquity.CodeSetID 'ANTIQUITY ID', antiquity.CodeValue 'ANTIQUITY value', antiquity.CodeDescription 'ANTIQUITY description'
from app.tblCodeSet child
left join app.tblCodeSet parent
on child.CodeSetParent = parent.CodeSetID
left join app.tblCodeSet grand
on parent.CodeSetParent = grand.codesetId
left join app.tblCodeSet greatGrand
on grand.CodeSetParent = greatGrand.codesetId
left join app.tblCodeSet ancestor
on greatGrand.CodeSetParent = ancestor.CodeSetID
left join app.tblCodeSet antiquity
on ancestor.CodeSetParent = antiquity.CodeSetID
left join app.tblCodeSet hierarchy
on hierarchy.CodeSetID = child.hierarchytype
order by greatGrand.CodeValue,grand.CodeValue,parent.CodeValue, child.CodeValue
GO
/****** Object:  Table [app].[tblQuestion]    Script Date: 4/16/2021 10:04:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblQuestion](
	[QuestionID] [int] IDENTITY(1,1) NOT NULL,
	[QuestionKey] [varchar](20) NOT NULL,
	[Order] [int] NULL,
	[QuestionTitle] [varchar](200) NULL,
	[Question] [varchar](max) NOT NULL,
	[GroupTitle] [varchar](50) NULL,
	[FormFK] [int] NOT NULL,
	[FormSectionFK] [int] NULL,
	[AnswerCodeSetFK] [int] NOT NULL,
	[BranchingPoint] [bit] NULL,
	[MultiChoice] [bit] NULL,
 CONSTRAINT [PK_app.tblQuestion] PRIMARY KEY CLUSTERED 
(
	[QuestionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [app].[vQuestionStandardChoices]    Script Date: 4/16/2021 10:04:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [app].[vQuestionStandardChoices]
as
select *
from [app].[tblQuestion] a
inner join [app].[vCodeSetHierarchy] b
on a.AnswerCodeSetFK = b.[Parent id]
GO
/****** Object:  View [app].[vQuestionStandardChoices_Condensed]    Script Date: 4/16/2021 10:04:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [app].[vQuestionStandardChoices_Condensed]
as
select top 100 percent 
a.QuestionID, a.QuestionKey, a.Question, a.AnswerCodeSetFK 'Code Set', b.[CHILD description] 'Valid Choice', b.[CHILD value] 'Choice Code'
from [app].[tblQuestion] a
left join [app].[vCodeSetHierarchy] b
on a.AnswerCodeSetFK = b.[Parent id]
order by a.[order]
GO
/****** Object:  Table [app].[tblAnswer]    Script Date: 4/16/2021 10:04:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblAnswer](
	[EpsideOfCareIDFK] [int] NOT NULL,
	[QuestionIDFK] [int] NOT NULL,
	[AnswerCodeSetFK] [int] NOT NULL,
	[AnswerSequenceNumber] [int] NOT NULL,
	[Description] [varchar](max) NULL,
 CONSTRAINT [PK_tblAnswer] PRIMARY KEY CLUSTERED 
(
	[EpsideOfCareIDFK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[tblEpisodeOfCare]    Script Date: 4/16/2021 10:04:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblEpisodeOfCare](
	[EpisodeOfCareID] [int] IDENTITY(1,1) NOT NULL,
	[OnsetDate] [date] NOT NULL,
	[AdmissionDate] [date] NOT NULL,
	[PatientICNFK] [varchar](10) NOT NULL,
 CONSTRAINT [PK_app.tblEpisodeOfCare] PRIMARY KEY CLUSTERED 
(
	[EpisodeOfCareID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[tblPatient]    Script Date: 4/16/2021 10:04:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblPatient](
	[ICN] [varchar](10) NOT NULL,
	[IEN] [varchar](10) NOT NULL,
	[FirstName] [varchar](20) NOT NULL,
	[LastName] [varchar](20) NOT NULL,
	[MiddleName] [varchar](20) NULL,
	[DateOfBirth] [date] NOT NULL,
	[Last4SSN] [char](4) NOT NULL,
 CONSTRAINT [PK_tblPatient] PRIMARY KEY CLUSTERED 
(
	[ICN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [app].[tblQuestionInstruction]    Script Date: 4/16/2021 10:04:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblQuestionInstruction](
	[InstructionID] [int] IDENTITY(1,1) NOT NULL,
	[QuestionIDFK] [int] NOT NULL,
	[Order] [int] NULL,
	[Instruction] [varchar](max) NOT NULL,
 CONSTRAINT [PK_tblInstruction] PRIMARY KEY CLUSTERED 
(
	[InstructionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [app].[tblUser]    Script Date: 4/16/2021 10:04:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [app].[tblUser](
	[ID] [int] NOT NULL,
	[FirstName] [varchar](20) NOT NULL,
	[LastName] [varchar](20) NOT NULL,
 CONSTRAINT [PK_tblUser] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSignature]    Script Date: 4/16/2021 10:04:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSignature](
	[EpisodeCareIDFK] [int] NOT NULL,
	[Signature] [binary](50) NOT NULL,
	[Title] [varchar](100) NOT NULL,
	[DateInformationProvided] [date] NOT NULL,
	[Time] [time](7) NULL,
 CONSTRAINT [PK_tblSignature] PRIMARY KEY CLUSTERED 
(
	[EpisodeCareIDFK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_tblAnswer]    Script Date: 4/16/2021 10:04:28 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblAnswer] ON [app].[tblAnswer]
(
	[EpsideOfCareIDFK] ASC,
	[QuestionIDFK] ASC,
	[AnswerCodeSetFK] ASC,
	[AnswerSequenceNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tblAnswer_AnswerCodeSetFK]    Script Date: 4/16/2021 10:04:28 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblAnswer_AnswerCodeSetFK] ON [app].[tblAnswer]
(
	[AnswerCodeSetFK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tblAnswer_EpisodeOfCareIDFK]    Script Date: 4/16/2021 10:04:28 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblAnswer_EpisodeOfCareIDFK] ON [app].[tblAnswer]
(
	[EpsideOfCareIDFK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tblAnswer_QuestionIDFK]    Script Date: 4/16/2021 10:04:28 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblAnswer_QuestionIDFK] ON [app].[tblAnswer]
(
	[QuestionIDFK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_tblCodeSet]    Script Date: 4/16/2021 10:04:28 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblCodeSet] ON [app].[tblCodeSet]
(
	[CodeSetParent] ASC,
	[CodeValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_tblPatient_UniqueName]    Script Date: 4/16/2021 10:04:28 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblPatient_UniqueName] ON [app].[tblPatient]
(
	[LastName] ASC,
	[FirstName] ASC,
	[MiddleName] ASC,
	[Last4SSN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_tblQuestion_QuestionKey]    Script Date: 4/16/2021 10:04:28 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblQuestion_QuestionKey] ON [app].[tblQuestion]
(
	[QuestionKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_tblInstruction]    Script Date: 4/16/2021 10:04:28 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblInstruction] ON [app].[tblQuestionInstruction]
(
	[QuestionIDFK] ASC,
	[Order] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_tblSignature]    Script Date: 4/16/2021 10:04:28 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblSignature] ON [dbo].[tblSignature]
(
	[Signature] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [app].[tblAnswer]  WITH CHECK ADD  CONSTRAINT [FK_tblAnswer_tblCodeSet] FOREIGN KEY([AnswerCodeSetFK])
REFERENCES [app].[tblCodeSet] ([CodeSetID])
GO
ALTER TABLE [app].[tblAnswer] CHECK CONSTRAINT [FK_tblAnswer_tblCodeSet]
GO
ALTER TABLE [app].[tblAnswer]  WITH CHECK ADD  CONSTRAINT [FK_tblAnswer_tblEpisodeOfCare] FOREIGN KEY([EpsideOfCareIDFK])
REFERENCES [app].[tblEpisodeOfCare] ([EpisodeOfCareID])
GO
ALTER TABLE [app].[tblAnswer] CHECK CONSTRAINT [FK_tblAnswer_tblEpisodeOfCare]
GO
ALTER TABLE [app].[tblAnswer]  WITH CHECK ADD  CONSTRAINT [FK_tblAnswer_tblQuestion] FOREIGN KEY([QuestionIDFK])
REFERENCES [app].[tblQuestion] ([QuestionID])
GO
ALTER TABLE [app].[tblAnswer] CHECK CONSTRAINT [FK_tblAnswer_tblQuestion]
GO
ALTER TABLE [app].[tblCodeSet]  WITH CHECK ADD  CONSTRAINT [FK_tblCodeSet_tblCodeSet] FOREIGN KEY([CodeSetParent])
REFERENCES [app].[tblCodeSet] ([CodeSetID])
GO
ALTER TABLE [app].[tblCodeSet] CHECK CONSTRAINT [FK_tblCodeSet_tblCodeSet]
GO
ALTER TABLE [app].[tblEpisodeOfCare]  WITH CHECK ADD  CONSTRAINT [FK_app.tblEpisodeOfCare_app.tblPatient] FOREIGN KEY([PatientICNFK])
REFERENCES [app].[tblPatient] ([ICN])
GO
ALTER TABLE [app].[tblEpisodeOfCare] CHECK CONSTRAINT [FK_app.tblEpisodeOfCare_app.tblPatient]
GO
ALTER TABLE [app].[tblQuestion]  WITH CHECK ADD  CONSTRAINT [FK_app.tblQuestion_tblCodeSet] FOREIGN KEY([AnswerCodeSetFK])
REFERENCES [app].[tblCodeSet] ([CodeSetID])
GO
ALTER TABLE [app].[tblQuestion] CHECK CONSTRAINT [FK_app.tblQuestion_tblCodeSet]
GO
ALTER TABLE [app].[tblQuestion]  WITH CHECK ADD  CONSTRAINT [FK_tblQuestion_tblCodeSet_FomSection] FOREIGN KEY([FormSectionFK])
REFERENCES [app].[tblCodeSet] ([CodeSetID])
GO
ALTER TABLE [app].[tblQuestion] CHECK CONSTRAINT [FK_tblQuestion_tblCodeSet_FomSection]
GO
ALTER TABLE [app].[tblQuestion]  WITH CHECK ADD  CONSTRAINT [FK_tblQuestion_tblCodeSet_Form] FOREIGN KEY([FormFK])
REFERENCES [app].[tblCodeSet] ([CodeSetID])
GO
ALTER TABLE [app].[tblQuestion] CHECK CONSTRAINT [FK_tblQuestion_tblCodeSet_Form]
GO
ALTER TABLE [app].[tblQuestionInstruction]  WITH CHECK ADD  CONSTRAINT [FK_tblInstruction_tblQuestion] FOREIGN KEY([QuestionIDFK])
REFERENCES [app].[tblQuestion] ([QuestionID])
GO
ALTER TABLE [app].[tblQuestionInstruction] CHECK CONSTRAINT [FK_tblInstruction_tblQuestion]
GO
ALTER TABLE [dbo].[tblSignature]  WITH CHECK ADD  CONSTRAINT [FK_tblSignature_tblEpisodeOfCare] FOREIGN KEY([EpisodeCareIDFK])
REFERENCES [app].[tblEpisodeOfCare] ([EpisodeOfCareID])
GO
ALTER TABLE [dbo].[tblSignature] CHECK CONSTRAINT [FK_tblSignature_tblEpisodeOfCare]
GO
USE [master]
GO
ALTER DATABASE [RehabMetricsAndOutcomes] SET  READ_WRITE 
GO
