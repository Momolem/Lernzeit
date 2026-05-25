namespace Lernzeit.Application.Contracts;

public interface ICalendarRepository
{
    public Task<object> GetUserCalendar();
}