/*
Run this script on SQL Server for the new "Gửi tài liệu học" feature.
It is safe to run once on existing databases.
*/

IF OBJECT_ID(N'[dbo].[LearningMaterials]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[LearningMaterials]
    (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [ClassId] INT NOT NULL,
        [TeacherId] NVARCHAR(450) NOT NULL,
        [Title] NVARCHAR(300) NOT NULL,
        [Description] NVARCHAR(2000) NULL,
        [FilePath] NVARCHAR(500) NOT NULL,
        [OriginalFileName] NVARCHAR(255) NOT NULL,
        [FileSize] BIGINT NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_LearningMaterials_CreatedAt] DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_LearningMaterials] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LearningMaterials_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Classes]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_LearningMaterials_AspNetUsers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE NO ACTION
    );

    CREATE NONCLUSTERED INDEX [IX_LearningMaterials_ClassId]
        ON [dbo].[LearningMaterials]([ClassId]);

    CREATE NONCLUSTERED INDEX [IX_LearningMaterials_TeacherId]
        ON [dbo].[LearningMaterials]([TeacherId]);
END
GO
