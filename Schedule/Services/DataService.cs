using Schedule.DTO;
using Schedule.Models;

namespace Schedule.Services;

public class DataService
{
    private readonly ScheduleManagementContext _context;
    private readonly ValidationService _validationService;

    public DataService(ScheduleManagementContext context, ValidationService validationService)
    {
        _context = context;
        _validationService = validationService;
    }


    public List<string> AddDataToDb(CsvDataDTO csvData)
    {
        List<string> messages = _validationService.MessageValidateData(csvData);

        if (messages.Count > 0)
        {
            return messages;
        }

        Models.Schedule schedule = GetScheduleFromData(csvData);

        List<string> validationMessages = _validationService.MessageValidateData(schedule);
        
        if (validationMessages.Count > 0)
        {
            messages.AddRange(validationMessages);
            return messages;
        }

        try
        {
            _context.Schedules.Add(schedule);
            _context.SaveChanges();
        }
        catch(Exception ex)
        {
            messages.Add($"An error occurred while saving the schedule: {ex.Message}");
            return messages;
        }

        messages.Add("Saved successfully!");
        return messages;
    }

    private Models.Schedule GetScheduleFromData(CsvDataDTO data)
    {
        var schedule = new Models.Schedule
        {
            RoomId = _context.Rooms.FirstOrDefault(r => r.Code == data.Room)!.Id,
            SubjectId = _context.Subjects.FirstOrDefault(s => s.Code == data.Subject)!.Id,
            ClassId = _context.GroupClasses.FirstOrDefault(c => c.Code == data.Class)!.Id,
            TeacherId = _context.Teachers.FirstOrDefault(t => t.Code == data.Teacher)!.Id,
            SlotId = _context.Slots.FirstOrDefault(s => s.SlotName == data.Slot)!.Id
        };

        return schedule;
    }

    public void DeleteAllData()
    {
        var schedules = _context.Schedules.ToList();
        _context.Schedules.RemoveRange(schedules);
        _context.SaveChanges();
    }
}