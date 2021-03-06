USE [TestAutomation]
GO
/****** Object:  User [DOMAIN\user]    Script Date: 3/6/2018 2:14:46 PM ******/
CREATE USER [DOMAIN\user] FOR LOGIN [DOMAIN\user] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [DOMAIN\user]    Script Date: 3/6/2018 2:14:46 PM ******/
CREATE USER [DOMAIN\user] FOR LOGIN [DOMAIN\user] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [DOMAIN\user]    Script Date: 3/6/2018 2:14:46 PM ******/
CREATE USER [DOMAIN\user] FOR LOGIN [DOMAIN\user] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [old]    Script Date: 3/6/2018 2:14:46 PM ******/
CREATE USER [old] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[Data]
GO
/****** Object:  User [testuser]    Script Date: 3/6/2018 2:14:46 PM ******/
CREATE USER [testuser] FOR LOGIN [testuser] WITH DEFAULT_SCHEMA=[Data]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DOMAIN\user]
GO
ALTER ROLE [db_datareader] ADD MEMBER [DOMAIN\user]
GO
ALTER ROLE [db_owner] ADD MEMBER [DOMAIN\user]
GO
/****** Object:  Schema [Active]    Script Date: 3/6/2018 2:14:46 PM ******/
CREATE SCHEMA [Active]
GO
/****** Object:  Schema [Data]    Script Date: 3/6/2018 2:14:46 PM ******/
CREATE SCHEMA [Data]
GO
/****** Object:  Schema [Definition]    Script Date: 3/6/2018 2:14:46 PM ******/
CREATE SCHEMA [Definition]
GO
/****** Object:  Table [Active].[Log]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Active].[Log](
	[LogID] [int] IDENTITY(1,1) NOT NULL,
	[LogEntry] [varchar](256) NOT NULL,
	[LogDetails] [varchar](1024) NULL,
	[LogLevel] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Active].[TestJob]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Active].[TestJob](
	[TestJobID] [int] IDENTITY(1,1) NOT NULL,
	[TestSuiteID] [int] NOT NULL,
	[TestPackageID] [int] NOT NULL,
	[TestPackageVersionID] [int] NOT NULL,
	[ConfigurationID] [int] NOT NULL,
	[TestPriority] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[VMInstanceID] [int] NULL,
	[TestJobState] [int] NULL,
	[Version] [varchar](32) NULL,
	[LicenseKey] [varchar](32) NULL,
	[DownloadLink] [varchar](32) NULL,
	[TestSuiteDirectory] [varchar](256) NULL,
	[StartTime] [datetime] NULL,
	[StopOnError] [bit] NULL,
	[RunCount] [int] NULL,
	[TimeoutMinutes] [int] NULL,
 CONSTRAINT [PK_TestJob] PRIMARY KEY CLUSTERED 
(
	[TestJobID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Active].[TestJobCommand]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Active].[TestJobCommand](
	[TestJobCommandID] [int] IDENTITY(1,1) NOT NULL,
	[TestJobID] [int] NOT NULL,
	[TestSuiteID] [int] NOT NULL,
	[TestPackageCommandID] [int] NOT NULL,
	[ExecutionOrder] [int] NOT NULL,
	[VMInstanceID] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[PassedCount] [int] NULL,
	[WarningCount] [int] NULL,
	[FailedCount] [int] NULL,
	[ResultCode] [int] NULL,
	[ResultString] [varchar](1024) NULL,
	[OutputDirectory] [varchar](256) NULL,
	[SnapshotName] [varchar](256) NULL,
	[DumpFilesGenerated] [bit] NULL,
	[Version] [varchar](32) NULL,
	[DownloadLink] [varchar](32) NULL,
	[SkippedCount] [int] NULL,
 CONSTRAINT [PK_TestJobCommand] PRIMARY KEY CLUSTERED 
(
	[TestJobCommandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Active].[TestJobPlan]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Active].[TestJobPlan](
	[TestJobPlanID] [int] IDENTITY(1,1) NOT NULL,
	[TestSuitePlanID] [int] NOT NULL,
	[TestPackageID] [int] NOT NULL,
	[ConfigurationID] [int] NOT NULL,
	[TestPriority] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[TestJobState] [int] NULL,
	[Version] [varchar](32) NULL,
	[LicenseKey] [varchar](32) NULL,
	[DownloadLink] [varchar](32) NULL,
	[StartTime] [datetime] NULL,
 CONSTRAINT [PK_TestJobPlan] PRIMARY KEY CLUSTERED 
(
	[TestJobPlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Active].[TestSuite]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Active].[TestSuite](
	[TestSuiteID] [int] IDENTITY(1,1) NOT NULL,
	[TestSuiteName] [varchar](256) NOT NULL,
	[TestSuiteDescription] [varchar](1024) NULL,
	[TestPackageID] [int] NOT NULL,
	[TestPackageVersionID] [int] NOT NULL,
	[TestPriority] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[AddedByUser] [varchar](64) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[MaxRunning] [int] NULL,
	[TestSuiteState] [int] NULL,
	[ServerGroupID] [int] NULL,
	[StartTime] [datetime] NULL,
	[TestSuitePlanID] [int] NULL,
	[AutomationLabID] [int] NULL,
	[OperatingSystemID] [int] NULL,
 CONSTRAINT [PK_TestSuite] PRIMARY KEY CLUSTERED 
(
	[TestSuiteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Active].[TestSuitePlan]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Active].[TestSuitePlan](
	[TestSuitePlanID] [int] IDENTITY(1,1) NOT NULL,
	[TestSuitePlanName] [varchar](256) NOT NULL,
	[TestSuitePlanDescription] [varchar](1024) NULL,
	[TestPackageID] [int] NOT NULL,
	[TestPriority] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[AddedByUser] [varchar](64) NOT NULL,
	[MaxRunning] [int] NULL,
	[TestSuiteState] [int] NULL,
	[ServerGroupID] [int] NULL,
	[OperatingSystemID] [int] NULL,
 CONSTRAINT [PK_TestSuitePlan] PRIMARY KEY CLUSTERED 
(
	[TestSuitePlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Active].[TestSuitePlanStateLkp]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Active].[TestSuitePlanStateLkp](
	[StateID] [int] NOT NULL,
	[StateName] [varchar](32) NOT NULL,
	[StateDescription] [varchar](256) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [Active].[TestSuiteSchedule]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Active].[TestSuiteSchedule](
	[TestSuiteScheduleID] [int] IDENTITY(1,1) NOT NULL,
	[TestSuitePlanID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [Active].[TestSuiteScheduleEntry]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Active].[TestSuiteScheduleEntry](
	[TestSuiteScheduleID] [int] NOT NULL,
	[WeekDayID] [int] NOT NULL,
	[StartingHour] [int] NOT NULL,
	[StartingMinute] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[AutomationLabLkp]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[AutomationLabLkp](
	[AutomationLabID] [int] IDENTITY(1,1) NOT NULL,
	[AutomationLabName] [varchar](32) NOT NULL,
	[AutomationLabDescription] [varchar](256) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_AutomationLabLkp] PRIMARY KEY CLUSTERED 
(
	[AutomationLabID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[AutomationLabMembership]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[AutomationLabMembership](
	[VMInstanceID] [int] NOT NULL,
	[AutomationLabID] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [UC_AutomationLabMembership] UNIQUE NONCLUSTERED 
(
	[VMInstanceID] ASC,
	[AutomationLabID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[Configuration]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[Configuration](
	[ConfigurationID] [int] IDENTITY(1,1) NOT NULL,
	[ConfigurationName] [varchar](64) NOT NULL,
	[ConfigurationDescription] [varchar](1024) NOT NULL,
	[WindowsVersion] [int] NOT NULL,
	[Is64Bit] [bit] NOT NULL,
	[IsServer] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[OperatingSystemID] [int] NULL,
 CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED 
(
	[ConfigurationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[DownloadLink]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[DownloadLink](
	[DownloadLinkID] [int] IDENTITY(1,1) NOT NULL,
	[DownloadLink] [varchar](32) NOT NULL,
	[DownloadLinkDescription] [varchar](256) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[OperatingSystemID] [int] NULL,
 CONSTRAINT [PK_DownloadLink] PRIMARY KEY CLUSTERED 
(
	[DownloadLinkID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[LabIDLkp]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[LabIDLkp](
	[LabID] [int] NOT NULL,
	[LabName] [varchar](64) NOT NULL,
 CONSTRAINT [PK_LabIDLkp] PRIMARY KEY CLUSTERED 
(
	[LabID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[LicenseKey]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[LicenseKey](
	[LicenseKeyID] [int] IDENTITY(1,1) NOT NULL,
	[LicenseKey] [varchar](32) NOT NULL,
	[LicenseKeyName] [varchar](256) NOT NULL,
	[LicenseKeyDescription] [varchar](1024) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_LicenseKey] PRIMARY KEY CLUSTERED 
(
	[LicenseKeyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[OperatingSystemLkp]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[OperatingSystemLkp](
	[OperatingSystemID] [int] IDENTITY(1,1) NOT NULL,
	[OperatingSystemName] [varchar](32) NOT NULL,
	[OperatingSystemDescription] [varchar](256) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_OperatingSystemLkp] PRIMARY KEY CLUSTERED 
(
	[OperatingSystemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[PeriodicReport]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[PeriodicReport](
	[PeriodicReportID] [int] IDENTITY(1,1) NOT NULL,
	[ScriptPath] [varchar](256) NOT NULL,
	[Recipients] [varchar](1024) NOT NULL,
	[EmailHeader] [varchar](256) NOT NULL,
	[EmailBody] [varchar](1024) NOT NULL,
	[ScheduleDay] [int] NOT NULL,
	[ScheduleHour] [int] NOT NULL,
	[ScheduleMinute] [int] NOT NULL,
	[PeriodicReportStatus] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[ServerGroupLkp]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[ServerGroupLkp](
	[ServerGroupID] [int] IDENTITY(1,1) NOT NULL,
	[ServerGroupName] [varchar](32) NOT NULL,
	[ServerGroupDescription] [varchar](256) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ServerGroupLkp] PRIMARY KEY CLUSTERED 
(
	[ServerGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[ServerGroupMembership]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[ServerGroupMembership](
	[VMInstanceID] [int] NOT NULL,
	[ServerGroupID] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [UC_ServerGroupMembership] UNIQUE NONCLUSTERED 
(
	[VMInstanceID] ASC,
	[ServerGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[TestPackage]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[TestPackage](
	[TestPackageID] [int] IDENTITY(1,1) NOT NULL,
	[TestPackageName] [varchar](256) NOT NULL,
	[TestPackageDescription] [varchar](1024) NULL,
	[UserName] [varchar](256) NULL,
	[CreateDate] [datetime] NOT NULL,
	[IsActive] [bit] NULL,
	[OperatingSystemID] [int] NULL,
	[LabID] [int] NULL,
 CONSTRAINT [PK_TestPackage] PRIMARY KEY CLUSTERED 
(
	[TestPackageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[TestPackageCommand]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[TestPackageCommand](
	[TestPackageCommandID] [int] IDENTITY(1,1) NOT NULL,
	[TestPackageID] [int] NOT NULL,
	[TestPackageVersionID] [int] NOT NULL,
	[TestPackageCommand] [varchar](256) NOT NULL,
	[ExecutionOrder] [int] NOT NULL,
	[TestPackageEntryPointID] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TestPackageCommand] PRIMARY KEY CLUSTERED 
(
	[TestPackageCommandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[TestPackageCommandLimit]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[TestPackageCommandLimit](
	[TestPackageCommandLimitID] [int] IDENTITY(1,1) NOT NULL,
	[TestPackageCommand] [varchar](256) NOT NULL,
	[MaxRunning] [int] NULL,
 CONSTRAINT [PK_TestPackageCommandLimit] PRIMARY KEY CLUSTERED 
(
	[TestPackageCommandLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[TestPackageVersion]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[TestPackageVersion](
	[TestPackageID] [int] NOT NULL,
	[TestPackageVersionID] [int] NOT NULL,
	[TestPackagePath] [varchar](256) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[Version] [varchar](32) NULL,
	[LicenseKey] [varchar](32) NULL,
	[ServerGroupID] [int] NULL,
	[LabID] [int] NULL,
 CONSTRAINT [PK_TestPackageVersion] PRIMARY KEY CLUSTERED 
(
	[TestPackageID] ASC,
	[TestPackageVersionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Definition].[VMInstance]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Definition].[VMInstance](
	[VMInstanceID] [int] IDENTITY(1,1) NOT NULL,
	[VMName] [varchar](32) NOT NULL,
	[HostName] [varchar](32) NOT NULL,
	[IPAddress] [varchar](16) NOT NULL,
	[VMConfigurationID] [int] NOT NULL,
	[VMState] [int] NOT NULL,
	[AlwaysOn] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[AddedByUser] [varchar](64) NOT NULL,
	[RunningTestJobID] [int] NULL,
	[Location] [varchar](32) NULL,
	[LastHeartbeat] [datetime] NULL,
 CONSTRAINT [PK_VMInstance] PRIMARY KEY CLUSTERED 
(
	[VMInstanceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [Active].[TestJob]  WITH CHECK ADD  CONSTRAINT [FK_TestJob_ConfigurationID] FOREIGN KEY([ConfigurationID])
REFERENCES [Definition].[Configuration] ([ConfigurationID])
GO
ALTER TABLE [Active].[TestJob] CHECK CONSTRAINT [FK_TestJob_ConfigurationID]
GO
ALTER TABLE [Active].[TestJob]  WITH CHECK ADD  CONSTRAINT [FK_TestJob_TestSuiteID] FOREIGN KEY([TestSuiteID])
REFERENCES [Active].[TestSuite] ([TestSuiteID])
GO
ALTER TABLE [Active].[TestJob] CHECK CONSTRAINT [FK_TestJob_TestSuiteID]
GO
ALTER TABLE [Active].[TestJob]  WITH CHECK ADD  CONSTRAINT [FK_TestJob_VMInstanceID] FOREIGN KEY([VMInstanceID])
REFERENCES [Definition].[VMInstance] ([VMInstanceID])
GO
ALTER TABLE [Active].[TestJob] CHECK CONSTRAINT [FK_TestJob_VMInstanceID]
GO
ALTER TABLE [Active].[TestJobCommand]  WITH CHECK ADD  CONSTRAINT [FK_TestJobCommand_TestJobID] FOREIGN KEY([TestJobID])
REFERENCES [Active].[TestJob] ([TestJobID])
GO
ALTER TABLE [Active].[TestJobCommand] CHECK CONSTRAINT [FK_TestJobCommand_TestJobID]
GO
ALTER TABLE [Active].[TestJobCommand]  WITH CHECK ADD  CONSTRAINT [FK_TestJobCommand_TestPackageCommandID] FOREIGN KEY([TestPackageCommandID])
REFERENCES [Definition].[TestPackageCommand] ([TestPackageCommandID])
GO
ALTER TABLE [Active].[TestJobCommand] CHECK CONSTRAINT [FK_TestJobCommand_TestPackageCommandID]
GO
ALTER TABLE [Active].[TestJobCommand]  WITH CHECK ADD  CONSTRAINT [FK_TestJobCommand_TestSuiteID] FOREIGN KEY([TestSuiteID])
REFERENCES [Active].[TestSuite] ([TestSuiteID])
GO
ALTER TABLE [Active].[TestJobCommand] CHECK CONSTRAINT [FK_TestJobCommand_TestSuiteID]
GO
ALTER TABLE [Active].[TestJobPlan]  WITH CHECK ADD  CONSTRAINT [FK_TestJobPlan_ConfigurationID] FOREIGN KEY([ConfigurationID])
REFERENCES [Definition].[Configuration] ([ConfigurationID])
GO
ALTER TABLE [Active].[TestJobPlan] CHECK CONSTRAINT [FK_TestJobPlan_ConfigurationID]
GO
ALTER TABLE [Active].[TestJobPlan]  WITH CHECK ADD  CONSTRAINT [FK_TestJobPlan_TestSuiteID] FOREIGN KEY([TestSuitePlanID])
REFERENCES [Active].[TestSuitePlan] ([TestSuitePlanID])
GO
ALTER TABLE [Active].[TestJobPlan] CHECK CONSTRAINT [FK_TestJobPlan_TestSuiteID]
GO
ALTER TABLE [Active].[TestSuite]  WITH CHECK ADD  CONSTRAINT [FK_TestSuite_TestPackageVersionID] FOREIGN KEY([TestPackageID], [TestPackageVersionID])
REFERENCES [Definition].[TestPackageVersion] ([TestPackageID], [TestPackageVersionID])
GO
ALTER TABLE [Active].[TestSuite] CHECK CONSTRAINT [FK_TestSuite_TestPackageVersionID]
GO
ALTER TABLE [Definition].[TestPackageCommand]  WITH CHECK ADD  CONSTRAINT [FK_TestPackageCommand_TestPackageVersionID] FOREIGN KEY([TestPackageID], [TestPackageVersionID])
REFERENCES [Definition].[TestPackageVersion] ([TestPackageID], [TestPackageVersionID])
GO
ALTER TABLE [Definition].[TestPackageCommand] CHECK CONSTRAINT [FK_TestPackageCommand_TestPackageVersionID]
GO
ALTER TABLE [Definition].[TestPackageVersion]  WITH CHECK ADD  CONSTRAINT [FK_TestPackageVersion_TestPackageID] FOREIGN KEY([TestPackageID])
REFERENCES [Definition].[TestPackage] ([TestPackageID])
GO
ALTER TABLE [Definition].[TestPackageVersion] CHECK CONSTRAINT [FK_TestPackageVersion_TestPackageID]
GO
ALTER TABLE [Definition].[VMInstance]  WITH CHECK ADD  CONSTRAINT [FK_VMInstance_VMConfigurationID] FOREIGN KEY([VMConfigurationID])
REFERENCES [Definition].[Configuration] ([ConfigurationID])
GO
ALTER TABLE [Definition].[VMInstance] CHECK CONSTRAINT [FK_VMInstance_VMConfigurationID]
GO
/****** Object:  StoredProcedure [Active].[CheckTestSuiteSchedule]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[CheckTestSuiteSchedule]
AS

DECLARE @TestSuiteScheduleID int
DECLARE @TestSuitePlanID int
DECLARE @NextScheduledTime datetime
DECLARE @CalculatedScheduledTime datetime
DECLARE @TestSuiteName varchar(256)
DECLARE @TestSuiteDescription varchar(1024)
DECLARE @TestPackageID int
DECLARE @Priority int
DECLARE @MaxRunning int
DECLARE @TestSuiteID int
DECLARE @TestPackageVersionID int
DECLARE @TestJobID int
DECLARE @TestSuiteState int
DECLARE @OperaringSystemID int
DECLARE @UserName varchar(64)

-- For every entry in the Active.TestSuiteSchedule table, find the latest scheduled test suite
DECLARE suiteCursor CURSOR FOR
SELECT	TestSuiteScheduleID,
		TestSuitePlanID
FROM	Active.TestSuiteSchedule

OPEN suiteCursor
FETCH NEXT FROM suiteCursor INTO @TestSuiteScheduleID, @TestSuitePlanID 

WHILE @@FETCH_STATUS = 0
BEGIN
	SELECT	@TestSuiteState = TestSuiteState
	FROM	Active.TestSuitePlan
	WHERE	TestSuitePlanID = @TestSuitePlanID

	IF (@TestSuiteState = 1)
	BEGIN
		SELECT	@NextScheduledTime = MAX(StartTime)
		FROM	Active.TestSuite
		WHERE	StartTime is not NULL
		AND		TestSuitePlanID = @TestSuitePlanID

		SELECT @CalculatedScheduledTime = Definition.fn_NextScheduledDatetime (@TestSuiteScheduleID, @NextScheduledTime)

		IF (@NextScheduledTime IS NULL OR DATEADD(minute, -1, @CalculatedScheduledTime) > @NextScheduledTime)
		BEGIN
			-- Create TestSuite
			print 'Next schedule for ' + CONVERT(varchar(6), @TestSuiteScheduleID) + ' is ' + CONVERT(varchar(20), @CalculatedScheduledTime)

			SELECT	@TestSuiteName = TestSuitePlanName,
					@TestSuiteDescription = TestSuitePlanDescription,
					@TestPackageID = TestPackageID,
					@Priority = TestPriority,
					@MaxRunning = MaxRunning,
					@OperaringSystemID = OperatingSystemID,
					@UserName = AddedByUser
			FROM	Active.TestSuitePlan
			WHERE	TestSuitePlanID = @TestSuitePlanID

			-- Create TestSuite
			EXEC Active.TestSuite_Create @TestSuiteName, @TestSuiteDescription, @TestPackageID, @Priority, @UserName, @MaxRunning, @OperaringSystemID, @TestSuiteID OUTPUT

			UPDATE	Active.TestSuite
			SET		StartTime = @CalculatedScheduledTime,
					TestSuitePlanID = @TestSuitePlanID
			WHERE	TestSuiteID = @TestSuiteID

			SELECT	@TestPackageVersionID = MAX(TestPackageVersionID)
			FROM	[Definition].[TestPackageVersion]
			WHERE	TestPackageID = @TestPackageID
			GROUP BY TestPackageID

			-- Create TestJobs
			INSERT INTO [Active].[TestJob] (
				TestSuiteID
				,TestPackageID
				,TestPackageVersionID
				,ConfigurationID
				,LicenseKey
				,DownloadLink
				,CreateDate
				,StartDate
				,EndDate
				,VMInstanceID
				,TestJobState
				,TestPriority
				,Version
				,TestSuiteDirectory
				,StartTime
				,RunCount)
			SELECT
				@TestSuiteID
				,TestPackageID
				,@TestPackageVersionID
				,ConfigurationID
				,LicenseKey
				,DownloadLink
				,SYSDATETIME()
				,null
				,null
				,null
				,1	 -- Queued
				,TestPriority
				,Version
				,NULL
				,@CalculatedScheduledTime
				,0
			FROM	Active.TestJobPlan
			WHERE	TestSuitePlanID = @TestSuitePlanID

			-- Find all newly created TestJobs
			DECLARE TestJobCursor CURSOR FOR
			SELECT	TestJobID
			FROM	[Active].[TestJob]
			WHERE	TestSuiteID = @TestSuiteID

			-- Create Commands for all new TestJobs
			OPEN TestJobCursor
			FETCH NEXT FROM TestJobCursor INTO @TestJobID 

			WHILE @@FETCH_STATUS = 0
			BEGIN
				EXEC [Active].[TestJobCommand_Create] @TestJobID
				FETCH NEXT FROM TestJobCursor INTO @TestJobID
			END

			CLOSE TestJobCursor
			DEALLOCATE TestJobCursor
		END
	END

	FETCH NEXT FROM suiteCursor INTO @TestSuiteScheduleID, @TestSuitePlanID
END

CLOSE suiteCursor
DEALLOCATE suiteCursor

RETURN 0

GO
/****** Object:  StoredProcedure [Active].[GetNextCommand]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[GetNextCommand]
	@pVMInstanceID int
AS

DECLARE @VMState int
DECLARE @testJobID int
DECLARE @executionOrder int
DECLARE	@testJobCommandID int
DECLARE @testJobCommand varchar(256)
DECLARE @Entry varchar(256)
DECLARE @testJobState int
DECLARE @packageCommandLimit int
DECLARE @runningCount int
DECLARE @timeoutMinutes int

-- Uncomment the next line to stop VMs from picking up new tasks:
-- RETURN 0

EXEC [Active].[Log_GetNextCommand] @pVMInstanceID

-- Update timestamp
UPDATE	[Definition].[VMInstance]
SET		LastHeartbeat = SYSDATETIME()
WHERE	VMInstanceID = @pVMInstanceID

-- First check whether the VM is active or not
SELECT	@VMState = VMState FROM [Definition].[VMInstance] WHERE VMInstanceID = @pVMInstanceID
IF (@VMState is NULL OR @VMState = 0)
	RETURN 0

-- Check if a command is currently assigned to this VM
SELECT	@testJobCommandID = TestJobCommandID,
		@testJobID = TestJobID,
		@executionOrder = TPC.ExecutionOrder
FROM	[Active].[TestJobCommand] TJC
JOIN	[Definition].[TestPackageCommand] TPC
ON		TJC.TestPackageCommandID = TPC.TestPackageCommandID
WHERE	VMInstanceID = @pVMInstanceID
AND		EndDate is NULL

IF (@testJobCommandID is not NULL AND @executionOrder != 0)
BEGIN
	SET @Entry = 'GetNextCommand called for VMInstanceID ' + CONVERT(varchar(10), @pVMInstanceID) + ' when TestJobCommandID ' + CONVERT(varchar(10), @testJobCommandID) + ' was still running.'
	INSERT INTO [Active].Log
	VALUES	(@Entry, NULL, 1, SYSDATETIME())

	SELECT	@testJobState = TestJobState
	FROM	Active.TestJob
	WHERE	TestJobID = @testJobID

	IF (@testJobState != 2)
	BEGIN
		UPDATE	Active.TestJobCommand
		SET		EndDate = CASE WHEN EndDate is NULL THEN SYSDATETIME() ELSE EndDate END,
				VMInstanceID = NULL
		WHERE	TestJobID = @testJobID

		SET @Entry = 'Reseting VMInstance entries for ' + CONVERT(varchar(10), @pVMInstanceID) + ' and TestJobCommandID ' + CONVERT(varchar(10), @testJobCommandID)
		EXEC	Active.Log_Insert @Entry, '', 0

		SET @testJobID = NULL
	END
END

-- Check if a TestJob entry is assigned to this VM
SELECT	@testJobID = TestJobID,
		@timeoutMinutes = TimeoutMinutes
FROM	Active.TestJob WHERE VMInstanceID = @pVMInstanceID
AND		TestJobState = 2 -- Running

IF (@testJobID is NULL)
BEGIN
	-- Select a new TestJob for this VM
	EXEC Active.SelectNextTestJob @pVMInstanceID, @testJobID OUTPUT
END

IF (@testJobID is NULL)
BEGIN
	-- Now there really is no TestJob
	RETURN 0
END

IF (@testJobCommandID is NULL)
BEGIN
	-- Return the next (first available) TestJobCommand or the currently running one.
	-- For that reason only the EndDate is checked, not the StartDate.
	SELECT	@testJobCommandID = (SELECT TOP(1) TestJobCommandID
	FROM	Active.TestJobCommand
	WHERE	TestJobID = @testJobID
	AND		EndDate is NULL
	ORDER BY ExecutionOrder)

	-- Check if there is a limit for this command
	SELECT	@packageCommandLimit = TPCL.MaxRunning,
			@testJobCommand = TPC.TestPackageCommand
	FROM	Active.TestJobCommand TJC
	JOIN	Definition.TestPackageCommand TPC
	ON		TJC.TestPackageCommandID = TPC.TestPackageCommandID
	JOIN	[Definition].[TestPackageCommandLimit] TPCL
	ON		TPC.TestPackageCommand = TPCL.TestPackageCommand
	WHERE	TJC.TestJobCommandID = @testJobCommandID

	IF (@packageCommandLimit is NOT NULL)
	BEGIN
		SELECT	@runningCount = COUNT(1)
		FROM	Active.TestJobCommand TJC
		JOIN	Definition.TestPackageCommand TPC
		ON		TJC.TestPackageCommandID = TPC.TestPackageCommandID
		WHERE	TJC.StartDate is not null
		AND		TJC.EndDate is null
		AND		TPC.TestPackageCommand = @testJobCommand

		IF (@runningCount > @packageCommandLimit)
		BEGIN
			RETURN 0
		END
	END
END

-- Do not overwrite an existing StartDate
UPDATE	Active.TestJobCommand
SET		StartDate = SYSDATETIME()
		,VMInstanceID = @pVMInstanceID
WHERE	TestJobCommandID = @testJobCommandID
AND		StartDate is NULL

SELECT	TestJobID
		,TestJobCommandID
		,(SELECT TestPackageCommand FROM Definition.TestPackageCommand TPC WHERE TPC.TestPackageCommandID = TJC.TestPackageCommandID)
		,ExecutionOrder
		,(CASE WHEN @timeoutMinutes IS NULL THEN 60 ELSE @timeoutMinutes END) AS TimeoutMinutes
		,(SELECT TestPackagePath FROM [Definition].[TestPackageVersion] TPV
				JOIN [Active].[TestSuite] TS ON TS.TestPackageVersionID = TPV.TestPackageVersionID AND TS.TestPackageID = TPV.TestPackageID
				JOIN [Active].[TestJob] TJ ON TJ.TestSuiteID = TS.TestSuiteID
				WHERE TJ.TestJobID = TJC.TestJobID)
		,(SELECT Version FROM [Active].[TestJob] TJ WHERE TJ.TestJobID = TJC.TestJobID)
		,(SELECT LicenseKey FROM [Active].[TestJob] TJ WHERE TJ.TestJobID = TJC.TestJobID)
		,(SELECT DownloadLink FROM [Active].[TestJob] TJ WHERE TJ.TestJobID = TJC.TestJobID)
		,(SELECT TestSuiteDirectory FROM [Active].[TestJob] TJ WHERE TJ.TestJobID = TJC.TestJobID)
		,(SELECT MAX(ExecutionOrder) FROM Active.TestJobCommand TJC2 where TJC2.TestJobID = TJC.TestJobID)
		,TestSuiteID
		,(SELECT AddedByUser FROM [Active].[TestSuite] TS WHERE TS.TestSuiteID = TJC.TestSuiteID)
		,(SELECT RunCount FROM [Active].[TestJob] TJ WHERE TJ.TestJobID = TJC.TestJobID)
FROM	Active.TestJobCommand TJC
WHERE	TestJobCommandID = @testJobCommandID

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[Log_GetNextCommand]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[Log_GetNextCommand]
	@pVMInstanceID int
AS

DECLARE @EntryDetails varchar(256)
SET @EntryDetails = '@pVMInstanceID=' + CONVERT(varchar(10), @pVMInstanceID)

INSERT INTO [Active].[Log]
VALUES ('Active.GetNextCommand called', @EntryDetails, 0, SYSDATETIME())

RETURN 0



GO
/****** Object:  StoredProcedure [Active].[Log_Insert]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[Log_Insert]
	@pLogEntry varchar(256),
	@pLogDetails varchar(1024),
	@pLogLevel int
AS

INSERT INTO [Active].[Log]
VALUES	(@pLogEntry
		,@pLogDetails
		,@pLogLevel
		,SYSDATETIME())

RETURN 0



GO
/****** Object:  StoredProcedure [Active].[Log_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[Log_Select]
	@pLogLevel int
AS

SELECT	TOP(100) LogEntry
		,LogDetails
		,LogLevel
		,CreateDate
FROM	[Active].[Log]
WHERE	LogLevel >= @pLogLevel
ORDER BY CreateDate DESC

RETURN 0

-- EXEC [Active].[Log_Select] 1
GO
/****** Object:  StoredProcedure [Active].[Log_TestJobCommandDone]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[Log_TestJobCommandDone]
	@pTestJobCommandID int,
	@pVMInstanceID int,
	@pPassedCount int,
	@pWarningCount int,
	@pFailedCount int
AS

DECLARE @EntryDetails varchar(1024)

SET @EntryDetails =
	'@pTestJobCommandID=' + CONVERT(varchar(10), @pTestJobCommandID) +
	'; @pVMInstanceID=' + CONVERT(varchar(10), @pVMInstanceID) +
	'; @pPassedCount=' + CONVERT(varchar(10), @pPassedCount) +
	'; @pWarningCount=' + CONVERT(varchar(10), @pWarningCount) +
	'; @pFailedCount=' + CONVERT(varchar(10), @pFailedCount)

INSERT INTO [Active].[Log]
VALUES ('Active.TestJobCommand_Done called', @EntryDetails, 0, SYSDATETIME())

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[Queue_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[Queue_Select]
	@pVMInstanceID int
AS

DECLARE @ConfigurationID int
DECLARE @OperatingSystemID int
DECLARE	@LabID int
DECLARE @LogEntry varchar(256)

SELECT	@ConfigurationID = VMConfigurationID
		,@OperatingSystemID = C.OperatingSystemID
FROM	[Definition].[VMInstance] VM
JOIN	[Definition].[Configuration] C
ON		VM.VMConfigurationID = C.ConfigurationID
WHERE	VMInstanceID = @pVMInstanceID

SELECT	@LabID = AutomationLabID
FROM	[Definition].[AutomationLabMembership]
WHERE	VMInstanceID = @pVMInstanceID

-- Select last 5 jobs
SELECT	DISTINCT TOP 5
	TJ.TestJobID,
	TJ.CreateDate,
	TJ.StartDate,
	TJ.EndDate,
	TP.TestPackageName,
	TS.MaxRunning,
	TS.AddedByUser,
	TJ.TestPriority,
	TJ.TestSuiteID
FROM	Active.TestJobCommand TJC
JOIN	Active.TestJob TJ
ON		TJ.TestJobID = TJC.TestJobID
JOIN	Active.TestSuite TS
ON		TS.TestSuiteID = TJ.TestSuiteID
JOIN	Definition.TestPackage TP
ON		TJ.TestPackageID = TP.TestPackageID
WHERE	TJ.StartDate is NOT NULL
AND		TJ.EndDate is NOT NULL
AND		TJC.VMInstanceID = @pVMInstanceID
AND		ExecutionOrder = 0
ORDER BY TJ.StartDate DESC

-- Select running
SELECT	TestJobID,
		TJ.CreateDate,
		TJ.StartDate,
		NULL AS EndDate,
		TP.TestPackageName,
		TS.MaxRunning,
		TS.AddedByUser,
		TJ.TestPriority,
		TJ.TestSuiteID
FROM	Active.TestJob TJ
JOIN	Definition.TestPackage TP
ON		TJ.TestPackageID = TP.TestPackageID
JOIN	Active.TestSuite TS
ON		TS.TestSuiteID = TJ.TestSuiteID
WHERE	VMInstanceID = @pVMInstanceID

-- Select queued
SELECT	TOP(5)
		TestJobID,
		TJ.CreateDate,
		NULL AS StartDate,
		NULL AS EndDate,
		TP.TestPackageName,
		TS.MaxRunning,
		TS.AddedByUser,
		TJ.TestPriority,
		TJ.TestSuiteID
FROM	Active.TestJob TJ
JOIN	Definition.TestPackage TP
ON		TJ.TestPackageID = TP.TestPackageID
JOIN	Active.TestSuite TS
ON		TS.TestSuiteID = TJ.TestSuiteID
WHERE	TJ.StartDate is NULL
AND		(ConfigurationID = @ConfigurationID OR ConfigurationID = 25)	-- 25 is first available VM
AND		((SELECT 1 FROM [Active].[TestSuite] TS
			JOIN	[Definition].[ServerGroupMembership] SGM ON SGM.ServerGroupID = TS.ServerGroupID
			WHERE	SGM.VMInstanceID = @pVMInstanceID
			AND		TS.TestSuiteID = TJ.TestSuiteID) is not NULL)
--AND		(TJ.StartTime IS NULL OR TJ.StartTime < SYSDATETIME())
AND		TS.AutomationLabID = @LabID
AND		TS.OperatingSystemID = @OperatingSystemID
ORDER BY TJ.TestPriority, TestJobID

GO
/****** Object:  StoredProcedure [Active].[SelectNextTestJob]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Active].[SelectNextTestJob]
	@pVMInstanceID int,
	@oTestJobID int OUTPUT
AS

DECLARE @ConfigurationID int
DECLARE @LogEntry varchar(256)
DECLARE @OperatingSystemID int

SET @oTestJobID = null

--RETURN 0

SELECT	@ConfigurationID = VMConfigurationID,
		@OperatingSystemID = OperatingSystemID
FROM	[Definition].[VMInstance] VM
JOIN	[Definition].[Configuration] C
ON		VM.VMConfigurationID = C.ConfigurationID
WHERE	VMInstanceID = @pVMInstanceID

IF (@ConfigurationID is NULL)
BEGIN
	SELECT @LogEntry = 'VM with ID ' + CONVERT(varchar(10), @pVMInstanceID) + ' has an invalid configuration.';
	EXEC [Active].[Log_Insert] @LogEntry, null, 2
	RETURN 0
END

SELECT	TOP(1) @oTestJobID = TestJobID
FROM	Active.TestJob TJ
WHERE	TestJobState = 1 -- Scheduled/Queued; was: StartDate is NULL
AND		(ConfigurationID = @ConfigurationID OR ConfigurationID = 25)	-- 25 is first available VM
AND		(((SELECT [MaxRunning] FROM [Active].[TestSuite] TS WHERE TS.TestSuiteID = TJ.TestSuiteID) is null)
	OR
		(
			(SELECT	COUNT(1)
			FROM	[Active].[TestJob] TJ2
			WHERE	StartDate is not NULL
			AND		EndDate is NULL
			AND		TJ.TestSuiteID = TJ2.TestSuiteID) < (SELECT [MaxRunning] FROM [Active].[TestSuite] TS WHERE TS.TestSuiteID = TJ.TestSuiteID))
		)
AND		((SELECT 1 FROM [Active].[TestSuite] TS
			JOIN	[Definition].[ServerGroupMembership] SGM ON SGM.ServerGroupID = TS.ServerGroupID
			WHERE	SGM.VMInstanceID = @pVMInstanceID
			AND		TS.TestSuiteID = TJ.TestSuiteID) is not NULL)
AND		((SELECT 1 FROM [Active].[TestSuite] TS
			JOIN	[Definition].[AutomationLabMembership] ALM ON ALM.AutomationLabID = TS.AutomationLabID
			WHERE	ALM.VMInstanceID = @pVMInstanceID
			AND		TS.TestSuiteID = TJ.TestSuiteID) is not NULL)
AND		((SELECT 1 FROM [Active].[TestSuite] TS
			WHERE	TS.TestSuiteID = TJ.TestSuiteID
			AND		(TS.OperatingSystemID IS NULL OR TS.OperatingSystemID = @OperatingSystemID)) is not NULL)
AND		(StartTime IS NULL OR StartTime < SYSDATETIME())
AND		(VMInstanceID IS NULL OR VMInstanceID = @pVMInstanceID)	-- new
ORDER BY TestPriority, TestJobID

IF (@oTestJobID is not NULL)
BEGIN
	UPDATE	Active.TestJob
	SET		StartDate = SYSDATETIME()
			,VMInstanceID = @pVMInstanceID
			,TestJobState = 2		-- Running
	WHERE	TestJobID = @oTestJobID

	UPDATE	TS
	SET		StartDate = SYSDATETIME()
			,TestSuiteState = 2		-- Running
	FROM	Active.TestSuite TS
	JOIN	Active.TestJob TJ
	ON		TJ.TestSuiteID = TS.TestSuiteID
	WHERE	TS.StartDate is null
	AND		TJ.TestJobID = @oTestJobID
END

RETURN 0



GO
/****** Object:  StoredProcedure [Active].[TestCommand_Create]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestCommand_Create]
	@pTestJobID int
AS

DECLARE @TestPackageID int
DECLARE @TestPackageVersionID int

SELECT	@TestPackageID = TS.TestPackageID
FROM	Active.TestSuite TS (NOLOCK)
JOIN	Active.TestJob TJ (NOLOCK)
ON		TS.TestSuiteID = TJ.TestSuiteID
WHERE	TJ.TestJobID = @pTestJobID

SELECT	@TestPackageVersionID = MAX(TestPackageVersionID)
FROM	Definition.TestPackageVersion TPV
WHERE	TestPackageID = @TestPackageID

INSERT INTO Active.TestJobCommand(
	TestJobID,
	TestSuiteID,
	TestPackageCommandID,
	ExecutionOrder,
	CreateDate)
SELECT	@pTestJobID
		,(SELECT TestSuiteID FROM Active.TestJob WHERE TestJobID = @pTestJobID)
		,TPC.TestPackageCommandID
		,TPC.ExecutionOrder
		,SYSDATETIME()
FROM	Definition.TestPackageCommand TPC
WHERE	TPC.TestPackageID = @TestPackageID
AND		TPC.TestPackageVersionID = @TestPackageVersionID
ORDER BY TPC.ExecutionOrder

RETURN 0



GO
/****** Object:  StoredProcedure [Active].[TestJob_Cancel]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestJob_Cancel]
	@pTestJobID int = null,
	@pVMInstanceID int = null,
	@pMessage varchar(256)
AS

DECLARE @Count int
DECLARE @TestJobID int
DECLARE @TestSuiteID int
DECLARE @LogEntry varchar(256)

IF (@pVMInstanceID is not NULL)
BEGIN
	SELECT	@TestJobID = TestJobID,
			@TestSuiteID = TestSuiteID
	FROM	[Active].[TestJob]
	WHERE	VMInstanceID = @pVMInstanceID

	IF (@TestJobID is NULL) RETURN 0
END
ELSE
BEGIN
	SET @TestJobID = @pTestJobID
	SELECT @TestSuiteID = TestSuiteID
	FROM [Active].[TestJob]
	WHERE TestJobID = @TestJobID
END

SELECT	@LogEntry = 'Job ' + CONVERT(varchar(10), @TestJobID) + ' cancelled due to ' + @pMessage
EXEC	Active.Log_Insert @LogEntry, null, 1

UPDATE	[Active].[TestJob]
SET		StartDate = (CASE WHEN StartDate is NULL THEN SYSDATETIME() ELSE StartDate END)
		,EndDate = (CASE WHEN EndDate is NULL THEN SYSDATETIME() ELSE EndDate END)
		,TestJobState = 5		-- Canceled
		,VMInstanceID = null
WHERE	TestJobID = @TestJobID

UPDATE	[Active].[TestJobCommand]
SET		ResultString = @pMessage
WHERE	TestJobID = @TestJobID
AND		StartDate IS NOT NULL
AND		EndDate IS NULL
AND		ResultString IS NULL

UPDATE	[Active].[TestJobCommand]
SET		StartDate = (CASE WHEN StartDate is NULL THEN SYSDATETIME() ELSE StartDate END)
		-- Set EndDate on all entries that have not run yet, and are not currently running
		,EndDate = (CASE WHEN (EndDate is NULL AND VMInstanceID is NULL) THEN SYSDATETIME() ELSE EndDate END)
WHERE	TestJobID = @TestJobID

-- Update TestSuite state
SELECT	@Count = COUNT(1)
FROM	Active.TestJob TJ
WHERE	TJ.EndDate is null
AND		TJ.TestSuiteID = @TestSuiteID

if (@Count = 0)
BEGIN
	UPDATE	TS
	SET		EndDate = SYSDATETIME()
			,TestSuiteState = 3		-- Done
	FROM	Active.TestSuite TS
	JOIN	Active.TestJob TJ
	ON		TJ.TestSuiteID = TS.TestSuiteID
	WHERE	TS.EndDate is null
	AND		TJ.TestJobID = @TestJobID
END

RETURN 0

GO
/****** Object:  StoredProcedure [Active].[TestJob_Create]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Active].[TestJob_Create]
	@pTestSuiteID int,
	@pTestPackageID int,
	@pConfigurationID int,
	@pLicenseKey varchar(32),
	@pDownloadLink varchar(32),
	@pPriority int,
	@pVersion varchar(32),
	@pTestSuiteDirectory varchar(256) = null,
	@oTestJobID int OUTPUT
AS

DECLARE @TestPackageVersionID int

SELECT	@TestPackageVersionID = MAX(TestPackageVersionID)
FROM	[Definition].[TestPackageVersion]
WHERE	TestPackageID = @pTestPackageID
GROUP BY TestPackageID

INSERT INTO [Active].[TestJob] (
	TestSuiteID
	,TestPackageID
	,TestPackageVersionID
	,ConfigurationID
	,LicenseKey
	,DownloadLink
	,CreateDate
	,StartDate
	,EndDate
	,VMInstanceID
	,TestJobState
	,TestPriority
	,Version
	,TestSuiteDirectory
	,StopOnError
	,RunCount)
VALUES (
	@pTestSuiteID
	,@pTestPackageID
	,@TestPackageVersionID
	,@pConfigurationID
	,@pLicenseKey
	,@pDownloadLink
	,SYSDATETIME()
	,null
	,null
	,null
	,1	 -- Queued
	,@pPriority
	,@pVersion
	,@pTestSuiteDirectory
	,0
	,0)

SET @oTestJobID = @@IDENTITY

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[TestJob_Pause]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Active].[TestJob_Pause]
	@pTestJobID int
AS

-- It's only possible to pause a running test job
UPDATE	[Active].[TestJob]
SET		TestJobState = 4		-- Paused
WHERE	TestJobID = @pTestJobID
AND		TestJobState = 2		-- Running

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[TestJob_Reset]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Active].[TestJob_Reset]
	@pTestJobID int,
	@pMessage varchar(256)
AS

DECLARE @LogDetails varchar(1024)
DECLARE @State int

SELECT	@State = TestJobState
FROM	[Active].[TestJob]
WHERE	TestJobID = @pTestJobID

IF (@State = 6)
BEGIN
	SET @LogDetails = '[Active].[TestJob_Reset] called canceled TestJob ' + CONVERT(varchar(10), @pTestJobID)
	EXEC Active.Log_Insert @LogDetails, @pMessage, 1
	RETURN 0
END

UPDATE	[Active].[TestJob]
SET		StartDate = null,
		EndDate = null,
		VMInstanceID = null,
		TestJobState = 1,		-- Queued
		RunCount = RunCount + 1
WHERE	TestJobID = @pTestJobID

-- TODO: update the TestSuite state

UPDATE	[Active].[TestJobCommand]
SET		StartDate = null,
		EndDate = null,
		PassedCount = null,
		WarningCount = null,
		FailedCount = null,
		SkippedCount = null,
		ResultString = null,
		VMInstanceID = null,
		Version = null,
		DownloadLink = null,
		DumpFilesGenerated = 0
WHERE	TestJobID = @pTestJobID

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[TestJob_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Active].[TestJob_Select]
	@pTestSuiteID int = null,
	@pTestJobID int = null,
	@pOperatingSystemID int = null
AS

	SELECT	TestJobID
			,TJ.TestSuiteID
			,ConfigurationID
			,LicenseKey
			,DownloadLink
			,TJ.CreateDate
			,TJ.StartDate
			,TJ.EndDate
			,TS.TestSuiteName
			,TJ.TestPriority
			,TS.TestPackageID
			,TS.TestPackageVersionID
			,(SELECT VMName FROM [Definition].[VMInstance] VI WHERE VI.VMInstanceID = TJ.VMInstanceID)
			,(SELECT ConfigurationName FROM [Definition].[Configuration] CF WHERE CF.ConfigurationID = TJ.ConfigurationID)
			,(SELECT TestPackageName FROM [Definition].[TestPackage] TP
				WHERE TS.TestPackageID = TP.TestPackageID)
			,TJ.StartTime
			,Version
			,'Unknown'
			,TJ.TestJobState
	FROM	[Active].[TestJob] TJ (NOLOCK)
	JOIN	[Active].[TestSuite] TS (NOLOCK)
	ON		TJ.TestSuiteID = TS.TestSuiteID
	WHERE	(@pTestSuiteID is NULL OR TJ.TestSuiteID = @pTestSuiteID)
	AND		(@pTestJobID is NULL OR TestJobID = @pTestJobID)
	AND		TestJobState < 6
	AND		(OperatingSystemID is NULL OR TS.OperatingSystemID = @pOperatingSystemID)

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[TestJob_SetStopOnError]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestJob_SetStopOnError]
	@pTestJobID int,
	@pStopOnError	bit
AS

UPDATE	[Active].[TestJob]
SET		StopOnError = @pStopOnError
WHERE	TestJobID = @pTestJobID

RETURN 0

GO
/****** Object:  StoredProcedure [Active].[TestJob_SetTimeout]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestJob_SetTimeout]
	@pTestJobID int,
	@pTimeout	int
AS

UPDATE	[Active].[TestJob]
SET		TimeoutMinutes = @pTimeout
WHERE	TestJobID = @pTestJobID

RETURN 0

GO
/****** Object:  StoredProcedure [Active].[TestJob_Start]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Active].[TestJob_Start]
	@pTestJobID int
AS

-- It's only possible to pause a running test job
UPDATE	[Active].[TestJob]
SET		TestJobState = 1		-- Scheduled
WHERE	TestJobID = @pTestJobID
AND		TestJobState = 4		-- Paused

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[TestJobCommand_Create]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestJobCommand_Create]
	@pTestJobID int
AS

DECLARE @TestPackageID int
DECLARE @TestPackageVersionID int

SELECT	@TestPackageID = TS.TestPackageID
FROM	Active.TestSuite TS (NOLOCK)
JOIN	Active.TestJob TJ (NOLOCK)
ON		TS.TestSuiteID = TJ.TestSuiteID
WHERE	TJ.TestJobID = @pTestJobID

SELECT	@TestPackageVersionID = MAX(TestPackageVersionID)
FROM	Definition.TestPackageVersion TPV
WHERE	TestPackageID = @TestPackageID

INSERT INTO Active.TestJobCommand(
	TestJobID,
	TestSuiteID,
	TestPackageCommandID,
	ExecutionOrder,
	CreateDate)
SELECT	@pTestJobID
		,(SELECT TestSuiteID FROM Active.TestJob WHERE TestJobID = @pTestJobID)
		,TPC.TestPackageCommandID
		,TPC.ExecutionOrder
		,SYSDATETIME()
FROM	Definition.TestPackageCommand TPC
WHERE	TPC.TestPackageID = @TestPackageID
AND		TPC.TestPackageVersionID = @TestPackageVersionID
ORDER BY TPC.ExecutionOrder

RETURN 0



GO
/****** Object:  StoredProcedure [Active].[TestJobCommand_Done]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Active].[TestJobCommand_Done]
	@pTestJobCommandID int,
	@pVMInstanceID int,
	@pPassedCount int,
	@pWarningCount int,
	@pFailedCount int,
	@pDumpFilesGenerated bit,
	@pResultString varchar(256),
	@pVersion varchar(32) = null,
	@pDownloadLink varchar(32) = null,
	@pSkippedCount int = null,
	@oTestJobState int OUTPUT
AS

DECLARE @Count int
DECLARE @TestJobID int
DECLARE @TestSuiteID int
DECLARE @StopOnError bit

EXEC [Active].[Log_TestJobCommandDone] @pTestJobCommandID, @pVMInstanceID, @pPassedCount, @pWarningCount, @pFailedCount

UPDATE	Active.TestJobCommand
SET		EndDate = SYSDATETIME()
		,PassedCount = @pPassedCount
		,WarningCount = @pWarningCount
		,FailedCount = @pFailedCount
		,SkippedCount = @pSkippedCount
		,ResultString = (CASE WHEN ResultString IS NULL THEN @pResultString ELSE ResultString END)
		,DumpFilesGenerated = @pDumpFilesGenerated
		,Version = @pVersion
		,DownloadLink = @pDownloadLink
WHERE	StartDate is not NULL
AND		EndDate is NULL
AND		TestJobCommandID = @pTestJobCommandID

SELECT	@TestJobID = TJC.TestJobID
		,@StopOnError = StopOnError
FROM	Active.TestJobCommand TJC
JOIN	Active.TestJob TJ
ON		TJ.TestJobID = TJC.TestJobID
WHERE	TJC.TestJobCommandID = @pTestJobCommandID

--IF (@pFailedCount > 0 AND @StopOnError = 1)
--BEGIN
--	EXEC Active.TestJob_Cancel @TestJobID, NULL, 'Stopping on error.'
--END
--ELSE
BEGIN
	-- Check if all commands are done
	SELECT	@Count = COUNT(1)
	FROM	Active.TestJobCommand
	WHERE	EndDate is null
	AND		TestJobID = @TestJobID

	IF (@Count = 0)
	BEGIN
		-- TestJob is done
		UPDATE	TJ
		SET		EndDate = SYSDATETIME()
				,VMInstanceID = null
				,TestJobState = 3		-- Done
		FROM	Active.TestJob TJ
		WHERE	TJ.EndDate is null
		AND		TJ.TestJobID = @TestJobID

		SELECT	@TestSuiteID = TestSuiteID
		FROM	Active.TestJob
		WHERE	TestJobID = @TestJobID

		-- Check if all testJobs are done for a given suite
		SELECT	@Count = COUNT(1)
		FROM	Active.TestJob TJ
		JOIN	Active.TestJobCommand TJC
		ON		TJ.TestJobID = TJC.TestJobID
		WHERE	TJ.EndDate is null
		AND		TJ.TestSuiteID = @TestSuiteID

		if (@Count = 0)
		BEGIN
			UPDATE	Active.TestSuite
			SET		EndDate = SYSDATETIME()
					,TestSuiteState = 3		-- Done
			WHERE	EndDate is null
			AND		TestSuiteID = @TestSuiteID
		END
	END
END

SELECT	@oTestJobState = TestJobState
FROM	Active.TestJob
WHERE	TestJobID = @TestJobID

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[TestJobCommand_IsRunning]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Active].[TestJobCommand_IsRunning]
	@pTestJobCommandID int,
	@pRunCount int = null
AS

DECLARE @State int
DECLARE @RunCount int
DECLARE @VMInstanceID int

SELECT	@State = TJ.TestJobState
		,@RunCount = TJ.RunCount
		,@VMInstanceID = TJ.VMInstanceID
FROM	[Active].[TestJob] TJ
JOIN	[Active].[TestJobCommand] TJC
ON		TJ.TestJobID = TJC.TestJobID
WHERE	TJC.TestJobCommandID = @pTestJobCommandID

IF (@pRunCount IS NULL)
BEGIN
	IF (@State >= 5)
	BEGIN
		SELECT 0
	END
	ELSE
	BEGIN
		SELECT 1
	END
END
ELSE
BEGIN
	IF (@State >= 5 OR @RunCount != @pRunCount)
	BEGIN
		SELECT 0
	END
	ELSE
	BEGIN
		SELECT 1
	END
END

IF (@VMInstanceID IS NOT NULL)
BEGIN
	UPDATE	[Definition].[VMInstance]
	SET		LastHeartbeat = SYSDATETIME()
	WHERE	VMInstanceID = @VMInstanceID
END

RETURN 1

GO
/****** Object:  StoredProcedure [Active].[TestJobCommand_Reset]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Active].[TestJobCommand_Reset]
	@pVMInstanceID int,
	@pMessage varchar(256)
AS

DECLARE @TestJobID int
DECLARE @LogDetails varchar(1024)

SELECT	TestJobID
FROM	[Active].[TestJob]
WHERE	VMInstanceID = @pVMInstanceID

IF (@@ROWCOUNT > 1)
BEGIN
	SET @LogDetails = 'More than one TestJob associated with VM ' + CONVERT(varchar(10), @pVMInstanceID)
	EXEC Active.Log_Insert @LogDetails, '', 1
	RETURN -1
END

SET @LogDetails = '[Active].[TestJobCommand_Reset] called for VM ' + CONVERT(varchar(10), @pVMInstanceID)
EXEC Active.Log_Insert @LogDetails, @pMessage, 1

SELECT	@TestJobID = TestJobID
FROM	[Active].[TestJob]
WHERE	VMInstanceID = @pVMInstanceID

UPDATE	[Active].[TestJob]
SET		StartDate = null,
		EndDate = null,
		VMInstanceID = null,
		TestJobState = 1		-- Queued
WHERE	TestJobID = @TestJobID

-- TODO: update the TestSuite state

UPDATE	[Active].[TestJobCommand]
SET		StartDate = null,
		EndDate = null,
		PassedCount = null,
		WarningCount = null,
		FailedCount = null,
		ResultString = null
WHERE	TestJobID = @TestJobID

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[TestJobCommand_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Active].[TestJobCommand_Select]
	@pTestSuiteID int = NULL,
	@pTestJobID int = NULL,
	@pOperatingSystemID int = NULL
AS

--RETURN 0

IF (@pTestSuiteID is NULL AND @pTestJobID is NULL)
BEGIN
	SELECT	TOP(200) TestJobCommandID
			,source.TestSuiteID
			,source.TestJobID
			,1
			,(SELECT TestPackageCommand FROM [Definition].[TestPackageCommand] target WHERE target.TestPackageCommandID = source.TestPackageCommandID) AS EntryPoint
			,''
			,source.CreateDate
			,source.StartDate
			,source.EndDate
			,PassedCount
			,WarningCount
			,FailedCount
			,ResultString
			,(SELECT VMName FROM [Definition].[VMInstance] VM WHERE VM.VMInstanceID = source.VMInstanceID)
			--,(SELECT TestPackagePath FROM [Definition].[TestPackageVersion] TPV
			--	JOIN [Active].[TestSuite] TS ON TS.TestPackageVersionID = TPV.TestPackageVersionID AND TS.TestPackageID = TPV.TestPackageID
			--	JOIN [Active].[TestJob] TJ ON TJ.TestSuiteID = TS.TestSuiteID
			--	WHERE TJ.TestJobID = source.TestJobID)
			,''
			,CONVERT(bit, (CASE WHEN DumpFilesGenerated is NULL THEN 0 ELSE DumpFilesGenerated END))
			,(SELECT ConfigurationName FROM Definition.Configuration CO
				JOIN Active.TestJob TJ ON TJ.TestJobID = source.TestJobID
				WHERE CO.ConfigurationID = TJ.ConfigurationID)
			,Version
			,source.DownloadLink
			,TJ.RunCount
			,SkippedCount
	FROM	[Active].[TestJobCommand] source WITH (NOLOCK)
	JOIN	[Active].[TestJob] TJ
	ON		TJ.TestJobID = source.TestJobID
	JOIN	[Active].[TestSuite] TS
	ON		TJ.TestSuiteID = TS.TestSuiteID
	WHERE	TJ.TestJobState < 5
	AND		(@pOperatingSystemID IS NULL OR @pOperatingSystemID = TS.OperatingSystemID)
	ORDER BY source.StartDate DESC 
END
ELSE
IF (@pTestJobID IS NOT NULL)
BEGIN
	SELECT	TestJobCommandID
			,source.TestSuiteID
			,source.TestJobID
			,1
			,(SELECT TestPackageCommand FROM [Definition].[TestPackageCommand] target WHERE target.TestPackageCommandID = source.TestPackageCommandID) AS EntryPoint
			,''
			,source.CreateDate
			,source.StartDate
			,source.EndDate
			,PassedCount
			,WarningCount
			,FailedCount
			,ResultString
			,(SELECT VMName FROM [Definition].[VMInstance] VM WHERE VM.VMInstanceID = source.VMInstanceID)
			--,(SELECT TestPackagePath FROM [Definition].[TestPackageVersion] TPV
			--	JOIN [Active].[TestSuite] TS ON TS.TestPackageVersionID = TPV.TestPackageVersionID AND TS.TestPackageID = TPV.TestPackageID
			--	JOIN [Active].[TestJob] TJ ON TJ.TestSuiteID = TS.TestSuiteID
			--	WHERE TJ.TestJobID = source.TestJobID)
			,''
			,CONVERT(bit, (CASE WHEN DumpFilesGenerated is NULL THEN 0 ELSE DumpFilesGenerated END))
			,(SELECT ConfigurationName FROM Definition.Configuration CO
				JOIN Active.TestJob TJ ON TJ.TestJobID = source.TestJobID
				WHERE CO.ConfigurationID = TJ.ConfigurationID)
			,Version
			,source.DownloadLink
			,TJ.RunCount
			,SkippedCount
	FROM	[Active].[TestJobCommand] source WITH (NOLOCK)
	JOIN	[Active].[TestJob] TJ
	ON		TJ.TestJobID = source.TestJobID
	WHERE	source.TestJobID = @pTestJobID
	AND		TJ.TestJobState < 6
	ORDER BY TestJobCommandID
END
ELSE
IF (@pTestSuiteID IS NOT NULL)
BEGIN
	SELECT	TestJobCommandID
			,source.TestSuiteID
			,source.TestJobID
			,1
			,(SELECT TestPackageCommand FROM [Definition].[TestPackageCommand] target WHERE target.TestPackageCommandID = source.TestPackageCommandID) AS EntryPoint
			,''
			,source.CreateDate
			,source.StartDate
			,source.EndDate
			,PassedCount
			,WarningCount
			,FailedCount
			,ResultString
			,(SELECT VMName FROM [Definition].[VMInstance] VM WHERE VM.VMInstanceID = source.VMInstanceID)
			--,(SELECT TestPackagePath FROM [Definition].[TestPackageVersion] TPV
			--	JOIN [Active].[TestSuite] TS ON TS.TestPackageVersionID = TPV.TestPackageVersionID AND TS.TestPackageID = TPV.TestPackageID
			--	JOIN [Active].[TestJob] TJ ON TJ.TestSuiteID = TS.TestSuiteID
			--	WHERE TJ.TestJobID = source.TestJobID)
			,''
			,CONVERT(bit, (CASE WHEN DumpFilesGenerated is NULL THEN 0 ELSE DumpFilesGenerated END))
			,(SELECT ConfigurationName FROM Definition.Configuration CO
				JOIN Active.TestJob TJ ON TJ.TestJobID = source.TestJobID
				WHERE CO.ConfigurationID = TJ.ConfigurationID)
			,Version
			,source.DownloadLink
			,TJ.RunCount
	FROM	[Active].[TestJobCommand] source WITH (NOLOCK)
	JOIN	[Active].[TestJob] TJ
	ON		TJ.TestJobID = source.TestJobID
	WHERE	source.TestSuiteID = @pTestSuiteID
	AND		TJ.TestJobState < 6
	ORDER BY TestJobCommandID
END

RETURN 0

-- exec [Data].[TestInstance_Select] 1, null

GO
/****** Object:  StoredProcedure [Active].[TestJobPlan_Create]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Active].[TestJobPlan_Create]
	@pTestSuitePlanID int,
	@pTestPackageID int,
	@pConfigurationID int,
	@pLicenseKey varchar(32),
	@pDownloadLink varchar(32),
	@pPriority int,
	@pVersion varchar(32),
	@oTestJobPlanID int OUTPUT
AS

INSERT INTO [Active].[TestJobPlan] (
	TestSuitePlanID
	,TestPackageID
	,ConfigurationID
	,LicenseKey
	,DownloadLink
	,CreateDate
	,TestJobState
	,TestPriority
	,Version)
VALUES (
	@pTestSuitePlanID
	,@pTestPackageID
	,@pConfigurationID
	,@pLicenseKey
	,@pDownloadLink
	,SYSDATETIME()
	,1	 -- Scheduled
	,@pPriority
	,@pVersion)

SET @oTestJobPlanID = @@IDENTITY

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[TestJobReport_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestJobReport_Select]
	@pTestJobID int
AS

SELECT	TJC.TestSuiteID,
		TPC.TestPackageCommand,
		SUM(PassedCount),
		SUM(WarningCount),
		SUM(FailedCount),
		COUNT(1),
		TJC.TestJobCommandID
FROM	Active.TestJobCommand TJC
JOIN	Definition.TestPackageCommand TPC
ON		TJC.TestPackageCommandID = TPC.TestPackageCommandID
WHERE	TestJobID = @pTestJobID
AND		TJC.StartDate IS NOT NULL
AND		TJC.StartDate != TJC.EndDate
GROUP BY TPC.TestPackageCommand, TJC.TestSuiteID, TJC.TestJobCommandID
ORDER BY TJC.TestJobCommandID

RETURN 0

-- EXEC [Active].[TestJobReport_Select] 32044

GO
/****** Object:  StoredProcedure [Active].[TestPlanSchedule_Create]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestPlanSchedule_Create]
	@pTestSuitePlanID int,
	@oTestSuiteScheduleID int OUTPUT
AS

INSERT INTO [Active].[TestSuiteSchedule]
VALUES	(@pTestSuitePlanID)

SELECT @oTestSuiteScheduleID = @@IDENTITY

RETURN 0

GO
/****** Object:  StoredProcedure [Active].[TestPlanScheduleEntry_Create]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestPlanScheduleEntry_Create]
	@pTestSuiteScheduleID int,
	@pWeekDayID int,
	@pStartingHour int,
	@pStartingMinute int
AS

INSERT INTO [Active].[TestSuiteScheduleEntry] (
	[TestSuiteScheduleID],
	[WeekDayID],
	[StartingHour],
	[StartingMinute])
VALUES (@pTestSuiteScheduleID,
		@pWeekDayID,
		@pStartingHour,
		@pStartingMinute)

RETURN 0

GO
/****** Object:  StoredProcedure [Active].[TestServer_Start]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestServer_Start]
AS

RETURN 0

GO
/****** Object:  StoredProcedure [Active].[TestSuite_Cancel]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestSuite_Cancel]
	@pTestSuiteID int
AS

UPDATE	[Active].[TestSuite]
SET		StartDate = (CASE WHEN StartDate is NULL THEN SYSDATETIME() ELSE StartDate END)
		,EndDate = (CASE WHEN EndDate is NULL THEN SYSDATETIME() ELSE EndDate END)
		,TestSuiteDescription = TestSuiteDescription + ' [Canceled]'
		,TestSuiteState = 5		-- Canceled
WHERE	TestSuiteID = @ptestSuiteID

UPDATE	[Active].[TestJob]
SET		StartDate = (CASE WHEN StartDate is NULL THEN SYSDATETIME() ELSE StartDate END)
		,EndDate = (CASE WHEN EndDate is NULL THEN SYSDATETIME() ELSE EndDate END)
		,TestJobState = 5		-- Canceled
		,VMInstanceID = null
WHERE	TestSuiteID = @pTestSuiteID

UPDATE	[Active].[TestJobCommand]
SET		StartDate = (CASE WHEN StartDate is NULL THEN SYSDATETIME() ELSE StartDate END)
		-- Set EndDate on all entries that have not run yet, and are not currently running
		,EndDate = (CASE WHEN (EndDate is NULL AND VMInstanceID is NULL) THEN SYSDATETIME() ELSE EndDate END)
WHERE	TestSuiteID = @pTestSuiteID

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[TestSuite_Create]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestSuite_Create]
	@pTestSuiteName varchar(256),
	@pTestSuiteDescription varchar(1024),
	@pTestPackageID int,
	@pTestPriority int,
	@pUser varchar(64),
	@pMaxRunning int = null,
	@pOperatingSystemID int = null,
	@oTestSuiteID int OUTPUT
AS

DECLARE @TestPackageVersionID int
DECLARE @ServerGroupID int
DECLARE @AutomationLabID int

SELECT	@TestPackageVersionID = MAX(TestPackageVersionID)
FROM	[Definition].[TestPackageVersion]
WHERE	TestPackageID = @pTestPackageID
GROUP BY TestPackageID

SELECT	@ServerGroupID = ServerGroupID,
		@AutomationLabID = LabID
FROM	[Definition].[TestPackageVersion]
WHERE	TestPackageVersionID = @TestPackageVersionID
AND		TestPackageID = @pTestPackageID

INSERT INTO [Active].[TestSuite] (
	[TestSuiteName],
	[TestSuiteDescription],
	[TestPackageID],
	[TestPackageVersionID],
	[TestPriority],
	[CreateDate],
	[AddedByUser],
	[TestSuiteState],
	[MaxRunning],
	[ServerGroupID],
	[AutomationLabID],
	[OperatingSystemID])
VALUES (
	@pTestSuiteName,
	@pTestSuiteDescription,
	@pTestPackageID,
	@TestPackageVersionID,
	@pTestPriority,
	SYSDATETIME(),
	@pUser,
	1,	-- Queued
	@pMaxRunning,
	(CASE WHEN @ServerGroupID IS NULL THEN 1 ELSE @ServerGroupID END),
	@AutomationLabID,
	@pOperatingSystemID)

SET @oTestSuiteID = @@IDENTITY

RETURN 0





GO
/****** Object:  StoredProcedure [Active].[TestSuite_Delete]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestSuite_Delete]
	@pTestSuiteID int
AS

UPDATE	[Active].[TestSuite]
SET		TestSuiteState = 6		-- Deleted
WHERE	TestSuiteID = @ptestSuiteID
AND		TestSuiteState = 5

UPDATE	[Active].[TestJob]
SET		TestJobState = 6		-- Deleted
WHERE	TestSuiteID = @pTestSuiteID
AND		TestJobState = 5

RETURN 0


GO
/****** Object:  StoredProcedure [Active].[TestSuite_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestSuite_Select]
	@pTestSuiteID int = null,
	@pOperatingSystemID int = null
AS

IF (@pTestSuiteID is null)
BEGIN
	SELECT	TestSuiteID
			,TestSuiteName
			,testSuiteDescription
			,(SELECT TestPackageName FROM [Definition].[TestPackage] TP WHERE TS.TestPackageID = TP.TestPackageID)
			,CreateDate
			,StartDate
			,EndDate
			,TestPackageID
			,TestPackageVersionID
			,AddedByUser
			,(SELECT SUM (PassedCount) FROM [Active].[TestJobCommand] TJC WHERE TJC.TestSuiteID = TS.TestSuiteID)
			,(SELECT SUM (WarningCount) FROM [Active].[TestJobCommand] TJC WHERE TJC.TestSuiteID = TS.TestSuiteID)
			,(SELECT SUM (FailedCount) FROM [Active].[TestJobCommand] TJC WHERE TJC.TestSuiteID = TS.TestSuiteID)
			,TestSuiteState
			,MaxRunning
			,StartTime
	FROM	[Active].[TestSuite] TS
	WHERE	TestSuiteState < 6		-- Do not return canceled TestSuites
	AND		(OperatingSystemID is null OR @pOperatingSystemID = OperatingSystemID)
END
ELSE
BEGIN
	SELECT	TestSuiteID
			,TestSuiteName
			,testSuiteDescription
			,(SELECT TestPackageName FROM [Definition].[TestPackage] TP WHERE TestPackageID = TP.TestPackageID)
			,CreateDate
			,StartDate
			,EndDate
			,TestPackageID
			,TestPackageVersionID
			,AddedByUser
			,(SELECT SUM (PassedCount) FROM [Active].[TestJobCommand] TJC WHERE TJC.TestSuiteID = TS.TestSuiteID)
			,(SELECT SUM (WarningCount) FROM [Active].[TestJobCommand] TJC WHERE TJC.TestSuiteID = TS.TestSuiteID)
			,(SELECT SUM (FailedCount) FROM [Active].[TestJobCommand] TJC WHERE TJC.TestSuiteID = TS.TestSuiteID)
			,TestSuiteState
			,MaxRunning
			,StartTime
	FROM	[Active].[TestSuite] TS
	WHERE	TestSuiteID = @pTestSuiteID
	AND		TestSuiteState < 6		-- Do not return canceled TestSuites
	AND		(OperatingSystemID is null OR @pOperatingSystemID = OperatingSystemID)
END

RETURN 0

-- exec active.testsuite_select


GO
/****** Object:  StoredProcedure [Active].[TestSuitePlan_Cancel]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestSuitePlan_Cancel]
	@pTestSuitePlanID int
AS

DECLARE @TestSuiteID int

UPDATE	Active.TestSuitePlan
SET		TestSuiteState = 3
WHERE	TestSuitePlanID = @pTestSuitePlanID

SELECT	@TestSuiteID = TestSuiteID
FROM	Active.TestSuite
WHERE	TestSuitePlanID = @pTestSuitePlanID
AND		StartDate IS NULL

IF (@TestSuiteID IS NOT NULL)
BEGIN
	EXEC Active.TestSuite_Cancel @TestSuiteID
END

RETURN 0

GO
/****** Object:  StoredProcedure [Active].[TestSuitePlan_Create]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestSuitePlan_Create]
	@pTestSuitePlanName varchar(256),
	@pTestSuitePlanDescription varchar(1024),
	@pTestPackageID int,
	@pTestPriority int,
	@pUser varchar(64),
	@pMaxRunning int = null,
	@pOperatingSystemID int = null,
	@oTestSuitePlanID int OUTPUT
AS

INSERT INTO [Active].[TestSuitePlan] (
	[TestSuitePlanName],
	[TestSuitePlanDescription],
	[TestPackageID],
	[TestPriority],
	[CreateDate],
	[AddedByUser],
	[TestSuiteState],
	[MaxRunning],
	[ServerGroupID],
	[OperatingSystemID])
VALUES (
	@pTestSuitePlanName,
	@pTestSuitePlanDescription,
	@pTestPackageID,
	@pTestPriority,
	SYSDATETIME(),
	@pUser,
	1,	-- Queued/Scheduled
	@pMaxRunning,
	1,
	@pOperatingSystemID)

SET @oTestSuitePlanID = @@IDENTITY

RETURN 0





GO
/****** Object:  StoredProcedure [Active].[TestSuitePlan_Disable]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestSuitePlan_Disable]
	@pTestSuitePlanID int
AS

-- Only allow a transition from 1 (enabled/active) to 2 (disabled)
UPDATE	Active.TestSuitePlan
SET		TestSuiteState = 2
WHERE	TestSuitePlanID = @pTestSuitePlanID
AND		TestSuiteState = 1

RETURN 0

GO
/****** Object:  StoredProcedure [Active].[TestSuitePlan_Enable]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestSuitePlan_Enable]
	@pTestSuitePlanID int
AS

-- Only allow a transition from 2 (Disabled) to enabled/active
UPDATE	Active.TestSuitePlan
SET		TestSuiteState = 1
WHERE	TestSuitePlanID = @pTestSuitePlanID
AND		TestSuiteState = 2

RETURN 0

GO
/****** Object:  StoredProcedure [Active].[TestSuitePlan_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestSuitePlan_Select]
AS

SELECT	[TestSuitePlanID]
		,[TestSuitePlanName]
		,[TestSuitePlanDescription]
		,(SELECT TestPackageName FROM Definition.TestPackage TP WHERE TP.TestPackageID = TSP.TestPackageID)
		,TSP.CreateDate
		,AddedByUser
		,(SELECT StateName from Active.TestSuitePlanStateLkp WHERE StateID = TestSuiteState)
FROM	Active.TestSuitePlan TSP
WHERE	TestSuiteState in (1, 2)

RETURN 0

GO
/****** Object:  StoredProcedure [Active].[TestSuiteReport_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Active].[TestSuiteReport_Select]
	@pTestSuiteID int
AS

SELECT	TJC.TestSuiteID,
		TPC.TestPackageCommand,
		SUM(PassedCount),
		SUM(WarningCount),
		SUM(FailedCount),
		COUNT(1),
		TPC.TestPackageCommandID
FROM	Active.TestJobCommand TJC
JOIN	Definition.TestPackageCommand TPC
ON		TJC.TestPackageCommandID = TPC.TestPackageCommandID
WHERE	TestSuiteID = @pTestSuiteID
AND		TJC.StartDate IS NOT NULL
AND		TJC.StartDate != TJC.EndDate
GROUP BY TPC.TestPackageCommand, TJC.TestSuiteID, TPC.TestPackageCommandID
ORDER BY TPC.TestPackageCommandID

RETURN 0

-- EXEC [Active].[TestSuiteReport_Select] 3080

GO
/****** Object:  StoredProcedure [Definition].[Configuration_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Definition].[Configuration_Select]
	@pOperatingSystemID int,
	@pLabID int
AS

-- Return a list of unique configurations for VMs that currently enabled.
SELECT	DISTINCT ConfigurationID,
		ConfigurationName
FROM	[Definition].[Configuration] C
JOIN	[Definition].[VMInstance] V
ON		C.ConfigurationID = V.VMConfigurationID
JOIN	[Definition].[AutomationLabMembership] ALM
ON		V.VMInstanceID = ALM.VMInstanceID
WHERE	OperatingSystemID = @pOperatingSystemID
AND		ALM.AutomationLabID = @pLabID
AND		V.VMState = 1

RETURN 0

--EXEC [Definition].[Configuration_Select] 1, 2

GO
/****** Object:  StoredProcedure [Definition].[DownloadLink_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Definition].[DownloadLink_Select]
	@pOperatingSystemID int = NULL
AS

SELECT	DownloadLinkID,
		DownloadLink,
		DownloadLinkDescription
FROM	[Definition].[DownloadLink]
WHERE	(@pOperatingSystemID IS NULL OR OperatingSystemID = @pOperatingSystemID)

RETURN 0

GO
/****** Object:  StoredProcedure [Definition].[LicenseKey_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Definition].[LicenseKey_Select]
AS

SELECT	LicenseKeyID
		,LicenseKey
		,LicenseKeyName
		,LicenseKeyDescription
FROM	[Definition].[LicenseKey] (NOLOCK)

RETURN 0



GO
/****** Object:  StoredProcedure [Definition].[PeriodicReport_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Definition].[PeriodicReport_Select]
AS

SELECT	PeriodicReportID,
		ScriptPath,
		Recipients,
		EmailHeader,
		EmailBody,
		ScheduleDay,
		ScheduleHour,
		ScheduleMinute,
		PeriodicReportStatus
FROM	[Definition].[PeriodicReport]

RETURN 0

GO
/****** Object:  StoredProcedure [Definition].[TestPackage_Create]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Definition].[TestPackage_Create]
	@pTestPackageName varchar(256),
	@pTestPackageDescription varchar(1024),
	@pTestPackagePath varchar(256),
	@pLicenseKey varchar(32) = null,
	@pServerGroupID int = null,
	@pOperatingSystemID int = null,
	@pLabID int = null,
	@oTestPackageID int OUTPUT,
	@oTestPackageVersionID int OUTPUT
AS

DECLARE @PackageID int
DECLARE @VersionID int
DECLARE @Path varchar(256)

-- Find package by name first
SELECT @PackageID = TestPackageID FROM [Definition].[TestPackage] WHERE TestPackageName = @pTestPackageName
IF (@@ROWCOUNT = 0)
BEGIN
	INSERT INTO [Definition].[TestPackage] (
		[TestPackageName],
		[TestPackageDescription],
		[CreateDate],
		[IsActive],
		[OperatingSystemID],
		[LabID])
	VALUES (
		@pTestPackageName,
		@pTestPackageDescription,
		SYSDATETIME(),
		1,
		(CASE WHEN @pOperatingSystemID IS NULL THEN 1 ELSE @pOperatingSystemID END),
		(CASE WHEN @pLabID IS NULL THEN 2 ELSE @pLabID END));  -- to be removed soon

	set @PackageID = @@IDENTITY
	set @VersionID = 0
END
ELSE
BEGIN
	-- Find current version
	SELECT @VersionID = MAX(TestPackageVersionID) + 1 FROM [Definition].[TestPackageVersion] WHERE TestPackageID = @PackageID
END

SET @Path = @pTestPackagePath + '\' + CONVERT(varchar(10), @VersionID)

INSERT INTO [Definition].[TestPackageVersion] (
	[TestPackageID],
	[TestPackageVersionID],
	[CreateDate],
	[TestPackagePath],
	[LicenseKey],
	[ServerGroupID],
	[LabID])
VALUES (
	@PackageID,
	@VersionID,
	SYSDATETIME(),
	@Path,
	@pLicenseKey,
	@pServerGroupID,
	(CASE WHEN @pLabID IS NULL THEN 2 ELSE @pLabID END))

-- Mark a test package as Active when a new version is uploaded.
UPDATE	[Definition].[TestPackage]
SET		IsActive = 1
WHERE	TestPackageID = @PackageID

SET @oTestPackageID = @PackageID
SET @oTestPackageVersionID = @VersionID

RETURN 0


GO
/****** Object:  StoredProcedure [Definition].[TestPackage_Disable]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Definition].[TestPackage_Disable]
	@pTestPackageID int
AS

UPDATE	[Definition].[TestPackage]
SET		IsActive = 0
WHERE	TestPackageID = @pTestPackageID

RETURN 0


GO
/****** Object:  StoredProcedure [Definition].[TestPackage_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Definition].[TestPackage_Select]
	@pTestPackageID int = NULL,
	@pOperatingSystemID int = NULL,
	@pLabID int
AS

SELECT	TestPackageID
		,TestPackageName
		,TestPackageDescription
		,UserName
		,CreateDate
		,(SELECT MAX(TestPackageVersionID) FROM [Definition].[TestPackageVersion] TPV WHERE TP.TestPackageID = TPV.TestPackageID)
		,(SELECT LicenseKey FROM [Definition].[TestPackageVersion] TPV WHERE TPV.TestPackageVersionID =
			(SELECT MAX(TestPackageVersionID)
			FROM [Definition].[TestPackageVersion] TPV
			WHERE TP.TestPackageID = TPV.TestPackageID)
			AND TPV.TestPackageID = TP.TestPackageID
		)
		,(SELECT MAX(CreateDate) FROM [Definition].[TestPackageVersion] TPV WHERE TP.TestPackageID = TPV.TestPackageID)
		,(SELECT ServerGroupID FROM [Definition].[TestPackageVersion] TPV WHERE TPV.TestPackageVersionID =
			(SELECT MAX(TestPackageVersionID)
			FROM [Definition].[TestPackageVersion] TPV
			WHERE TP.TestPackageID = TPV.TestPackageID)
			AND TPV.TestPackageID = TP.TestPackageID
		) AS ServerGroupID
		, (SELECT AutomationLabName FROM [Definition].[AutomationLabLkp] AL WHERE AL.AutomationLabID = 
		(SELECT LabID FROM [Definition].[TestPackageVersion] TPV WHERE TPV.TestPackageVersionID =
			(SELECT MAX(TestPackageVersionID)
			FROM [Definition].[TestPackageVersion] TPV
			WHERE TP.TestPackageID = TPV.TestPackageID)
			AND TPV.TestPackageID = TP.TestPackageID)
		) AS LabID
		,(SELECT LabID FROM [Definition].[TestPackageVersion] TPV WHERE TPV.TestPackageVersionID =
			(SELECT MAX(TestPackageVersionID)
			FROM [Definition].[TestPackageVersion] TPV
			WHERE TP.TestPackageID = TPV.TestPackageID)
			AND TPV.TestPackageID = TP.TestPackageID)
FROM	[Definition].[TestPackage] TP (NOLOCK)
WHERE	(@pTestPackageID IS NULL OR @pTestPackageID = TestPackageID)
AND		IsActive = 1
AND		(@pOperatingSystemID IS NULL OR OperatingSystemID = @pOperatingSystemID)
AND		(@pLabID IS NULL OR LabID = @pLabID)

RETURN 0


GO
/****** Object:  StoredProcedure [Definition].[TestPackageCommand_Create]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Definition].[TestPackageCommand_Create]
	@pTestPackageID int,
	@pTestCommand varchar(256),
	@pExecutionOrder int
AS

DECLARE @TestPackageVersionID int

SELECT	@TestPackageVersionID = MAX(TestPackageVersionID)
FROM	[Definition].[TestPackageVersion]
WHERE	TestPackageID = @pTestPackageID
GROUP BY TestPackageID

INSERT INTO [Definition].[TestPackageCommand] (
	TestPackageID
	,TestPackageVersionID
	,TestPackageCommand
	,ExecutionOrder
	,TestPackageEntryPointID
	,CreateDate)
VALUES (
	@pTestPackageID
	,@TestPackageVersionID
	,@pTestCommand
	,@pExecutionOrder
	,null
	,SYSDATETIME())

RETURN 0



GO
/****** Object:  StoredProcedure [Definition].[TestPackageVersion_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Definition].[TestPackageVersion_Select]
	@pTestPackageID int
AS

DECLARE @VersionID int

SELECT	@VersionID = MAX(TestPackageVersionID)
FROM	[Definition].[TestPackageVersion] (NOLOCK)
WHERE	TestPackageID = @pTestPackageID

IF (@@ROWCOUNT > 0)
BEGIN
	SELECT	TestPackageID,
			TestPackageVersionID,
			TestPackagePath
	FROM	[Definition].[TestPackageVersion] (NOLOCK)
	WHERE	TestPackageID = @pTestPackageID
	AND		TestPackageVersionID = @VersionID
END

RETURN 0

-- exec [Data].[TestPackageVersion_Select] 1


GO
/****** Object:  StoredProcedure [Definition].[VMInstance_Disable]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Definition].[VMInstance_Disable]
	@pVMInstanceID int
AS

DECLARE @State int
DECLARE @TestJobID int

SELECT	@State = VMState
FROM	Definition.VMInstance
WHERE	VMInstanceID = @pVMInstanceID

IF (@State = 1)
BEGIN
	UPDATE	[Definition].[VMInstance]
	SET		VMState = 0
	WHERE	VMInstanceID = @pVMInstanceID

	/*
	SELECT	@TestJobID = TestJobID
	FROM	[Active].[TestJob]
	WHERE	VMInstanceID = @pVMInstanceID

	IF (@TestJobID is not NULL)
	BEGIN
		UPDATE	[Active].[TestJob]
		SET		VMInstanceID = NULL,
				EndDate = (CASE WHEN EndDate is NULL THEN SYSDATETIME() ELSE EndDate END)
		WHERE	TestJobID = @TestJobID

		-- Cancel the commands that were running on this VM
		UPDATE	[Active].[TestJobCommand]
		SET		EndDate = (CASE WHEN EndDate is NULL THEN SYSDATETIME() ELSE EndDate END)
		WHERE	TestJobID = @TestJobID
	END	
	*/

END

RETURN 0



GO
/****** Object:  StoredProcedure [Definition].[VMInstance_Enable]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Definition].[VMInstance_Enable]
	@pVMInstanceID int
AS

UPDATE	[Definition].[VMInstance]
SET		VMState = 1
WHERE	VMInstanceID = @pVMInstanceID

RETURN 0



GO
/****** Object:  StoredProcedure [Definition].[VMInstance_Inactive]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Definition].[VMInstance_Inactive]
	@pVMInstanceID int,
	@oCommand varchar(64) OUTPUT
AS

DECLARE @Status int

SET @oCommand = ''

SELECT	@Status = VMState
FROM	[Definition].[VMInstance]
WHERE	VMInstanceID = @pVMInstanceID

IF (@Status = 1)
BEGIN
	SET @oCommand = 'rollback=Service'
END

RETURN 0


GO
/****** Object:  StoredProcedure [Definition].[VMInstance_Select]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Definition].[VMInstance_Select]
	@pAutomationLabID	int = NULL,
	@pOperatingSystemID int = NULL,
	@pServerGroupID		int = NULL
AS

-- Make sure the result set is the same as [Definition].[VMInstance_SelectByIP]
SELECT	VI.VMInstanceID
		,VMName
		,HostName
		,IPAddress
		,VMConfigurationID
		,VMState
		,VI.CreateDate
		,AlwaysOn
		,CF.ConfigurationName
		,TJ.TestSuiteID
		,TJ.TestJobID
		,NULL
		,(SELECT TestPackageName FROM Definition.TestPackage TP (nolock) WHERE TP.TestPackageID = TJ.TestPackageID)
		,''
		,[Location]
		,(SELECT AutomationLabName FROM [Definition].[AutomationLabLkp] AL WHERE AL.AutomationLabID =
			(SELECT TOP(1) AutomationLabID FROM Definition.AutomationLabMembership WHERE VMInstanceID = VI.VMInstanceID))
		,LastHeartbeat
		,OperatingSystemID
FROM	[Definition].[VMInstance] VI WITH (NOLOCK)
JOIN	[Definition].[Configuration] CF
ON		CF.ConfigurationID = VI.VMConfigurationID
LEFT JOIN	[Active].[TestJob] TJ
ON		TJ.VMInstanceID = VI.VMInstanceID
WHERE	(@pOperatingSystemID IS NULL OR @pOperatingSystemID = CF.OperatingSystemID)
AND		(@pServerGroupID IS NULL OR EXISTS(
			SELECT 1 FROM Definition.ServerGroupMembership SGM
			WHERE SGM.ServerGroupID = @pServerGroupID AND SGM.VMInstanceID = VI.VMInstanceID))
ORDER BY ConfigurationName

RETURN 0

GO
/****** Object:  StoredProcedure [Definition].[VMInstance_SelectByIP]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Definition].[VMInstance_SelectByIP]
	@pIPAddress varchar(24)
AS

-- Make sure the result set is the same as [Definition].[VMInstance_Select]
SELECT	VI.VMInstanceID
		,VMName
		,HostName
		,IPAddress
		,VMConfigurationID
		,VMState
		,VI.CreateDate
		,AlwaysOn
		,CF.ConfigurationName
		,TJ.TestSuiteID
		,TJ.TestJobID
		,NULL
		,(SELECT TestPackageName FROM Definition.TestPackage TP (nolock) WHERE TP.TestPackageID = TJ.TestPackageID)
		,''
		,[Location]
		,(SELECT AutomationLabName FROM [Definition].[AutomationLabLkp] AL WHERE AL.AutomationLabID =
			(SELECT TOP(1) AutomationLabID FROM Definition.AutomationLabMembership WHERE VMInstanceID = VI.VMInstanceID))
		,LastHeartbeat
		,OperatingSystemID
FROM	[Definition].[VMInstance] VI WITH (NOLOCK)
JOIN	[Definition].[Configuration] CF
ON		CF.ConfigurationID = VI.VMConfigurationID
LEFT JOIN	[Active].[TestJob] TJ
ON		TJ.VMInstanceID = VI.VMInstanceID
WHERE	IPAddress = @pIpAddress

RETURN 0

-- EXEC [Definition].[VMInstance_SelectByIP] '10.0.0.1'


GO
/****** Object:  StoredProcedure [Definition].[VMInstance_SetAutomationLab]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Definition].[VMInstance_SetAutomationLab]
	@pVMInstanceID int,
	@pAutomationLabID int
AS

MERGE INTO [Definition].[AutomationLabMembership] Target
USING (VALUES(@pVMInstanceID)) Source (VMInstanceID)
ON Target.VMInstanceID = Source.VMInstanceID
WHEN NOT MATCHED THEN
	INSERT(VMInstanceID, AutomationLabID, CreateDate) VALUES (@pVMInstanceID, @pAutomationLabID, SYSDATETIME())
WHEN MATCHED THEN
	UPDATE SET Target.AutomationLabID = @pAutomationLabID;

RETURN 0

GO
/****** Object:  StoredProcedure [Definition].[VMInstance_Start]    Script Date: 3/6/2018 2:14:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Definition].[VMInstance_Start]
	@pVMInstanceID int
AS

DECLARE @Entry varchar(1024)
DECLARE @TestJobID int
DECLARE @TestJobCommandID int
DECLARE @TestOrder int
DECLARE @Count int

-- Count the number of running TestJobCommand entries that are assigned to this VM
-- A running TestJobCommand can be identifed by a having a StartDate that is not null, and
-- an EndDate that is still null
SELECT	@Count = COUNT(1)
FROM	Active.TestJobCommand
WHERE	StartDate is not null
AND		EndDate is null
AND		VMInstanceID = @pVMInstanceID

-- If more than 1 TestJobCommand was assigned to a VM, then there must be an issue with the code, so log it.
IF (@Count > 1)
BEGIN
	SELECT @Entry = 'More than one TestJobCommand assigned to VM ' + CONVERT(varchar(10), @pVMInstanceID)
	EXEC Active.Log_Insert @Entry, NULL, 1
	RETURN 0
END

-- This is a scneario where the VM restarted unexpectedly so mark the currently running TestJobCommand as failed.
IF (@Count = 1)
BEGIN
	SELECT	@TestOrder = TPC.ExecutionOrder,
			@TestJobID = TJC.TestJobID,
			@TestJobCommandID = TJC.TestJobCommandID
	FROM	Active.TestJobCommand TJC
	JOIN	Definition.TestPackageCommand TPC
	ON		TJC.TestPackageCommandID = TPC.TestPackageCommandID
	WHERE	StartDate is not null
	AND		EndDate is null
	AND		VMInstanceID = @pVMInstanceID

	SELECT @Entry = 'Resetting TestJob ' + CONVERT(varchar(10), @TestJobID) + ' on VM ' + CONVERT(varchar(10), @pVMInstanceID)
	EXEC Active.Log_Insert @Entry, NULL, 1

	EXEC Active.TestJob_Cancel @TestJobID, null, 'Cancelled due to VM start'

	UPDATE	Active.TestJobCommand
	SET		EndDate = SYSDATETIME()
	WHERE	TestJobCommandID = @TestJobCommandID
END

RETURN 0

-- exec [Definition].[VMInstance_Start] 11
GO
