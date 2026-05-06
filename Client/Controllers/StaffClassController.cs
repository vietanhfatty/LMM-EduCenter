using Client.Services;
using Client.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Staff")]
public class StaffClassController : BaseController
{
    private readonly IClassApiClient _classApi;
    private readonly ICourseApiClient _courseApi;
    private readonly IRoomApiClient _roomApi;
    private readonly IUserApiClient _userApi;

    public StaffClassController(
        IClassApiClient classApi,
        ICourseApiClient courseApi,
        IRoomApiClient roomApi,
        IUserApiClient userApi)
    {
        _classApi = classApi;
        _courseApi = courseApi;
        _roomApi = roomApi;
        _userApi = userApi;
    }

    public async Task<IActionResult> Index(string? keyword, int? status, int? courseId, int page = 1, int pageSize = 10)
    {
        var result = await _classApi.GetAllAsync(GetToken()!);
        var classes = result.Data ?? new List<ClassDto>();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var normalized = keyword.Trim().ToLower();
            classes = classes.Where(c =>
                c.Name.ToLower().Contains(normalized) ||
                c.CourseName.ToLower().Contains(normalized) ||
                c.TeacherName.ToLower().Contains(normalized) ||
                c.RoomName.ToLower().Contains(normalized)).ToList();
        }

        if (status.HasValue)
            classes = classes.Where(c => c.Status == status.Value).ToList();

        if (courseId.HasValue)
            classes = classes.Where(c => c.CourseId == courseId.Value).ToList();

        var coursesResult = await _courseApi.GetAllAsync(GetToken()!);
        ViewBag.Courses = coursesResult.Data ?? new List<CourseDto>();

        var totalItems = classes.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var pagedData = classes
            .OrderByDescending(c => c.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.Keyword = keyword;
        ViewBag.Status = status;
        ViewBag.CourseId = courseId;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.TotalPages = totalPages;

        return View(pagedData);
    }

    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View(new CreateClassRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateClassRequest model)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns();
            return View(model);
        }

        var result = await _classApi.CreateAsync(GetToken()!, model);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Tạo lớp học thất bại.");
            await LoadDropdowns();
            return View(model);
        }

        TempData["Success"] = "Tạo lớp học thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _classApi.GetByIdAsync(id, GetToken()!);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = "Không tìm thấy lớp học.";
            return RedirectToAction(nameof(Index));
        }

        var data = result.Data;
        var model = new UpdateClassRequest
        {
            Name = data.Name,
            CourseId = data.CourseId,
            TeacherId = data.TeacherId,
            RoomId = data.RoomId,
            StartDate = data.StartDate,
            EndDate = data.EndDate,
            MaxStudents = data.MaxStudents,
            Status = data.Status,
            Schedules = data.Schedules.Select(s => new CreateScheduleRequest
            {
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime
            }).ToList()
        };

        ViewBag.ClassId = id;
        await LoadDropdowns();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateClassRequest model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ClassId = id;
            await LoadDropdowns();
            return View(model);
        }

        var result = await _classApi.UpdateAsync(id, GetToken()!, model);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage ?? "Cập nhật thất bại.");
            ViewBag.ClassId = id;
            await LoadDropdowns();
            return View(model);
        }

        TempData["Success"] = "Cập nhật lớp học thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _classApi.GetByIdAsync(id, GetToken()!);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = "Không tìm thấy lớp học.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _classApi.DeleteAsync(id, GetToken()!);
        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage ?? "Xóa thất bại.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Xóa lớp học thành công.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns()
    {
        var token = GetToken()!;
        var courses = await _courseApi.GetAllAsync(token);
        var rooms = await _roomApi.GetAllAsync(token);
        var teachers = await _userApi.GetTeachersAsync(token);

        ViewBag.Courses = courses.Data ?? new List<CourseDto>();
        ViewBag.Rooms = rooms.Data ?? new List<RoomDto>();
        ViewBag.Teachers = teachers.Data ?? new List<UserDto>();
    }
}
