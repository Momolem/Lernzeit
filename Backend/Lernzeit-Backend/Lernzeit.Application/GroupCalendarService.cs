using FunicularSwitch;
using FunicularSwitch.Extensions;
using Lernzeit.Application.Contracts;
using Lernzeit.Domain.Calendar;

namespace Lernzeit.Application;

public class GroupCalendarService
{
    private readonly ICalendarService calendarService;
    private readonly IGroupRepository groupRepository;

    public GroupCalendarService(ICalendarService calendarService, IGroupRepository groupRepository)
    {
        this.calendarService = calendarService;
        this.groupRepository = groupRepository;
    }

    public async Task<Option<Calendar>> GetGroupCalendar(Guid groupId)
    {
        var groupOption = await groupRepository.GetGroupById(groupId);

        return await groupOption.Bind(async group =>
        {
            var userIds = group.Members.Select(m => m.Id);
            return (await userIds.SelectAsync(id => this.calendarService.GetPersonalCalendar(id))).Aggregate()
                .ToOption().Map(cals => (group, cals));
            
        })
        .Map(tuple =>
        {
            var (group, userCals) = tuple;
            var busy = userCals.SelectMany(c => c.Events).OrderBy(e => e.Start).ToList();

            if (!busy.Any())
            {
                return CreateGroupCalendar(group.Name, []);
            }

            var merged = new List<Event>();
            
            foreach (var e in busy)
            {
                if (!merged.Any() || e.Start > merged.Last().End)
                {
                    merged.Add(e with { Name = "" }); // remove name from event
                }
                else
                {
                    var last = merged.Last();
                    merged[^1] = last with
                    {
                        End = e.End > last.End ? e.End : last.End
                    };
                }
            }

            return CreateGroupCalendar(group.Name, merged);
        });

    }

    private Calendar CreateGroupCalendar(string name, IEnumerable<Event> events)
    {
        return new Calendar(CalendarType.Group, name, events.ToList());
    }
    
}