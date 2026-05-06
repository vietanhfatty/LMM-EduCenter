/*
Reset all old class schedules and recreate by fixed hard slots.
Hard slots:
  1) 08:30 - 10:30
  2) 13:30 - 15:30
  3) 16:30 - 18:30
  4) 19:00 - 21:00
*/

USE [LMMEducation];
GO

BEGIN TRY
    BEGIN TRAN;

    -- 1) Remove attendance records first to avoid old-date inconsistencies with new schedule
    DELETE FROM [Attendances];

    -- 2) Clear all old schedules
    DELETE FROM [ClassSchedules];

    ;WITH ClassSeed AS
    (
        SELECT
            c.Id AS ClassId,
            ROW_NUMBER() OVER (ORDER BY c.Id) AS rn
        FROM [Classes] c
    )
    INSERT INTO [ClassSchedules] ([ClassId], [DayOfWeek], [StartTime], [EndTime])
    SELECT cs.ClassId,
           d.DayOfWeek,
           p.StartTime,
           p.EndTime
    FROM ClassSeed cs
    CROSS APPLY
    (
        -- Rotate slots by class index to distribute classes
        SELECT
            CASE ((cs.rn - 1) % 4) + 1
                WHEN 1 THEN CAST('08:30:00' AS time)
                WHEN 2 THEN CAST('13:30:00' AS time)
                WHEN 3 THEN CAST('16:30:00' AS time)
                ELSE CAST('19:00:00' AS time)
            END AS StartTime,
            CASE ((cs.rn - 1) % 4) + 1
                WHEN 1 THEN CAST('10:30:00' AS time)
                WHEN 2 THEN CAST('15:30:00' AS time)
                WHEN 3 THEN CAST('18:30:00' AS time)
                ELSE CAST('21:00:00' AS time)
            END AS EndTime
    ) p
    CROSS APPLY
    (
        -- Alternate weekdays pattern by class index
        SELECT v.DayOfWeek
        FROM (VALUES
            (CASE WHEN ((cs.rn - 1) % 2) = 0 THEN 1 ELSE 2 END), -- T2 or T3
            (CASE WHEN ((cs.rn - 1) % 2) = 0 THEN 3 ELSE 4 END), -- T4 or T5
            (CASE WHEN ((cs.rn - 1) % 2) = 0 THEN 5 ELSE 6 END)  -- T6 or T7
        ) v(DayOfWeek)
    ) d;

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRAN;
    THROW;
END CATCH;
GO

-- Quick check output
SELECT c.Id, c.Name, s.DayOfWeek, s.StartTime, s.EndTime
FROM [Classes] c
JOIN [ClassSchedules] s ON s.ClassId = c.Id
ORDER BY c.Id, s.DayOfWeek, s.StartTime;
GO
