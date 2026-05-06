# LMM Education - Tong hop tien do phien lam viec

> Cap nhat: 21/04/2026

---

## 1. Nhung gi da lam

### Step 0: Chuan bi nen tang (HOAN THANH)

| Viec | Chi tiet |
|------|---------|
| Xoa 7 file scaffold cu | `LmmeducationContext.cs`, `AspNetUser.cs`, `AspNetRole.cs`, `AspNetRoleClaim.cs`, `AspNetUserClaim.cs`, `AspNetUserLogin.cs`, `AspNetUserToken.cs` |
| Sua 8 domain models | Thay `AspNetUser` → `AppUser` trong: Class, Enrollment, Attendance, Grade, Payment, Notification, NotificationRead, TeacherReview |
| Them 12 DbSet vao LMMDbContext | Course, Subject, Room, Class, ClassSchedule, Enrollment, Attendance, Grade, Payment, Notification, NotificationRead, TeacherReview |
| Tao Migration | `AddDomainEntities` — tao 12 bang domain |
| Seed 3 test users | admin@lmm.com (Staff), teacher@lmm.com (Teacher), student@lmm.com (Student) |
| Fix login redirect | Goi `RedirectByRole()` thay vi redirect ve Profile |
| Tao BaseApiClient | Generic GET/POST/PUT/DELETE, refactor AuthApiClient ke thua |
| Tao Enums.cs | ClassStatus, EnrollmentStatus, AttendanceStatus, GradeType, PaymentMethod, PaymentStatus, NotificationTargetType |

### Step 1: Shared Components - Layout & Navigation (HOAN THANH)

| File | Mo ta |
|------|-------|
| `_LoginPartial.cshtml` | Avatar + ten user + role + dropdown |
| `_StaffNav.cshtml` | 10 menu items (them Bao cao) |
| `_TeacherNav.cshtml` | 6 menu items |
| `_StudentNav.cshtml` | 9 menu items (them Danh gia GV) |
| `_Layout.cshtml` | Dark navbar + sidebar + TempData alerts + SignalR |
| `BaseController.cs` | GetToken(), GetUserId(), GetUserRole() + `[RequireRole]` attribute |

### Step 2: Staff - Khoa hoc & Phong hoc CRUD (HOAN THANH)

| Server | Client |
|--------|--------|
| `CoursesController` (5 endpoints) | `StaffCourseController` + 4 Views |
| `RoomsController` (5 endpoints) | `StaffRoomController` + 3 Views |

### Step 3: Staff - Giao vien & Hoc vien (HOAN THANH)

| Server | Client |
|--------|--------|
| `TeachersController` (5 endpoints) | `StaffTeacherController` + 4 Views |
| `StudentsController` (6 endpoints) | `StaffStudentController` + 4 Views |

### Step 4: Staff - Lop hoc (HOAN THANH)

| Server | Client |
|--------|--------|
| `ClassesController` (6 endpoints) | `StaffClassController` + 4 Views |

### Step 5: Staff Dashboard - Data thuc (HOAN THANH)

| Server | Client |
|--------|--------|
| `ReportsController` — GET /api/reports/dashboard, revenue, enrollment-stats | `StaffDashboardController` + View (7 card thong ke thuc) |
| `DashboardDto`, `RevenueReportDto` | `IReportApiClient` + `ReportApiClient` |

### Step 6: Student - Dang ky hoc + Xem lich (HOAN THANH)

| Server | Client |
|--------|--------|
| `EnrollmentsController` — GET /my, POST, POST /{id}/cancel | `StudentCourseController` + 2 Views (khoa hoc + DS lop) |
| `EnrollmentDto`, `CreateEnrollmentDto` | `StudentEnrollmentController` + View (DK cua toi + Huy) |
| | `StudentScheduleController` + View (lich hoc bang) |
| | `IEnrollmentApiClient` + `EnrollmentApiClient` |

### Step 7: Staff - Duyet dang ky (HOAN THANH)

| Server | Client |
|--------|--------|
| GET ?status=, POST /{id}/approve, POST /{id}/reject | `StaffEnrollmentController` + 2 Views (DS + filter + Duyet/Tu choi) |

### Step 8: Teacher - Xem lop + Diem danh (HOAN THANH)

| Server | Client |
|--------|--------|
| `AttendancesController` — GET, GET /my, POST /batch, GET /report/{classId}, GET /dates/{classId} | `TeacherClassController` + 2 Views (DS lop + chi tiet) |
| `AttendanceDto`, `BatchAttendanceDto`, `AttendanceReportDto` | `TeacherScheduleController` + View (lich day bang) |
| | `TeacherAttendanceController` + 3 Views (chon lop, form diem danh, bao cao) |
| | `IAttendanceApiClient` + `AttendanceApiClient` |

### Step 9: Nhap/Xem diem (HOAN THANH)

| Server | Client |
|--------|--------|
| `GradesController` — GET, GET /my, POST /batch, PUT /{id} | `TeacherGradeController` + 3 Views (chon lop, nhap diem, bang diem) |
| `GradeDto`, `BatchGradeDto`, `StudentGradeSummaryDto` | `StudentGradeController` + View (diem tong hop theo lop) |
| | `StudentAttendanceController` + View (diem danh ca nhan + chon lop) |
| | `IGradeApiClient` + `GradeApiClient` |

### Step 10: Quan ly Hoc phi (HOAN THANH)

| Server | Client |
|--------|--------|
| `PaymentsController` — GET, GET /my, POST, GET /debts, GET /my/debts | `StaffPaymentController` + 4 Views (DS, ghi nhan, cong no, doanh thu) |
| `PaymentDto`, `CreatePaymentDto`, `DebtDto` | `StudentPaymentController` + View (lich su + cong no) |
| | `IPaymentApiClient` + `PaymentApiClient` |

### Step 11: Thong bao + SignalR (HOAN THANH)

| Server | Client |
|--------|--------|
| `NotificationsController` — GET, GET/{id}, POST, POST/{id}/read, GET/unread-count | `StaffNotificationController` + 2 Views (DS + gui) |
| `NotificationDto`, `CreateNotificationDto` | `TeacherNotificationController` + 2 Views (DS + gui) |
| SignalR push khi tao thong bao moi | `StudentNotificationController` + 2 Views (DS + chi tiet + mark read) |
| | `INotificationApiClient` + `NotificationApiClient` |

### Step 12: Bao cao + Danh gia GV (HOAN THANH)

| Server | Client |
|--------|--------|
| `ReportsController` — GET /revenue, GET /enrollment-stats | `StaffReportController` + 3 Views (trang bao cao, doanh thu, chuyen can) |
| `ReviewsController` — GET, GET /my, POST | `StudentReviewController` + 2 Views (DS danh gia + gui moi) |
| `ReviewDto`, `CreateReviewDto` | `IReviewApiClient` + `ReviewApiClient` |

---

## 2. Tien do tong the

```
Step 0:  Nen tang DB        ████████████████████ 100% ✅
Step 1:  Shared Layout      ████████████████████ 100% ✅
Step 2:  Khoa hoc + Phong   ████████████████████ 100% ✅
Step 3:  Giao vien + HV     ████████████████████ 100% ✅
Step 4:  Lop hoc            ████████████████████ 100% ✅
Step 5:  Staff Dashboard    ████████████████████ 100% ✅
Step 6:  Student DK hoc     ████████████████████ 100% ✅
Step 7:  Staff duyet DK     ████████████████████ 100% ✅
Step 8:  Teacher diem danh  ████████████████████ 100% ✅
Step 9:  Nhap/Xem diem      ████████████████████ 100% ✅
Step 10: Hoc phi            ████████████████████ 100% ✅
Step 11: Thong bao          ████████████████████ 100% ✅
Step 12: Bao cao + Danh gia ████████████████████ 100% ✅
──────────────────────────────────────────────────────
Tong the:                   ████████████████████ 100% ✅
```

---

## 3. Tong hop file da tao/sua

### Server (26 file moi + 11 file sua)

```
Server/
├── Controllers/
│   ├── AuthController.cs            (da co)
│   ├── CoursesController.cs         ✅ NEW
│   ├── RoomsController.cs           ✅ NEW
│   ├── TeachersController.cs        ✅ NEW
│   ├── StudentsController.cs        ✅ NEW
│   ├── ClassesController.cs         ✅ NEW
│   ├── ReportsController.cs         ✅ NEW (dashboard + revenue + enrollment-stats)
│   ├── EnrollmentsController.cs     ✅ NEW (CRUD + approve/reject/cancel)
│   ├── AttendancesController.cs     ✅ NEW (batch + report + dates)
│   ├── GradesController.cs          ✅ NEW (batch + my + update)
│   ├── PaymentsController.cs        ✅ NEW (CRUD + debts)
│   ├── NotificationsController.cs   ✅ NEW (CRUD + read + unread-count + SignalR)
│   └── ReviewsController.cs         ✅ NEW (CRUD)
├── DTOs/
│   ├── Auth/ (da co)
│   ├── Course/CourseDto.cs          ✅ NEW
│   ├── Room/RoomDto.cs              ✅ NEW
│   ├── User/UserDto.cs              ✅ NEW
│   ├── Class/ClassDto.cs            ✅ NEW
│   ├── Report/DashboardDto.cs       ✅ NEW (Dashboard + Revenue)
│   ├── Enrollment/EnrollmentDto.cs  ✅ NEW
│   ├── Attendance/AttendanceDto.cs  ✅ NEW
│   ├── Grade/GradeDto.cs            ✅ NEW
│   ├── Payment/PaymentDto.cs        ✅ NEW
│   ├── Notification/NotificationDto.cs ✅ NEW
│   └── Review/ReviewDto.cs          ✅ NEW
├── Models/ (8 file sua AspNetUser → AppUser)
├── Data/
│   ├── LMMDbContext.cs              ✏️  SUA
│   ├── DbSeeder.cs                  ✏️  SUA
│   └── Migrations/
└── Hubs/NotificationHub.cs          (da co)
```

### Client (84 file moi + 5 file sua)

```
Client/
├── Controllers/
│   ├── BaseController.cs              ✅ NEW
│   ├── AuthController.cs              ✏️  SUA
│   ├── StaffDashboardController.cs    ✅ NEW
│   ├── StaffCourseController.cs       ✅ NEW
│   ├── StaffRoomController.cs         ✅ NEW
│   ├── StaffTeacherController.cs      ✅ NEW
│   ├── StaffStudentController.cs      ✅ NEW
│   ├── StaffClassController.cs        ✅ NEW
│   ├── StaffEnrollmentController.cs   ✅ NEW
│   ├── StaffPaymentController.cs      ✅ NEW
│   ├── StaffNotificationController.cs ✅ NEW
│   ├── StaffReportController.cs       ✅ NEW
│   ├── TeacherDashboardController.cs  ✅ NEW
│   ├── TeacherClassController.cs      ✅ NEW
│   ├── TeacherScheduleController.cs   ✅ NEW
│   ├── TeacherAttendanceController.cs ✅ NEW
│   ├── TeacherGradeController.cs      ✅ NEW
│   ├── TeacherNotificationController.cs ✅ NEW
│   ├── StudentDashboardController.cs  ✅ NEW
│   ├── StudentCourseController.cs     ✅ NEW
│   ├── StudentEnrollmentController.cs ✅ NEW
│   ├── StudentScheduleController.cs   ✅ NEW
│   ├── StudentAttendanceController.cs ✅ NEW
│   ├── StudentGradeController.cs      ✅ NEW
│   ├── StudentPaymentController.cs    ✅ NEW
│   ├── StudentNotificationController.cs ✅ NEW
│   ├── StudentReviewController.cs     ✅ NEW
│   └── HomeController.cs             (da co)
├── Services/
│   ├── BaseApiClient.cs               ✅ NEW
│   ├── AuthApiClient.cs               ✏️  SUA
│   ├── ICourseApiClient.cs + CourseApiClient.cs       ✅ NEW
│   ├── IRoomApiClient.cs + RoomApiClient.cs           ✅ NEW
│   ├── IUserApiClient.cs + UserApiClient.cs           ✅ NEW
│   ├── IClassApiClient.cs + ClassApiClient.cs         ✅ NEW
│   ├── IReportApiClient.cs + ReportApiClient.cs       ✅ NEW
│   ├── IEnrollmentApiClient.cs + EnrollmentApiClient.cs ✅ NEW
│   ├── IAttendanceApiClient.cs + AttendanceApiClient.cs ✅ NEW
│   ├── IGradeApiClient.cs + GradeApiClient.cs         ✅ NEW
│   ├── IPaymentApiClient.cs + PaymentApiClient.cs     ✅ NEW
│   ├── INotificationApiClient.cs + NotificationApiClient.cs ✅ NEW
│   ├── IReviewApiClient.cs + ReviewApiClient.cs       ✅ NEW
│   └── Models/
│       ├── CourseDtos.cs, RoomDtos.cs, UserDtos.cs, ClassDtos.cs
│       ├── ReportDtos.cs, EnrollmentDtos.cs, AttendanceDtos.cs
│       ├── GradeDtos.cs, PaymentDtos.cs, NotificationDtos.cs, ReviewDtos.cs
│       └── ApiResult.cs
├── Views/
│   ├── Shared/ (_Layout, _LoginPartial, _StaffNav, _TeacherNav, _StudentNav)
│   ├── StaffDashboard/Index             ✅
│   ├── StaffCourse/Index,Create,Edit,Details  ✅
│   ├── StaffRoom/Index,Create,Edit      ✅
│   ├── StaffTeacher/Index,Create,Edit,Details ✅
│   ├── StaffStudent/Index,Create,Edit,Details ✅
│   ├── StaffClass/Index,Create,Edit,Details   ✅
│   ├── StaffEnrollment/Index,Details    ✅
│   ├── StaffPayment/Index,Create,Debts,Revenue ✅
│   ├── StaffNotification/Index,Create   ✅
│   ├── StaffReport/Index,Revenue,Attendance ✅
│   ├── TeacherDashboard/Index           ✅
│   ├── TeacherClass/Index,Details       ✅
│   ├── TeacherSchedule/Index            ✅
│   ├── TeacherAttendance/Index,Take,History ✅
│   ├── TeacherGrade/Index,ClassGrades,Enter ✅
│   ├── TeacherNotification/Index,Create ✅
│   ├── StudentDashboard/Index           ✅
│   ├── StudentCourse/Index,Details      ✅
│   ├── StudentEnrollment/Index          ✅
│   ├── StudentSchedule/Index            ✅
│   ├── StudentAttendance/Index          ✅
│   ├── StudentGrade/Index               ✅
│   ├── StudentPayment/Index             ✅
│   ├── StudentNotification/Index,Details ✅
│   └── StudentReview/Index,Create       ✅
├── Program.cs                           ✏️  SUA (12 HttpClient services)
└── wwwroot/css/site.css                 ✏️  SUA
```

---

## 4. Thong ke so luong

| Loai | So luong |
|------|---------|
| Server Controllers | 13 |
| Server DTOs | 11 folders |
| Client Services | 12 interfaces + 12 implementations |
| Client Controllers | 27 |
| Client Views (.cshtml) | 68 |
| **Tong file** | **~155 file** |

---

## 5. API Endpoints tong hop

### Auth (da co)
| Method | Endpoint | Auth |
|--------|----------|------|
| POST | /api/auth/login | No |
| POST | /api/auth/register | No |
| GET | /api/auth/profile | Yes |
| PUT | /api/auth/profile | Yes |
| POST | /api/auth/change-password | Yes |

### Courses
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | /api/courses | AllowAnonymous |
| GET | /api/courses/{id} | AllowAnonymous |
| POST | /api/courses | Staff |
| PUT | /api/courses/{id} | Staff |
| DELETE | /api/courses/{id} | Staff |

### Rooms
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | /api/rooms | AllowAnonymous |
| GET | /api/rooms/{id} | AllowAnonymous |
| POST | /api/rooms | Staff |
| PUT | /api/rooms/{id} | Staff |
| DELETE | /api/rooms/{id} | Staff |

### Teachers / Students
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | /api/teachers | Staff |
| GET/POST/PUT | /api/teachers/{id} | Staff |
| POST | /api/teachers/{id}/toggle | Staff |
| GET | /api/students | Staff |
| GET/POST/PUT | /api/students/{id} | Staff |
| POST | /api/students/{id}/toggle | Staff |

### Classes
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | /api/classes(?teacherId=) | AllowAnonymous |
| GET | /api/classes/{id} | AllowAnonymous |
| POST/PUT/DELETE | /api/classes/{id} | Staff |
| GET | /api/classes/{id}/students | Staff,Teacher |

### Enrollments
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | /api/enrollments(?status=) | Staff |
| GET | /api/enrollments/my | Student |
| POST | /api/enrollments | Student |
| POST | /api/enrollments/{id}/approve | Staff |
| POST | /api/enrollments/{id}/reject | Staff |
| POST | /api/enrollments/{id}/cancel | Student |

### Attendances
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | /api/attendances(?classId&date) | Staff,Teacher |
| GET | /api/attendances/my(?classId) | Student |
| POST | /api/attendances/batch | Teacher |
| GET | /api/attendances/report/{classId} | Staff,Teacher |
| GET | /api/attendances/dates/{classId} | Staff,Teacher |

### Grades
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | /api/grades(?classId) | Staff,Teacher |
| GET | /api/grades/my | Student |
| POST | /api/grades/batch | Teacher |
| PUT | /api/grades/{id} | Teacher |

### Payments
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | /api/payments | Staff |
| GET | /api/payments/my | Student |
| POST | /api/payments | Staff |
| GET | /api/payments/debts | Staff |
| GET | /api/payments/my/debts | Student |

### Notifications
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | /api/notifications | Authenticated |
| GET | /api/notifications/{id} | Authenticated |
| POST | /api/notifications | Staff,Teacher |
| POST | /api/notifications/{id}/read | Authenticated |
| GET | /api/notifications/unread-count | Authenticated |

### Reviews
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | /api/reviews | Staff |
| GET | /api/reviews/my | Student |
| POST | /api/reviews | Student |

### Reports
| Method | Endpoint | Auth |
|--------|----------|------|
| GET | /api/reports/dashboard | Staff |
| GET | /api/reports/revenue(?year&month) | Staff |
| GET | /api/reports/enrollment-stats | Staff |

---

## 6. Naming Convention

| Thanh phan | Pattern | Vi du |
|-----------|---------|-------|
| Server API Controller | `{Entity}sController` | `CoursesController`, `AttendancesController` |
| Server DTO | `{Entity}Dto`, `Create{Entity}Dto` | `CourseDto`, `CreatePaymentDto` |
| Client MVC Controller | `{Role}{Entity}Controller` | `StaffCourseController`, `TeacherGradeController` |
| Client API Service | `I{Entity}ApiClient` / `{Entity}ApiClient` | `IGradeApiClient`, `GradeApiClient` |
| Client DTO | `{Entity}Dto`, `Create{Entity}Request` | `GradeDto`, `CreateReviewRequest` |
| View folder | `Views/{ControllerName}/` | `Views/TeacherGrade/`, `Views/StaffPayment/` |

---

## 7. Tai khoan test

| Role | Email | Password |
|------|-------|----------|
| Staff | admin@lmm.com | Admin@123 |
| Teacher | teacher@lmm.com | Teacher@123 |
| Student | student@lmm.com | Student@123 |

---

## 8. Cach chay project

```bash
# Terminal 1: Chay Server API
cd Server
dotnet run
# → http://localhost:5000

# Terminal 2: Chay Client MVC
cd Client
dotnet run
# → http://localhost:5240
```

Lan chay dau tien se tu dong: tao DB + migrate + seed roles + seed 3 users.
