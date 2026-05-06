# LMM Education - Ke hoach chuc nang he thong

## Tong quan

- **He thong**: Quan ly giao duc trung tam day hoc truc tiep
- **Kien truc**: Console App (Server API) + ASP.NET Core MVC (Client)
- **3 Roles**: Staff, Teacher, Student
- **Database**: SQL Server + EF Core 8 (Code-First)
- **Authentication**: ASP.NET Identity + JWT Token

---

## Kien truc Client-Server

```
LMMClient (MVC - Port 5013)          Server (Console App - Port 5000)
┌──────────────────────────┐          ┌──────────────────────────┐
│  Views (Razor/HTML)      │          │  API Controllers         │
│  Controllers             │  HTTP    │  Services                │
│  HttpClient Service  ────┼─────────>│  EF Core + SQL Server    │
│  Session (JWT Token)     │  JSON    │  Identity (Auth)         │
│  Bootstrap 5 UI          │<─────────┤  JWT Token Generator     │
└──────────────────────────┘          └──────────────────────────┘
```

---

## PHASE 1: Authentication & Core (Uu tien cao nhat)

### 1.1 Server API - AuthController
| API Endpoint              | Method | Mo ta                          | Auth  |
|---------------------------|--------|--------------------------------|-------|
| /api/auth/login           | POST   | Dang nhap, tra JWT token       | No    |
| /api/auth/register        | POST   | Dang ky tai khoan Student      | No    |
| /api/auth/profile         | GET    | Lay thong tin user hien tai    | Yes   |
| /api/auth/profile         | PUT    | Cap nhat profile ca nhan       | Yes   |
| /api/auth/change-password | POST   | Doi mat khau                   | Yes   |

### 1.2 MVC Client - AuthController
| Route            | View           | Mo ta                              |
|------------------|----------------|------------------------------------|
| /Auth/Login      | Login.cshtml   | Form dang nhap                     |
| /Auth/Register   | Register.cshtml| Form dang ky (chi Student)         |
| /Auth/Logout     | -              | Xoa session, redirect Login        |
| /Auth/Profile    | Profile.cshtml | Xem/sua thong tin ca nhan          |

### 1.3 Luong xu ly Login
```
1. User nhap email + password tai Login form
2. MVC AuthController goi POST /api/auth/login
3. Server xac thuc -> tra JWT token + user info + role
4. MVC luu token vao Session
5. Redirect ve Dashboard tuong ung voi role
6. Moi request tiep theo, MVC gui token trong Header khi goi API
```

### 1.4 Phan quyen theo Role
| Role    | Redirect sau Login | Menu hien thi               |
|---------|--------------------|-----------------------------|
| Staff   | /Staff/Dashboard   | Full menu quan tri          |
| Teacher | /Teacher/Dashboard | Menu giao vien              |
| Student | /Student/Dashboard | Menu hoc vien               |

---

## PHASE 2: Staff - Quan tri he thong

### 2.1 Dashboard (StaffController)
| Route            | Mo ta                                              |
|------------------|----------------------------------------------------|
| /Staff/Dashboard | Tong quan: so lop, hoc vien, giao vien, doanh thu  |

**Dashboard hien thi:**
- Tong so hoc vien dang hoat dong
- Tong so giao vien
- Tong so lop dang dien ra
- Doanh thu thang hien tai
- Bieu do dang ky theo thang (chart)
- Danh sach lop sap bat dau

### 2.2 Quan ly Khoa hoc (Server: CoursesController, Client: CourseController)

**Server API:**
| API Endpoint              | Method | Mo ta                     |
|---------------------------|--------|---------------------------|
| /api/courses              | GET    | Danh sach khoa hoc        |
| /api/courses/{id}         | GET    | Chi tiet khoa hoc         |
| /api/courses              | POST   | Tao khoa hoc moi          |
| /api/courses/{id}         | PUT    | Cap nhat khoa hoc         |
| /api/courses/{id}         | DELETE | Xoa/an khoa hoc           |
| /api/courses/{id}/subjects| GET    | Danh sach mon hoc         |

**MVC Client:**
| Route                      | View              | Mo ta                    |
|----------------------------|-------------------|--------------------------|
| /Staff/Courses             | Index.cshtml      | Danh sach khoa hoc       |
| /Staff/Courses/Create      | Create.cshtml     | Form tao khoa hoc        |
| /Staff/Courses/Edit/{id}   | Edit.cshtml       | Form sua khoa hoc        |
| /Staff/Courses/Details/{id}| Details.cshtml    | Chi tiet + mon hoc       |
| /Staff/Courses/Delete/{id} | Delete.cshtml     | Xac nhan xoa             |

### 2.3 Quan ly Lop hoc (Server: ClassesController, Client: ClassController)

**Server API:**
| API Endpoint                    | Method | Mo ta                      |
|---------------------------------|--------|----------------------------|
| /api/classes                    | GET    | Danh sach lop hoc          |
| /api/classes/{id}               | GET    | Chi tiet lop hoc           |
| /api/classes                    | POST   | Tao lop hoc moi            |
| /api/classes/{id}               | PUT    | Cap nhat lop hoc           |
| /api/classes/{id}               | DELETE | Xoa lop hoc                |
| /api/classes/{id}/students      | GET    | DS hoc vien trong lop      |
| /api/classes/{id}/schedule      | GET    | Lich hoc cua lop           |
| /api/classes/{id}/schedule      | POST   | Them lich hoc              |

**MVC Client:**
| Route                           | View              | Mo ta                   |
|---------------------------------|-------------------|-------------------------|
| /Staff/Classes                  | Index.cshtml      | DS lop hoc (filter)     |
| /Staff/Classes/Create           | Create.cshtml     | Form tao lop            |
| /Staff/Classes/Edit/{id}        | Edit.cshtml       | Form sua lop            |
| /Staff/Classes/Details/{id}     | Details.cshtml    | Chi tiet + DS hoc vien  |

**Form tao lop gom:**
- Chon khoa hoc (dropdown)
- Chon giao vien (dropdown)
- Chon phong hoc (dropdown)
- Ngay bat dau / ket thuc
- Si so toi da
- Cai dat lich hoc (ngay trong tuan + gio)

### 2.4 Quan ly Phong hoc (Server: RoomsController, Client: RoomController)

**Server API:**
| API Endpoint         | Method | Mo ta                    |
|----------------------|--------|--------------------------|
| /api/rooms           | GET    | Danh sach phong hoc      |
| /api/rooms/{id}      | GET    | Chi tiet phong hoc       |
| /api/rooms           | POST   | Tao phong hoc            |
| /api/rooms/{id}      | PUT    | Cap nhat phong hoc       |
| /api/rooms/{id}      | DELETE | Xoa phong hoc            |
| /api/rooms/available | GET    | Phong trong theo lich    |

**MVC Client:**
| Route                     | View              | Mo ta                |
|---------------------------|-------------------|----------------------|
| /Staff/Rooms              | Index.cshtml      | DS phong hoc         |
| /Staff/Rooms/Create       | Create.cshtml     | Tao phong            |
| /Staff/Rooms/Edit/{id}    | Edit.cshtml       | Sua phong            |

### 2.5 Quan ly Giao vien (Server: TeachersController)

**Server API:**
| API Endpoint              | Method | Mo ta                       |
|---------------------------|--------|-----------------------------|
| /api/teachers             | GET    | DS giao vien                |
| /api/teachers/{id}        | GET    | Chi tiet giao vien          |
| /api/teachers             | POST   | Tao tai khoan giao vien     |
| /api/teachers/{id}        | PUT    | Cap nhat thong tin GV       |
| /api/teachers/{id}/toggle | POST   | Kich hoat/vo hieu hoa       |
| /api/teachers/{id}/classes| GET    | DS lop dang/da day          |

**MVC Client:**
| Route                         | View              | Mo ta                   |
|-------------------------------|-------------------|-------------------------|
| /Staff/Teachers               | Index.cshtml      | DS giao vien            |
| /Staff/Teachers/Create        | Create.cshtml     | Tao tai khoan GV        |
| /Staff/Teachers/Edit/{id}     | Edit.cshtml       | Sua thong tin GV        |
| /Staff/Teachers/Details/{id}  | Details.cshtml    | Chi tiet + DS lop day   |

### 2.6 Quan ly Hoc vien (Server: StudentsController)

**Server API:**
| API Endpoint                    | Method | Mo ta                      |
|---------------------------------|--------|----------------------------|
| /api/students                   | GET    | DS hoc vien                |
| /api/students/{id}              | GET    | Chi tiet hoc vien          |
| /api/students                   | POST   | Tao tai khoan hoc vien     |
| /api/students/{id}              | PUT    | Cap nhat thong tin HV      |
| /api/students/{id}/toggle       | POST   | Kich hoat/vo hieu hoa      |
| /api/students/{id}/enrollments  | GET    | DS lop da dang ky          |
| /api/students/{id}/grades       | GET    | Bang diem tong hop         |

**MVC Client:**
| Route                          | View              | Mo ta                   |
|--------------------------------|-------------------|-------------------------|
| /Staff/Students                | Index.cshtml      | DS hoc vien             |
| /Staff/Students/Create         | Create.cshtml     | Tao tai khoan HV        |
| /Staff/Students/Edit/{id}      | Edit.cshtml       | Sua thong tin HV        |
| /Staff/Students/Details/{id}   | Details.cshtml    | Chi tiet + lich su hoc  |

### 2.7 Duyet dang ky hoc (Server: EnrollmentsController)

**Server API:**
| API Endpoint                    | Method | Mo ta                      |
|---------------------------------|--------|----------------------------|
| /api/enrollments                | GET    | DS dang ky (filter status) |
| /api/enrollments/{id}/approve   | POST   | Duyet dang ky              |
| /api/enrollments/{id}/reject    | POST   | Tu choi dang ky            |

**MVC Client:**
| Route                           | View              | Mo ta                  |
|---------------------------------|-------------------|------------------------|
| /Staff/Enrollments              | Index.cshtml      | DS cho duyet           |
| /Staff/Enrollments/Details/{id} | Details.cshtml    | Chi tiet de duyet      |

### 2.8 Quan ly Hoc phi (Server: PaymentsController)

**Server API:**
| API Endpoint                | Method | Mo ta                      |
|-----------------------------|--------|----------------------------|
| /api/payments               | GET    | DS thanh toan              |
| /api/payments               | POST   | Ghi nhan thanh toan        |
| /api/payments/{id}          | GET    | Chi tiet thanh toan        |
| /api/payments/report        | GET    | Bao cao doanh thu          |
| /api/payments/debts         | GET    | DS cong no hoc phi         |

**MVC Client:**
| Route                         | View              | Mo ta                   |
|-------------------------------|-------------------|-------------------------|
| /Staff/Payments               | Index.cshtml      | DS thanh toan           |
| /Staff/Payments/Create        | Create.cshtml     | Ghi nhan thanh toan     |
| /Staff/Payments/Report        | Report.cshtml     | Bao cao doanh thu       |
| /Staff/Payments/Debts         | Debts.cshtml      | DS cong no              |

### 2.9 Thong bao (Server: NotificationsController)

**Server API:**
| API Endpoint                | Method | Mo ta                      |
|-----------------------------|--------|----------------------------|
| /api/notifications          | GET    | DS thong bao               |
| /api/notifications          | POST   | Gui thong bao              |
| /api/notifications/{id}     | GET    | Chi tiet thong bao         |
| /api/notifications/{id}/read| POST   | Danh dau da doc            |

**MVC Client:**
| Route                            | View              | Mo ta                |
|----------------------------------|-------------------|----------------------|
| /Staff/Notifications             | Index.cshtml      | DS thong bao         |
| /Staff/Notifications/Create      | Create.cshtml     | Gui thong bao moi    |

### 2.10 Bao cao & Thong ke

**Server API:**
| API Endpoint                    | Method | Mo ta                      |
|---------------------------------|--------|----------------------------|
| /api/reports/dashboard          | GET    | Du lieu dashboard          |
| /api/reports/attendance/{classId}| GET   | Bao cao chuyen can lop     |
| /api/reports/revenue            | GET    | Bao cao doanh thu          |
| /api/reports/enrollments        | GET    | Thong ke dang ky           |

**MVC Client:**
| Route                    | View              | Mo ta                      |
|--------------------------|-------------------|----------------------------|
| /Staff/Reports           | Index.cshtml      | Trang bao cao tong hop     |
| /Staff/Reports/Attendance| Attendance.cshtml | Bao cao chuyen can         |
| /Staff/Reports/Revenue   | Revenue.cshtml    | Bao cao doanh thu          |

---

## PHASE 3: Teacher - Giao vien

### 3.1 Dashboard (TeacherController)
| Route              | Mo ta                                             |
|--------------------|---------------------------------------------------|
| /Teacher/Dashboard | Tong quan: lop dang day, lich hom nay, thong bao  |

**Dashboard hien thi:**
- DS lop dang day
- Lich day hom nay
- So hoc vien tong
- Thong bao moi nhat

### 3.2 Quan ly Lop day

**Server API** (dung chung ClassesController, filter theo TeacherId):
| API Endpoint                          | Method | Mo ta                   |
|---------------------------------------|--------|-------------------------|
| /api/classes?teacherId={id}           | GET    | DS lop dang day         |
| /api/classes/{id}/students            | GET    | DS hoc vien trong lop   |

**MVC Client:**
| Route                           | View              | Mo ta                   |
|---------------------------------|-------------------|-------------------------|
| /Teacher/Classes                | Index.cshtml      | DS lop dang day         |
| /Teacher/Classes/Details/{id}   | Details.cshtml    | Chi tiet lop + DS HV    |

### 3.3 Lich day

**MVC Client:**
| Route              | View              | Mo ta                          |
|--------------------|-------------------|--------------------------------|
| /Teacher/Schedule  | Index.cshtml      | Lich day theo tuan (calendar)  |

### 3.4 Diem danh (Server: AttendancesController)

**Server API:**
| API Endpoint                          | Method | Mo ta                      |
|---------------------------------------|--------|----------------------------|
| /api/attendances                      | POST   | Diem danh 1 buoi           |
| /api/attendances/batch                | POST   | Diem danh ca lop 1 buoi    |
| /api/attendances?classId={id}&date={d}| GET    | DS diem danh theo buoi     |
| /api/attendances/report/{classId}     | GET    | Bao cao chuyen can lop     |

**MVC Client:**
| Route                                | View              | Mo ta                  |
|--------------------------------------|-------------------|------------------------|
| /Teacher/Attendance/{classId}        | Index.cshtml      | Chon buoi de diem danh |
| /Teacher/Attendance/Take/{classId}   | Take.cshtml       | Form diem danh ca lop  |
| /Teacher/Attendance/History/{classId}| History.cshtml    | Lich su diem danh      |

**Giao dien diem danh:**
- Hien thi DS hoc vien cua lop
- Moi hoc vien co radio: Co mat / Vang / Di muon / Co phep
- Truong ghi chu cho tung HV
- Nut Luu diem danh

### 3.5 Nhap diem (Server: GradesController)

**Server API:**
| API Endpoint                          | Method | Mo ta                      |
|---------------------------------------|--------|----------------------------|
| /api/grades                           | POST   | Nhap diem cho HV           |
| /api/grades/batch                     | POST   | Nhap diem ca lop           |
| /api/grades?classId={id}              | GET    | Bang diem lop              |
| /api/grades/{id}                      | PUT    | Sua diem                   |

**MVC Client:**
| Route                            | View              | Mo ta                     |
|----------------------------------|-------------------|---------------------------|
| /Teacher/Grades/{classId}        | Index.cshtml      | Bang diem lop             |
| /Teacher/Grades/Enter/{classId}  | Enter.cshtml      | Form nhap diem ca lop     |
| /Teacher/Grades/Edit/{id}        | Edit.cshtml       | Sua diem 1 HV             |

**Giao dien nhap diem:**
- Chon loai diem: Quiz / Giua ky / Cuoi ky / Bai tap / Tham gia
- Bang nhap diem cho ca lop
- Trong so (weight) cho moi loai diem
- Tinh diem trung binh tu dong

### 3.6 Gui thong bao cho lop

**MVC Client:**
| Route                               | View              | Mo ta                  |
|-------------------------------------|-------------------|------------------------|
| /Teacher/Notifications              | Index.cshtml      | DS thong bao da gui   |
| /Teacher/Notifications/Create       | Create.cshtml     | Gui thong bao cho lop |

---

## PHASE 4: Student - Hoc vien

### 4.1 Dashboard (StudentController)
| Route              | Mo ta                                                |
|--------------------|------------------------------------------------------|
| /Student/Dashboard | Tong quan: lop dang hoc, lich hom nay, diem, hoc phi |

**Dashboard hien thi:**
- DS lop dang hoc
- Lich hoc hom nay
- Diem moi nhat
- Tinh trang hoc phi
- Thong bao moi

### 4.2 Dang ky khoa hoc

**Server API:**
| API Endpoint                   | Method | Mo ta                     |
|--------------------------------|--------|---------------------------|
| /api/courses/available         | GET    | DS khoa hoc dang mo       |
| /api/classes/available         | GET    | DS lop dang tuyen sinh    |
| /api/enrollments               | POST   | Dang ky vao lop           |
| /api/enrollments/my            | GET    | DS dang ky cua toi        |

**MVC Client:**
| Route                           | View              | Mo ta                    |
|---------------------------------|-------------------|--------------------------|
| /Student/Courses                | Index.cshtml      | DS khoa hoc dang mo      |
| /Student/Courses/Details/{id}   | Details.cshtml    | Chi tiet + DS lop        |
| /Student/Enroll/{classId}       | Confirm.cshtml    | Xac nhan dang ky         |
| /Student/Enrollments            | Index.cshtml      | DS dang ky cua toi       |

### 4.3 Lich hoc

**MVC Client:**
| Route              | View              | Mo ta                         |
|--------------------|-------------------|-------------------------------|
| /Student/Schedule  | Index.cshtml      | Lich hoc theo tuan (calendar) |

### 4.4 Xem diem danh

**MVC Client:**
| Route                                  | View              | Mo ta                |
|----------------------------------------|-------------------|----------------------|
| /Student/Attendance                    | Index.cshtml      | Chon lop xem         |
| /Student/Attendance/Details/{classId}  | Details.cshtml    | Chi tiet diem danh   |

### 4.5 Xem diem

**Server API:**
| API Endpoint                   | Method | Mo ta                     |
|--------------------------------|--------|---------------------------|
| /api/grades/my                 | GET    | Diem cua toi (tat ca lop) |
| /api/grades/my/{classId}       | GET    | Diem cua toi theo lop     |

**MVC Client:**
| Route                          | View              | Mo ta                    |
|--------------------------------|-------------------|--------------------------|
| /Student/Grades                | Index.cshtml      | Bang diem tong hop       |
| /Student/Grades/{classId}      | Details.cshtml    | Bang diem chi tiet lop   |

### 4.6 Hoc phi

**MVC Client:**
| Route                  | View              | Mo ta                        |
|------------------------|-------------------|------------------------------|
| /Student/Payments      | Index.cshtml      | Lich su thanh toan + cong no |

### 4.7 Danh gia giao vien

**Server API:**
| API Endpoint               | Method | Mo ta                     |
|----------------------------|--------|---------------------------|
| /api/reviews               | POST   | Gui danh gia GV           |
| /api/reviews/my            | GET    | DS danh gia da gui        |

**MVC Client:**
| Route                              | View              | Mo ta                |
|------------------------------------|-------------------|----------------------|
| /Student/Reviews/{classId}         | Create.cshtml     | Form danh gia GV     |
| /Student/Reviews                   | Index.cshtml      | DS danh gia da gui   |

### 4.8 Thong bao

**MVC Client:**
| Route                          | View              | Mo ta                    |
|--------------------------------|-------------------|--------------------------|
| /Student/Notifications         | Index.cshtml      | DS thong bao nhan duoc   |
| /Student/Notifications/{id}    | Details.cshtml    | Chi tiet thong bao       |

---

## PHASE 5: Shared Components (Dung chung)

### 5.1 Layout & Navigation
| Component          | Mo ta                                              |
|--------------------|----------------------------------------------------|
| _Layout.cshtml     | Layout chinh: header, sidebar, footer              |
| _StaffNav.cshtml   | Menu rieng cho Staff                               |
| _TeacherNav.cshtml | Menu rieng cho Teacher                             |
| _StudentNav.cshtml | Menu rieng cho Student                             |
| _LoginPartial.cshtml| Hien thi ten user + logout                        |

### 5.2 Cau truc thu muc Views
```
Views/
├── Shared/
│   ├── _Layout.cshtml
│   ├── _StaffNav.cshtml
│   ├── _TeacherNav.cshtml
│   ├── _StudentNav.cshtml
│   └── _LoginPartial.cshtml
├── Auth/
│   ├── Login.cshtml
│   ├── Register.cshtml
│   └── Profile.cshtml
├── Staff/
│   ├── Dashboard.cshtml
│   ├── Courses/  (Index, Create, Edit, Details)
│   ├── Classes/  (Index, Create, Edit, Details)
│   ├── Rooms/    (Index, Create, Edit)
│   ├── Teachers/ (Index, Create, Edit, Details)
│   ├── Students/ (Index, Create, Edit, Details)
│   ├── Enrollments/ (Index, Details)
│   ├── Payments/ (Index, Create, Report, Debts)
│   ├── Notifications/ (Index, Create)
│   └── Reports/  (Index, Attendance, Revenue)
├── Teacher/
│   ├── Dashboard.cshtml
│   ├── Classes/  (Index, Details)
│   ├── Schedule/ (Index)
│   ├── Attendance/ (Index, Take, History)
│   ├── Grades/   (Index, Enter, Edit)
│   └── Notifications/ (Index, Create)
└── Student/
    ├── Dashboard.cshtml
    ├── Courses/  (Index, Details)
    ├── Enrollments/ (Index, Confirm)
    ├── Schedule/ (Index)
    ├── Attendance/ (Index, Details)
    ├── Grades/   (Index, Details)
    ├── Payments/ (Index)
    ├── Reviews/  (Index, Create)
    └── Notifications/ (Index, Details)
```

### 5.3 Cau truc thu muc Server API Controllers
```
Server/
├── Controllers/
│   ├── AuthController.cs
│   ├── CoursesController.cs
│   ├── ClassesController.cs
│   ├── RoomsController.cs
│   ├── TeachersController.cs
│   ├── StudentsController.cs
│   ├── EnrollmentsController.cs
│   ├── AttendancesController.cs
│   ├── GradesController.cs
│   ├── PaymentsController.cs
│   ├── NotificationsController.cs
│   ├── ReviewsController.cs
│   └── ReportsController.cs
├── Services/
│   ├── JwtService.cs
│   └── ... (business logic services)
├── DTOs/
│   ├── Auth/ (LoginDto, RegisterDto, TokenDto...)
│   ├── Course/ (CourseDto, CreateCourseDto...)
│   ├── Class/ (ClassDto, CreateClassDto...)
│   └── ... (DTO cho tung module)
├── Models/
│   └── (13 entity models da tao)
└── Data/
    ├── LMMDbContext.cs
    └── DbSeeder.cs
```

---

## PHASE 6: Cac tinh nang nang cao (Optional)

| Tinh nang                | Mo ta                                        |
|--------------------------|----------------------------------------------|
| Xuat PDF/Excel           | Xuat bang diem, bao cao ra file              |
| Upload Avatar            | Upload anh dai dien cho user                 |
| Thong bao real-time      | Dung SignalR de push thong bao               |
| Tim kiem & Filter        | Tim kiem hoc vien, lop, khoa hoc             |
| Phan trang               | Phan trang cho cac danh sach                 |
| Bieu do thong ke         | Chart.js cho dashboard va bao cao            |
| Email notification       | Gui email nhac hoc phi, thay doi lich        |
| Lich hoc dang Calendar   | Hien thi lich hoc dang lich (FullCalendar)   |

---

## Thu tu trien khai khuyen nghi

| Thu tu | Module                           | Uu tien |
|--------|----------------------------------|---------|
| 1      | Authentication (Login/Register)  | Cao     |
| 2      | Staff Dashboard                  | Cao     |
| 3      | Quan ly Khoa hoc                 | Cao     |
| 4      | Quan ly Phong hoc                | Cao     |
| 5      | Quan ly Giao vien                | Cao     |
| 6      | Quan ly Hoc vien                 | Cao     |
| 7      | Quan ly Lop hoc                  | Cao     |
| 8      | Dang ky hoc (Student)            | Trung binh |
| 9      | Duyet dang ky (Staff)            | Trung binh |
| 10     | Lich hoc (Teacher + Student)     | Trung binh |
| 11     | Diem danh (Teacher)              | Trung binh |
| 12     | Nhap/Xem diem                    | Trung binh |
| 13     | Quan ly hoc phi                  | Trung binh |
| 14     | Thong bao                        | Thap    |
| 15     | Danh gia giao vien               | Thap    |
| 16     | Bao cao & Thong ke               | Thap    |
| 17     | Tinh nang nang cao               | Thap    |

---

## Tai khoan test mac dinh

| Role    | Email            | Password    |
|---------|------------------|-------------|
| Staff   | admin@lmm.com   | Admin@123   |
| Teacher | teacher@lmm.com | Teacher@123 |
| Student | student@lmm.com | Student@123 |
