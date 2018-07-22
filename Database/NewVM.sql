-- One-shot script to set up new VM
-- Please make YOUR OWN COPY

-- VARIABLES
DECLARE @VM_CONFIG_ID AS INT =					NULL			-- LEAVE NULL to create new config; otherwise, fill in config to use
	-- CONFIG VARS - Only needed if creating new configuration
DECLARE @CONFIG_NAME AS VARCHAR(64) =			'CONFIG NAME'	-- Configuration Name - Appears in ATGUI
DECLARE @CONFIG_DESCRIPTION AS VARCHAR(1024) =	'CONFIG DESC'	-- Configuration Description - Usually same as CONFIG_NAME
DECLARE @WINDOWS_VERSION AS INT =				7				-- 7, 8, 10, 2008, 2012, 2016
DECLARE @IS_64_BIT AS BIT =						0				-- 0 : 1 :: x86 : x64
DECLARE @IS_SERVER AS BIT =						0				-- 0 : 1 :: Desktop : Server
	-- END CONFIG VARS
DECLARE @VMNAME AS VARCHAR(32) =				'testuser'		-- Name of the VM - Usually just same as Hostname
DECLARE @HOSTNAME AS VARCHAR(32) =				'testuser'		-- Hostname of the VM
DECLARE @IP_ADDR AS VARCHAR(16) =				'10.0.0.1'		-- IP Address of the VM
DECLARE @LAB_ID AS INT =						2				-- 1: Production, 2: Development, 3: Test

-- VARIABLES YOU RARELY CHANGE
DECLARE @OS_ID AS INT =							1				-- 1: Win, 2: Mac
DECLARE @VM_STATE AS BIT =						1				-- Should usually be 1
DECLARE @ALWAYS_ON AS BIT =						0				-- Should usually be 0
DECLARE @USERNAME AS VARCHAR(64) =				'user'		-- Your username
DECLARE @SERVER_GROUP_ID AS INT =				1				-- (From [Definition].[ServerGroupLkp]) - 1: Consumer. Other options probably(?) not used anymore

-- VARIABLES YOU SHOULDN'T CHANGE
DECLARE @VM_INSTANCE_ID AS INT =				NULL			-- Auto, primary key after adding to VMInstance
DECLARE @RUNNING_TEST_JOB_ID AS INT =			NULL			-- Always NULL when creating new row
DECLARE @VM_LOCATION AS VARCHAR(32) =			'user'			-- VMWare Pool
DECLARE @LAST_HEARTBEAT AS DATETIME =			NULL			-- Always NULL when creating new row

--TMP VAR TABLE
CREATE TABLE #TempVars (CONFIG_ID INT IDENTITY(1,1) PRIMARY KEY,
	VM_CONFIG_ID INT FOREIGN KEY REFERENCES [Definition].[Configuration],
	VM_INSTANCE_ID INT FOREIGN KEY REFERENCES [Definition].[VMInstance],
	VMNAME VARCHAR(32),
	HOSTNAME VARCHAR(32),
	IP_ADDR VARCHAR(16),
	VM_STATE BIT,
	ALWAYS_ON BIT,
	USERNAME VARCHAR(64),
	RUNNING_TEST_JOB_ID INT,
	VM_LOCATION VARCHAR(32),
	LAST_HEARTBEAT DATETIME,
	SERVER_GROUP_ID INT,
	LAB_ID INT,
)

INSERT INTO #TempVars (VM_CONFIG_ID, VM_INSTANCE_ID, VMNAME, HOSTNAME, IP_ADDR, VM_STATE, ALWAYS_ON, 
	USERNAME, RUNNING_TEST_JOB_ID, VM_LOCATION, LAST_HEARTBEAT, SERVER_GROUP_ID, LAB_ID)
	VALUES(@VM_CONFIG_ID, @VM_INSTANCE_ID, @VMNAME, @HOSTNAME, @IP_ADDR, @VM_STATE, @ALWAYS_ON, 
		@USERNAME, @RUNNING_TEST_JOB_ID, @VM_LOCATION, @LAST_HEARTBEAT,@SERVER_GROUP_ID, @LAB_ID)

-- If Config ID hasn't been set, assume it needs to be created
IF @VM_CONFIG_ID IS NOT NULL
BEGIN
	SET NOEXEC ON
END
	INSERT INTO [Definition].[Configuration] (ConfigurationName, ConfigurationDescription, WindowsVersion, Is64Bit, IsServer, CreateDate, OperatingSystemID)
		VALUES (@CONFIG_NAME, @CONFIG_DESCRIPTION, @WINDOWS_VERSION, @IS_64_BIT, @IS_SERVER, GETDATE(), 1)
	GO
UPDATE #TempVars SET VM_CONFIG_ID = @@IDENTITY WHERE CONFIG_ID = 1
SET NOEXEC OFF
GO

-- VMInstance
INSERT INTO [Definition].[VMInstance] (VMName, HostName, IPAddress, VMConfigurationID, VMState, AlwaysOn, CreateDate, AddedByUser, RunningTestJobID, Location, LastHeartbeat)
	SELECT VMNAME, HOSTNAME, IP_ADDR, VM_CONFIG_ID, VM_STATE, ALWAYS_ON, GETDATE(), USERNAME, RUNNING_TEST_JOB_ID, VM_LOCATION, LAST_HEARTBEAT
	FROM #TempVars
	WHERE CONFIG_ID = 1
GO

UPDATE #TempVars SET VM_INSTANCE_ID = @@IDENTITY WHERE CONFIG_ID = 1

-- ServerGroupMembership
INSERT INTO [Definition].[ServerGroupMembership] (VMInstanceID, ServerGroupID, CreateDate)
	SELECT VM_INSTANCE_ID, SERVER_GROUP_ID, GETDATE()
	FROM #TempVars
	WHERE CONFIG_ID = 1
GO


-- AutomationLab
INSERT INTO [Definition].[AutomationLabMembership] (VMInstanceID, AutomationLabID, CreateDate)
	SELECT VM_INSTANCE_ID, LAB_ID, GETDATE()
	FROM #TempVars
	WHERE CONFIG_ID = 1
GO


-- Cleanup
DROP TABLE #TempVars
GO
