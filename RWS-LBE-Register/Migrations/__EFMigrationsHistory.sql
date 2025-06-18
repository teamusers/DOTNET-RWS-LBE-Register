USE [LBE]
GO

CREATE TABLE [dbo].[__EFMigrationsHistory] (
    [MigrationId] NVARCHAR(150) NOT NULL PRIMARY KEY,
    [ProductVersion] NVARCHAR(32) NOT NULL
);



INSERT INTO [dbo].[__EFMigrationsHistory]
           ([MigrationId]
           ,[ProductVersion])
     VALUES
           ('20250616015143_InitialCreate',
           '9.0.6') 

INSERT INTO [dbo].[__EFMigrationsHistory]
           ([MigrationId]
           ,[ProductVersion])
     VALUES
           ('20250616020931_AddMissingChanges',
           '9.0.6') 

INSERT INTO [dbo].[__EFMigrationsHistory]
           ([MigrationId]
           ,[ProductVersion])
     VALUES
           ('20250616030249_AddMissingChanges',
           '9.0.6') 