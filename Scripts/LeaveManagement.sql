-- Create database if not exists
IF DB_ID('hr_leavemanagement_db') IS NULL
BEGIN
    CREATE DATABASE hr_leavemanagement_db;
END
GO

-- Use the newly created database
USE hr_leavemanagement_db_Identity;
GO

-- Begin transaction
BEGIN TRANSACTION;

-- Create LeaveTypes table
CREATE TABLE [LeaveTypes] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(MAX) NULL,
    [DefaultDays] INT NOT NULL,
    [DateCreated] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(MAX) NULL,
    [LastModifiedDate] DATETIME2 NOT NULL,
    [LastModifiedBy] NVARCHAR(MAX) NULL
);

-- Create LeaveAllocations table
CREATE TABLE [LeaveAllocations] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [NumberOfDays] INT NOT NULL,
    [LeaveTypeId] INT NOT NULL,
    [Period] INT NOT NULL,
    [EmployeeId] NVARCHAR(MAX) NULL,
    [DateCreated] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(MAX) NULL,
    [LastModifiedDate] DATETIME2 NOT NULL,
    [LastModifiedBy] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_LeaveAllocations_LeaveTypes_LeaveTypeId] FOREIGN KEY ([LeaveTypeId]) REFERENCES [LeaveTypes] ([Id]) ON DELETE CASCADE
);

-- Create LeaveRequests table
CREATE TABLE [LeaveRequests] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NOT NULL,
    [LeaveTypeId] INT NOT NULL,
    [DateRequested] DATETIME2 NOT NULL,
    [RequestComments] NVARCHAR(MAX) NULL,
    [DateActioned] DATETIME2 NULL,
    [Approved] BIT NULL,
    [Cancelled] BIT NOT NULL,
    [RequestingEmployeeId] NVARCHAR(MAX) NULL,
    [DateCreated] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(MAX) NULL,
    [LastModifiedDate] DATETIME2 NOT NULL,
    [LastModifiedBy] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_LeaveRequests_LeaveTypes_LeaveTypeId] FOREIGN KEY ([LeaveTypeId]) REFERENCES [LeaveTypes] ([Id]) ON DELETE CASCADE
);

-- Insert data into LeaveTypes table
INSERT INTO [LeaveTypes] ([Name], [DefaultDays], [DateCreated], [LastModifiedDate])
VALUES ('Vacation', 10, '0001-01-01T00:00:00', '0001-01-01T00:00:00');

INSERT INTO [LeaveTypes] ([Name], [DefaultDays], [DateCreated], [LastModifiedDate])
VALUES ('Sick', 12, '0001-01-01T00:00:00', '0001-01-01T00:00:00');

-- Create indexes
CREATE INDEX [IX_LeaveAllocations_LeaveTypeId] ON [LeaveAllocations] ([LeaveTypeId]);
CREATE INDEX [IX_LeaveRequests_LeaveTypeId] ON [LeaveRequests] ([LeaveTypeId]);

-- Commit transaction
COMMIT;
GO
