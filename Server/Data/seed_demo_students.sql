SET NOCOUNT ON;

DECLARE @StudentRoleId nvarchar(450);
DECLARE @TemplatePasswordHash nvarchar(max);
DECLARE @TemplateSecurityStamp nvarchar(max);
DECLARE @TemplateLockoutEnabled bit;

SELECT @StudentRoleId = [Id]
FROM [AspNetRoles]
WHERE [NormalizedName] = N'STUDENT';

IF @StudentRoleId IS NULL
BEGIN
    RAISERROR('Role STUDENT not found. Run app seeder first.', 16, 1);
    RETURN;
END

SELECT TOP (1)
    @TemplatePasswordHash = [PasswordHash],
    @TemplateSecurityStamp = [SecurityStamp],
    @TemplateLockoutEnabled = [LockoutEnabled]
FROM [AspNetUsers]
WHERE [NormalizedEmail] = N'STUDENT@LMM.COM';

IF @TemplatePasswordHash IS NULL
BEGIN
    RAISERROR('Template account student@lmm.com not found. Create it first.', 16, 1);
    RETURN;
END

DECLARE @i int = 1;
WHILE @i <= 30
BEGIN
    DECLARE @Index varchar(2) = RIGHT('00' + CAST(@i AS varchar(2)), 2);
    DECLARE @Email nvarchar(256) = N'student' + @Index + N'@lmm.com';
    DECLARE @NormalizedEmail nvarchar(256) = UPPER(@Email);
    DECLARE @Id nvarchar(450) = CONVERT(nvarchar(36), NEWID());

    IF NOT EXISTS (
        SELECT 1
        FROM [AspNetUsers]
        WHERE [NormalizedEmail] = @NormalizedEmail
    )
    BEGIN
        INSERT INTO [AspNetUsers]
        (
            [Id],
            [FullName],
            [Phone],
            [Avatar],
            [DateOfBirth],
            [Address],
            [IsActive],
            [CreatedAt],
            [UserName],
            [NormalizedUserName],
            [Email],
            [NormalizedEmail],
            [EmailConfirmed],
            [PasswordHash],
            [SecurityStamp],
            [ConcurrencyStamp],
            [PhoneNumber],
            [PhoneNumberConfirmed],
            [TwoFactorEnabled],
            [LockoutEnd],
            [LockoutEnabled],
            [AccessFailedCount]
        )
        VALUES
        (
            @Id,
            N'Demo Student ' + @Index,
            N'090000' + RIGHT('0000' + CAST(@i AS varchar(4)), 4),
            NULL,
            NULL,
            N'Ho Chi Minh City',
            1,
            GETUTCDATE(),
            @Email,
            @NormalizedEmail,
            @Email,
            @NormalizedEmail,
            1,
            @TemplatePasswordHash,
            @TemplateSecurityStamp,
            CONVERT(nvarchar(36), NEWID()),
            NULL,
            0,
            0,
            NULL,
            ISNULL(@TemplateLockoutEnabled, 1),
            0
        );

        INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
        VALUES (@Id, @StudentRoleId);
    END

    SET @i += 1;
END

PRINT 'Seeded demo students: student01@lmm.com -> student30@lmm.com';
PRINT 'Password (same as template student@lmm.com): Student@123';
