using Client.Services;
using Client.Services.Models;
using Client.ViewModels.Teacher;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

[RequireRole("Teacher")]
public class TeacherScheduleController : BaseController
{
    private readonly IClassApiClient _classApi;

    public TeacherScheduleController(IClassApiClient classApi) => _classApi = classApi;

    public async Task<IActionResult> Index()
    {
        var token = GetToken()!;
        var userId = GetUserId()!;
        var result = await _classApi.GetAllAsync(token, userId);
        var selectedDate = ResolveSelectedDate(Request.Query["date"]);
        var selectedDow = NormalizeDayOfWeek((int)selectedDate.DayOfWeek);
        var activeClasses = result.Data?
            .Where(c => c.Status == 0 || c.Status == 1)
            .ToList() ?? new List<ClassDto>();

        var slots = BuildSlots(selectedDate, activeClasses, selectedDow);
        var model = new TeacherDailyScheduleViewModel
        {
            SelectedDate = selectedDate,
            DayTitle = GetDayTitle(selectedDow),
            Slots = slots
        };

        return View(model);
    }

    private static DateTime ResolveSelectedDate(string? dateQuery)
    {
        if (!string.IsNullOrWhiteSpace(dateQuery) && DateTime.TryParse(dateQuery, out var parsed))
            return parsed.Date;
        return DateTime.Today;
    }

    private static int NormalizeDayOfWeek(int dayOfWeek)
    {
        var normalized = dayOfWeek % 7;
        if (normalized < 0) normalized += 7;
        return normalized;
    }

    private static string GetDayTitle(int dayOfWeek) => dayOfWeek switch
    {
        1 => "Thứ 2",
        2 => "Thứ 3",
        3 => "Thứ 4",
        4 => "Thứ 5",
        5 => "Thứ 6",
        6 => "Thứ 7",
        _ => "Chủ nhật"
    };

    private static List<TeacherScheduleSlotViewModel> BuildSlots(DateTime selectedDate, List<ClassDto> classes, int selectedDow)
    {
        var slotDefs = new[]
        {
            new { Name = "Sáng", Start = new TimeOnly(8, 30), End = new TimeOnly(10, 30), Break = "—" },
            new { Name = "Chiều", Start = new TimeOnly(13, 30), End = new TimeOnly(15, 30), Break = "Nghỉ trưa ~3 tiếng" },
            new { Name = "Chiều tối", Start = new TimeOnly(16, 30), End = new TimeOnly(18, 30), Break = "Nghỉ 60 phút" },
            new { Name = "Tối", Start = new TimeOnly(19, 0), End = new TimeOnly(21, 0), Break = "Nghỉ 30 phút" }
        };

        var result = slotDefs
            .Select(s => new TeacherScheduleSlotViewModel
            {
                SlotName = s.Name,
                TimeRange = $"{s.Start:HH\\:mm} - {s.End:HH\\:mm}",
                BreakNote = s.Break
            })
            .ToList();
        var outsideSlot = new TeacherScheduleSlotViewModel
        {
            SlotName = "Ngoài khung chuẩn",
            TimeRange = "Theo lịch thực tế",
            BreakNote = "Ca đặc biệt"
        };

        foreach (var cls in classes)
        {
            var isInRange = selectedDate >= cls.StartDate.Date && selectedDate <= cls.EndDate.Date;
            foreach (var schedule in cls.Schedules.Where(s => NormalizeDayOfWeek(s.DayOfWeek) == selectedDow))
            {
                if (!TimeOnly.TryParse(schedule.StartTime, out var schStart) || !TimeOnly.TryParse(schedule.EndTime, out var schEnd))
                    continue;

                var matched = false;
                for (var i = 0; i < slotDefs.Length; i++)
                {
                    if (!IsTimeOverlap(schStart, schEnd, slotDefs[i].Start, slotDefs[i].End))
                        continue;

                    result[i].Classes.Add(new TeacherScheduleClassItemViewModel
                    {
                        ClassId = cls.Id,
                        ClassName = cls.Name,
                        CourseName = cls.CourseName,
                        RoomName = cls.RoomName,
                        StartTime = schedule.StartTime,
                        EndTime = schedule.EndTime,
                        IsInDateRange = isInRange
                    });
                    matched = true;
                    break;
                }

                if (!matched)
                {
                    // Keep sessions visible even when they fall in transition gaps between predefined slots.
                    outsideSlot.Classes.Add(new TeacherScheduleClassItemViewModel
                    {
                        ClassId = cls.Id,
                        ClassName = cls.Name,
                        CourseName = cls.CourseName,
                        RoomName = cls.RoomName,
                        StartTime = schedule.StartTime,
                        EndTime = schedule.EndTime,
                        IsInDateRange = isInRange
                    });
                }
            }
        }

        if (outsideSlot.Classes.Any())
            result.Add(outsideSlot);

        foreach (var slot in result)
        {
            slot.Classes = slot.Classes
                .OrderBy(c => c.StartTime)
                .ThenBy(c => c.ClassName)
                .ToList();
        }

        return result;
    }

    private static bool IsTimeOverlap(TimeOnly aStart, TimeOnly aEnd, TimeOnly bStart, TimeOnly bEnd) =>
        aStart < bEnd && bStart < aEnd;
}
