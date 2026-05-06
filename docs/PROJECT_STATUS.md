# LMM Education - Trang thai du an & Ke hoach tiep tuc

> Cap nhat: 21/04/2026

---

## 1. Tong quan kien truc

```
┌─────────────────────────────────────────────────────────────┐
│                    LMM Education System                     │
├──────────────────────────┬──────────────────────────────────┤
│   Client (MVC - :5240)   │    Server (API - :5000)          │
├──────────────────────────┼──────────────────────────────────┤
│ ASP.NET Core MVC         │ ASP.NET Core Web API             │
│ Razor Views + Bootstrap  │ EF Core 8 + SQL Server           │
│ HttpClient → API         │ ASP.NET Identity + JWT           │
│ Session (JWT Token)      │ SignalR Hub                      │
│ SignalR Client            │ Code-First Migrations            │
└──────────────────────────┴──────────────────────────────────┘
```

| Thanh phan       | Cong nghe                                       |
|------------------|-------------------------------------------------|
| Runtime          | .NET 8                                          |
| Server           | ASP.NET Core Web API, Console App               |
| Client           | ASP.NET Core MVC, Razor, Bootstrap 5            |
| Database         | SQL Server (LocalDB `LMMEducation`)             |
| ORM              | Entity Framework Core 8.0.11 (Code-First)       |
| Authentication   | ASP.NET Identity + JWT Bearer                   |
| Realtime         | SignalR                                         |
| Client Libraries | Bootstrap, jQuery, jQuery Validation, SignalR JS |

---

## 2. Cau truc thu muc hien tai

```
Project_PRN222/
├── Project_PRN222.sln
├── docs/
│   ├── SYSTEM_FEATURES_PLAN.md    ← Ke hoach chuc nang chi tiet
│   └── PROJECT_STATUS.md          ← File nay
├── Database/
│   ├── LMMEducation_CreateDB.sql  ← Script tao DB goc
│   └── LMMEducation_SeedData.sql  ← Script seed data
├── .config/
│   └── dotnet-tools.json          ← dotnet-ef 8.0.11
│
├── Server/
│   ├── Server.csproj
│   ├── Program.cs                 ← DI, JWT, Identity, SignalR, CORS
│   ├── appsettings.json           ← ConnectionString, JWT config
│   ├── Controllers/
│   │   └── AuthController.cs      ✅ DONE
│   ├── Services/
│   │   ├── IJwtService.cs         ✅ DONE
│   │   └── JwtService.cs          ✅ DONE
│   ├── DTOs/Auth/
│   │   ├── LoginRequest.cs        ✅ DONE
│   │   ├── RegisterRequest.cs     ✅ DONE
│   │   ├── AuthResponse.cs        ✅ DONE
│   │   ├── UserProfileDto.cs      ✅ DONE
│   │   ├── UpdateProfileRequest.cs ✅ DONE
│   │   └── ChangePasswordRequest.cs ✅ DONE
│   ├── Hubs/
│   │   └── NotificationHub.cs     ✅ DONE (scaffolded)
│   ├── Models/
│   │   ├── AppUser.cs             ✅ DONE (Identity, da dung)
│   │   ├── Course.cs              ⚠️  Co model, chua wire vao DbContext
│   │   ├── Class.cs               ⚠️  Co model, chua wire vao DbContext
│   │   ├── Room.cs                ⚠️  Co model, chua wire vao DbContext
│   │   ├── Subject.cs             ⚠️  Co model, chua wire vao DbContext
│   │   ├── Enrollment.cs          ⚠️  Co model, chua wire vao DbContext
│   │   ├── Attendance.cs          ⚠️  Co model, chua wire vao DbContext
│   │   ├── Grade.cs               ⚠️  Co model, chua wire vao DbContext
│   │   ├── Payment.cs             ⚠️  Co model, chua wire vao DbContext
│   │   ├── ClassSchedule.cs       ⚠️  Co model, chua wire vao DbContext
│   │   ├── Notification.cs        ⚠️  Co model, chua wire vao DbContext
│   │   ├── NotificationRead.cs    ⚠️  Co model, chua wire vao DbContext
│   │   ├── TeacherReview.cs       ⚠️  Co model, chua wire vao DbContext
│   │   ├── LmmeducationContext.cs ❌ Context cu (scaffold), KHONG dung
│   │   └── AspNet*.cs (6 file)    ❌ Entity cu (scaffold), KHONG dung
│   └── Data/
│       ├── LMMDbContext.cs        ✅ DONE (chi co AppUser)
│       ├── DbSeeder.cs            ✅ DONE (seed roles)
│       └── Migrations/            ✅ 1 migration (Identity tables)
│
└── Client/
    ├── Client.csproj
    ├── Program.cs                 ✅ DONE (Session, HttpClient, Routing)
    ├── Controllers/
    │   ├── HomeController.cs      ✅ DONE
    │   └── AuthController.cs      ✅ DONE
    ├── Services/
    │   ├── IAuthApiClient.cs      ✅ DONE
    │   ├── AuthApiClient.cs       ✅ DONE
    │   └── Models/
    │       ├── ApiResult.cs       ✅ DONE
    │       └── AuthDtos.cs        ✅ DONE
    ├── ViewModels/Auth/
    │   ├── LoginViewModel.cs      ✅ DONE
    │   ├── RegisterViewModel.cs   ✅ DONE
    │   ├── ProfileViewModel.cs    ✅ DONE
    │   └── ChangePasswordViewModel.cs ✅ DONE
    ├── Views/
    │   ├── Home/Index.cshtml      ✅ DONE
    │   ├── Home/Privacy.cshtml    ✅ DONE
    │   ├── Auth/Login.cshtml      ✅ DONE
    │   ├── Auth/Register.cshtml   ✅ DONE
    │   ├── Auth/Profile.cshtml    ✅ DONE
    │   └── Shared/_Layout.cshtml  ✅ DONE (co SignalR client)
    └── wwwroot/
        ├── css/site.css
        ├── js/site.js
        ├── js/notification-hub.js ✅ DONE
        └── lib/ (bootstrap, jquery, signalr)
```

---

## 3. Tinh trang hien tai - Da hoan thanh

### ✅ PHASE 1: Authentication & Core (100%)

| Chuc nang                  | Server API | Client MVC | Trang thai |
|----------------------------|------------|------------|------------|
| Dang nhap (Login)          | ✅          | ✅          | Hoan thanh |
| Dang ky (Register)         | ✅          | ✅          | Hoan thanh |
| Xem profile                | ✅          | ✅          | Hoan thanh |
| Cap nhat profile           | ✅          | ✅          | Hoan thanh |
| Doi mat khau               | ✅          | ✅          | Hoan thanh |
| Dang xuat (Logout)         | -          | ✅          | Hoan thanh |
| JWT Token + Session        | ✅          | ✅          | Hoan thanh |
| 3 Roles (Staff/Teacher/Student) | ✅     | ✅          | Hoan thanh |
| SignalR Hub                | ✅          | ✅ (client) | Scaffolded |
| Db Seeder (Roles)          | ✅          | -          | Hoan thanh |

### ⚠️ Van de can sua truoc khi tiep tuc

| Van de | Mo ta | Anh huong |
|--------|-------|-----------|
| Login redirect | Hien tai Login/Register redirect ve `Profile` thay vi Dashboard theo role | Can goi `RedirectByRole()` da co san |
| Domain Models chua wire | 12 entity models da co nhung chua add vao `LMMDbContext` | Can them DbSet + Migration |
| File scaffold cu | `LmmeducationContext.cs` + `AspNet*.cs` la file scaffold tu DB, khong dung | Can xoa de tranh nham lan |
| Chua seed test users | `DbSeeder` chi seed roles, chua tao 3 tai khoan test | Can them seed users |

---

## 4. Chua hoan thanh - Tong quan

| Phase | Mo ta | Trang thai | Uu tien |
|-------|-------|------------|---------|
| Phase 1 | Authentication & Core | ✅ 100% (can fix redirect) | - |
| Phase 2 | Staff - Quan tri he thong | ❌ 0% | **CAO** |
| Phase 3 | Teacher - Giao vien | ❌ 0% | TRUNG BINH |
| Phase 4 | Student - Hoc vien | ❌ 0% | TRUNG BINH |
| Phase 5 | Shared Components | ⚠️ 20% (co Layout) | CAO |
| Phase 6 | Tinh nang nang cao | ❌ 0% | THAP |

---

## 5. Ke hoach tiep tuc chi tiet

### STEP 0: Chuan bi nen tang (Can lam truoc tat ca)

**Muc tieu:** Fix cac van de hien tai va chuan bi database cho domain models.

| # | Viec can lam | File thay doi | Do kho |
|---|-------------|---------------|--------|
| 0.1 | Xoa file scaffold cu | Xoa `LmmeducationContext.cs`, `AspNet*.cs` (6 file) | De |
| 0.2 | Sua domain models dung `AppUser` thay `AspNetUser` | `Class.cs`, `Enrollment.cs`, `Attendance.cs`, `Grade.cs`, `Payment.cs`, `Notification.cs`, `NotificationRead.cs`, `TeacherReview.cs` | Trung binh |
| 0.3 | Them DbSet vao `LMMDbContext` | `LMMDbContext.cs` - them 12 DbSet + fluent config | Trung binh |
| 0.4 | Tao Migration moi | `dotnet ef migrations add AddDomainEntities` | De |
| 0.5 | Mo rong `DbSeeder` - seed test users | `DbSeeder.cs` - tao admin@lmm.com, teacher@lmm.com, student@lmm.com | De |
| 0.6 | Fix Login redirect theo role | `Client/AuthController.cs` - goi `RedirectByRole()` | De |
| 0.7 | Tao `BaseApiClient` cho Client | `Client/Services/BaseApiClient.cs` - generic HTTP helper | Trung binh |

**Ket qua:** Database san sang voi full schema, 3 test users, login redirect dung role.

---

### STEP 1: Shared Components (Layout + Navigation)

**Muc tieu:** Tao he thong layout va menu phan quyen cho 3 role.

| # | Viec can lam | File tao moi |
|---|-------------|-------------|
| 1.1 | Tao `_StaffNav.cshtml` | `Views/Shared/_StaffNav.cshtml` |
| 1.2 | Tao `_TeacherNav.cshtml` | `Views/Shared/_TeacherNav.cshtml` |
| 1.3 | Tao `_StudentNav.cshtml` | `Views/Shared/_StudentNav.cshtml` |
| 1.4 | Tao `_LoginPartial.cshtml` | `Views/Shared/_LoginPartial.cshtml` |
| 1.5 | Cap nhat `_Layout.cshtml` | Hien thi menu theo role tu Session |
| 1.6 | Tao `AuthorizeFilter` hoac `BaseController` | Check role truoc khi vao area |

**Ket qua:** Menu hien thi dung theo role, UI nhat quan.

---

### STEP 2: Staff - Quan ly Khoa hoc & Phong hoc (CRUD don gian nhat)

**Muc tieu:** Xay chuc nang CRUD co ban, tao pattern cho cac module sau.

#### 2A. Server API

| Viec | File |
|------|------|
| Tao `CoursesController.cs` | CRUD: GET, GET/{id}, POST, PUT, DELETE |
| Tao `RoomsController.cs` | CRUD: GET, GET/{id}, POST, PUT, DELETE |
| Tao DTOs | `CourseDto`, `CreateCourseDto`, `RoomDto`, `CreateRoomDto` |

#### 2B. Client MVC

| Viec | File |
|------|------|
| Tao `ICourseApiClient` + `CourseApiClient` | HTTP client goi API |
| Tao `IRoomApiClient` + `RoomApiClient` | HTTP client goi API |
| Tao `StaffCourseController` | Index, Create, Edit, Details, Delete |
| Tao `StaffRoomController` | Index, Create, Edit |
| Tao Views cho Course | 4 view: Index, Create, Edit, Details |
| Tao Views cho Room | 3 view: Index, Create, Edit |

**Ket qua:** Staff co the CRUD khoa hoc va phong hoc.

---

### STEP 3: Staff - Quan ly Giao vien & Hoc vien

**Muc tieu:** Quan ly nguoi dung (Teacher/Student) voi Identity.

#### 3A. Server API

| Viec | File |
|------|------|
| Tao `TeachersController.cs` | CRUD + Toggle Active |
| Tao `StudentsController.cs` | CRUD + Toggle Active |
| Tao DTOs | `TeacherDto`, `StudentDto`, `CreateTeacherDto`, `CreateStudentDto` |

#### 3B. Client MVC

| Viec | File |
|------|------|
| Tao `IUserApiClient` | Chung cho Teacher + Student API |
| Tao `StaffTeacherController` + Views | Index, Create, Edit, Details |
| Tao `StaffStudentController` + Views | Index, Create, Edit, Details |

**Ket qua:** Staff co the tao/sua/xem giao vien va hoc vien.

---

### STEP 4: Staff - Quan ly Lop hoc

**Muc tieu:** CRUD lop hoc voi lien ket Khoa hoc + GV + Phong + Lich.

#### 4A. Server API

| Viec | File |
|------|------|
| Tao `ClassesController.cs` | CRUD + Students list + Schedule |
| Tao DTOs | `ClassDto`, `CreateClassDto`, `ClassScheduleDto` |

#### 4B. Client MVC

| Viec | File |
|------|------|
| Tao `IClassApiClient` | HTTP client |
| Tao `StaffClassController` + Views | Index, Create, Edit, Details |
| Form tao lop | Dropdown Khoa hoc, GV, Phong + Lich hoc |

**Ket qua:** Staff co the tao lop hoc day du (chon khoa hoc, GV, phong, lich).

---

### STEP 5: Staff Dashboard

**Muc tieu:** Trang tong quan cho Staff.

| Viec | File |
|------|------|
| Tao `ReportsController.cs` (Server) | API `/api/reports/dashboard` |
| Tao `StaffDashboardController` (Client) | Hien thi dashboard |
| Dashboard View | So lop, HV, GV, doanh thu, DS lop sap bat dau |

**Ket qua:** Staff co dashboard tong quan.

---

### STEP 6: Student - Dang ky hoc & Xem lich

**Muc tieu:** Student co the xem khoa hoc, dang ky lop.

| Viec | File |
|------|------|
| Tao `EnrollmentsController.cs` (Server) | POST dang ky, GET my enrollments |
| Bo sung `CoursesController` | GET available courses/classes |
| Tao `StudentCourseController` (Client) | Xem khoa hoc + lop dang mo |
| Tao `StudentEnrollmentController` (Client) | Dang ky + xem DS dang ky |
| Tao `StudentScheduleController` (Client) | Lich hoc calendar |

**Ket qua:** Student xem khoa hoc, dang ky lop, xem lich hoc.

---

### STEP 7: Staff - Duyet dang ky

**Muc tieu:** Staff duyet/tu choi dang ky cua Student.

| Viec | File |
|------|------|
| Bo sung `EnrollmentsController` (Server) | Approve/Reject endpoints |
| Tao `StaffEnrollmentController` (Client) | DS cho duyet + action |

**Ket qua:** Quy trinh dang ky hoan chinh: Student dang ky → Staff duyet.

---

### STEP 8: Teacher - Xem lop + Diem danh

**Muc tieu:** Teacher xem lop dang day, diem danh.

| Viec | File |
|------|------|
| Tao `AttendancesController.cs` (Server) | Batch diem danh + History |
| Tao `TeacherClassController` (Client) | DS lop day + chi tiet |
| Tao `TeacherAttendanceController` (Client) | Form diem danh ca lop |
| Tao `TeacherScheduleController` (Client) | Lich day |

**Ket qua:** Teacher xem lop, diem danh tung buoi.

---

### STEP 9: Teacher - Nhap diem + Student xem diem

**Muc tieu:** Teacher nhap diem, Student xem diem.

| Viec | File |
|------|------|
| Tao `GradesController.cs` (Server) | Batch nhap diem + My grades |
| Tao `TeacherGradeController` (Client) | Form nhap diem ca lop |
| Tao `StudentGradeController` (Client) | Xem diem ca nhan |

**Ket qua:** Teacher nhap diem, Student xem diem theo lop.

---

### STEP 10: Quan ly Hoc phi

**Muc tieu:** Staff ghi nhan thanh toan, Student xem cong no.

| Viec | File |
|------|------|
| Tao `PaymentsController.cs` (Server) | CRUD + Report + Debts |
| Tao `StaffPaymentController` (Client) | Ghi nhan thanh toan + bao cao |
| Tao `StudentPaymentController` (Client) | Xem lich su + cong no |

**Ket qua:** Quy trinh hoc phi hoan chinh.

---

### STEP 11: Thong bao

**Muc tieu:** He thong thong bao + SignalR realtime.

| Viec | File |
|------|------|
| Tao `NotificationsController.cs` (Server) | CRUD + Mark read |
| Tao Controller cho 3 role (Client) | Gui/Nhan thong bao |
| Ket noi SignalR | Push thong bao real-time |

---

### STEP 12: Bao cao & Thong ke + Danh gia GV

| Viec | File |
|------|------|
| Mo rong `ReportsController` (Server) | Attendance, Revenue reports |
| Tao `ReviewsController.cs` (Server) | Student danh gia GV |
| Tao Views bao cao (Client) | Chart + table |
| Tao `StudentReviewController` (Client) | Form danh gia |

---

## 6. So luong file can tao cho moi Step

| Step | Server (Controllers + DTOs) | Client (Controllers + Services + Views) | Tong |
|------|---------------------------|----------------------------------------|------|
| 0 | Sua 2 file, xoa 7 file | Sua 1 file, tao 1 file | ~11 |
| 1 | 0 | Tao 5 file partial views | ~5 |
| 2 | 2 controllers + 4 DTOs | 2 services + 2 controllers + 7 views | ~17 |
| 3 | 2 controllers + 4 DTOs | 1 service + 2 controllers + 8 views | ~17 |
| 4 | 1 controller + 3 DTOs | 1 service + 1 controller + 4 views | ~10 |
| 5 | 1 controller + 1 DTO | 1 controller + 1 view | ~4 |
| 6 | 1 controller + 3 DTOs | 3 controllers + 6 views | ~13 |
| 7 | 0 (bo sung) | 1 controller + 2 views | ~3 |
| 8 | 1 controller + 3 DTOs | 3 controllers + 5 views | ~12 |
| 9 | 1 controller + 2 DTOs | 2 controllers + 4 views | ~9 |
| 10 | 1 controller + 2 DTOs | 2 controllers + 5 views | ~10 |
| 11 | 1 controller + 2 DTOs | 3 controllers + 5 views | ~11 |
| 12 | 2 controllers + 2 DTOs | 2 controllers + 5 views | ~11 |

**Tong uoc tinh: ~123 file moi**

---

## 7. Database Schema - Entity Relationship

```
AppUser (Identity)
    │
    ├── 1:N ── Class (as Teacher)
    ├── 1:N ── Enrollment (as Student)
    ├── 1:N ── Attendance (as Student)
    ├── 1:N ── Grade (as Student)
    ├── 1:N ── Payment (as Student)
    ├── 1:N ── Notification (as Sender)
    ├── 1:N ── NotificationRead (as User)
    ├── 1:N ── TeacherReview (as Student)
    └── 1:N ── TeacherReview (as Teacher)

Course
    ├── 1:N ── Class
    └── 1:N ── Subject

Class
    ├── N:1 ── Course
    ├── N:1 ── Room
    ├── N:1 ── AppUser (Teacher)
    ├── 1:N ── ClassSchedule
    ├── 1:N ── Enrollment
    ├── 1:N ── Attendance
    ├── 1:N ── Grade
    └── 1:N ── Payment

Room
    └── 1:N ── Class
```

---

## 8. Enum Values (Can dinh nghia)

```csharp
// Class.Status
enum ClassStatus { Upcoming = 0, InProgress = 1, Completed = 2, Cancelled = 3 }

// Enrollment.Status
enum EnrollmentStatus { Pending = 0, Approved = 1, Rejected = 2, Cancelled = 3 }

// Attendance.Status
enum AttendanceStatus { Present = 0, Absent = 1, Late = 2, Excused = 3 }

// Grade.Type
enum GradeType { Quiz = 0, Midterm = 1, Final = 2, Assignment = 3, Participation = 4 }

// Payment.Method
enum PaymentMethod { Cash = 0, BankTransfer = 1, Card = 2 }

// Payment.Status
enum PaymentStatus { Pending = 0, Completed = 1, Refunded = 2 }

// Notification.TargetType
enum NotificationTargetType { All = 0, Role = 1, Class = 2, User = 3 }
```

---

## 9. Tai khoan test

| Role    | Email             | Password    | Trang thai   |
|---------|-------------------|-------------|--------------|
| Staff   | admin@lmm.com    | Admin@123   | Chua seed    |
| Teacher | teacher@lmm.com  | Teacher@123 | Chua seed    |
| Student | student@lmm.com  | Student@123 | Chua seed    |

---

## 10. Tien do tong the

```
Phase 1: Auth           ████████████████████ 100%
Phase 2: Staff          ░░░░░░░░░░░░░░░░░░░░   0%
Phase 3: Teacher        ░░░░░░░░░░░░░░░░░░░░   0%
Phase 4: Student        ░░░░░░░░░░░░░░░░░░░░   0%
Phase 5: Shared         ████░░░░░░░░░░░░░░░░  20%
Phase 6: Advanced       ░░░░░░░░░░░░░░░░░░░░   0%
─────────────────────────────────────────────────
Tong the:               ████░░░░░░░░░░░░░░░░ ~15%
```

---

## 11. Ghi chu ky thuat

### Pattern chung cho moi module moi

**Server side (moi module):**
1. Tao DTOs trong `Server/DTOs/{Module}/`
2. Tao Controller trong `Server/Controllers/`
3. Authorize voi `[Authorize(Roles = "Staff")]` (hoac role phu hop)

**Client side (moi module):**
1. Tao Service interface + implementation trong `Client/Services/`
2. Register service trong `Client/Program.cs`
3. Tao Controller trong `Client/Controllers/`
4. Tao ViewModels trong `Client/ViewModels/{Module}/`
5. Tao Views trong `Client/Views/{Area}/`

### Quy tac naming

| Thanh phan | Server | Client |
|-----------|--------|--------|
| API Controller | `CoursesController` (so nhieu) | - |
| MVC Controller | - | `StaffCourseController` (Role + Module) |
| API Client | - | `ICourseApiClient` / `CourseApiClient` |
| DTO | `CourseDto`, `CreateCourseDto` | Dung chung hoac map sang ViewModel |
| View | - | `Views/Staff/Course/Index.cshtml` |

---

## 12. Buoc tiep theo ngay lap tuc

> **Bat dau voi STEP 0** - chuan bi nen tang database truoc khi lam bat ky chuc nang nao.

1. Xoa 7 file scaffold cu (AspNet*.cs + LmmeducationContext.cs)
2. Sua domain models: `AspNetUser` → `AppUser`
3. Them 12 DbSet vao `LMMDbContext`
4. Tao migration moi
5. Seed 3 test users
6. Fix login redirect
7. Tao `BaseApiClient` pattern

Sau khi STEP 0 xong → Chuyen sang STEP 1 (Shared Layout) → STEP 2 (Course + Room CRUD).
