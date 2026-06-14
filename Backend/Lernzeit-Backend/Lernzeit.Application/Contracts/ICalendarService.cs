using FunicularSwitch;

namespace Lernzeit.Application.Contracts;

public interface ICalendarService
{
    Task<Result<Calendar>> GetPersonalCalendar(Guid userId);
}