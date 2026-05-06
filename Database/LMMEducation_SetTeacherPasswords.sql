USE [LMMEducation];
GO

SET NOCOUNT ON;

PRINT '=== SET PASSWORD FOR TEACHER ACCOUNTS ===';

DECLARE @TeacherRoleId NVARCHAR(450) = (
    SELECT TOP 1 [Id]
    FROM [AspNetRoles]
    WHERE [NormalizedName] = N'TEACHER'
);

IF @TeacherRoleId IS NULL
BEGIN
    THROW 51001, 'Khong tim thay role TEACHER.', 1;
END;

DECLARE @TemplateHash NVARCHAR(MAX) = (
    SELECT TOP 1 [PasswordHash]
    FROM [AspNetUsers]
    WHERE [NormalizedEmail] = N'TEACHER@LMM.COM'
      AND [PasswordHash] IS NOT NULL
);

IF @TemplateHash IS NULL
BEGIN
    THROW 51002, 'Khong tim thay PasswordHash cua teacher@lmm.com. Hay tao tai khoan mau qua DbSeeder truoc.', 1;
END;

UPDATE u
SET
    u.[PasswordHash] = @TemplateHash,
    u.[SecurityStamp] = CONVERT(NVARCHAR(36), NEWID()),
    u.[ConcurrencyStamp] = CONVERT(NVARCHAR(36), NEWID()),
    u.[EmailConfirmed] = 1,
    u.[IsActive] = 1
FROM [AspNetUsers] u
INNER JOIN [AspNetUserRoles] ur ON ur.[UserId] = u.[Id]
WHERE ur.[RoleId] = @TeacherRoleId
  AND u.[NormalizedEmail] LIKE N'TEACHER%@LMM.COM';

PRINT 'Da cap nhat mat khau cho cac tai khoan teacher%@lmm.com.';
PRINT 'Mat khau dang dung: Teacher@123 (cung hash voi teacher@lmm.com).';
PRINT '=== DONE ===';
GO
