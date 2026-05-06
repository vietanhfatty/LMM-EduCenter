namespace Server.Models;

public enum ClassStatus
{
    Upcoming = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public enum EnrollmentStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3
}

public enum AttendanceStatus
{
    Present = 0,
    Absent = 1,
    Late = 2,
    Excused = 3
}

public enum GradeType
{
    Quiz = 0,
    Midterm = 1,
    Final = 2,
    Assignment = 3,
    Participation = 4
}

public enum PaymentMethod
{
    Cash = 0,
    BankTransfer = 1,
    Card = 2
}

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Refunded = 2
}

public enum NotificationTargetType
{
    All = 0,
    Role = 1,
    Class = 2,
    User = 3
}
