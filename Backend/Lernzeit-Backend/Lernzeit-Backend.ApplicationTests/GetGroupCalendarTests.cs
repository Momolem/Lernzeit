using AwesomeAssertions;
using FunicularSwitch;
using Lernzeit.Application;
using Lernzeit.Application.Contracts;
using Lernzeit.Domain;
using Lernzeit.Domain.Calendar;
using NSubstitute;

namespace Lernzeit_Backend.ApplicationTests;

public class GetGroupCalendarTests
{
    private readonly IGroupRepository groupRepository = Substitute.For<IGroupRepository>();
    private readonly ICalendarService calendarService = Substitute.For<ICalendarService>();

    private readonly GroupCalendarService sut;

    public GetGroupCalendarTests()
    {
        sut = new GroupCalendarService(calendarService, groupRepository);
    }

    [Fact]
    public async Task Returns_None_When_Group_Does_Not_Exist()
    {
        // Arrange
        var groupId = Guid.NewGuid();

        groupRepository
            .GetGroupById(groupId)
            .Returns(Option<Group>.None);

        // Act
        var result = await sut.GetGroupCalendar(groupId);

        // Assert
        result.Should().BeNone();
    }

    [Fact]
    public async Task Returns_Empty_Group_Calendar_When_No_User_Has_Events()
    {
        // Arrange
        var group = CreateGroup("Team", Guid.NewGuid(), Guid.NewGuid());

        groupRepository
            .GetGroupById(group.Id)
            .Returns(group.ToOption());

        calendarService
            .GetPersonalCalendar(Arg.Any<Guid>())
            .Returns(_ => Task.FromResult(Result.Ok(new Calendar(CalendarType.Group, "personal", []))));

        // Act
        var result = await sut.GetGroupCalendar(group.Id);

        // Assert
        var calendar = result.Should().BeSome().Which;

        calendar.Name.Should().Be("Team");
        calendar.Events.Should().BeEmpty();
    }

    [Fact]
    public async Task Merges_Overlapping_Events_From_Multiple_Users()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();

        var group = CreateGroup("Engineering", user1, user2);

        groupRepository
            .GetGroupById(group.Id)
            .Returns(group.ToOption());

        calendarService.GetPersonalCalendar(user1)
            .Returns(Task.FromResult(
                CreateCalendar(
                    Event(9, 11),
                    Event(14, 15)
                )));

        calendarService.GetPersonalCalendar(user2)
            .Returns(Task.FromResult(
                CreateCalendar(
                    Event(10, 12),
                    Event(15, 16)
                )));

        // Act
        var result = await sut.GetGroupCalendar(group.Id);

        // Assert
        var calendar = result.Should().BeSome().Which;
        calendar.Events.Should().HaveCount(2);
        calendar.Events[0].Start.Hour.Should().Be(9);
        calendar.Events[0].End.Hour.Should().Be(12);

        calendar.Events[1].Start.Hour.Should().Be(14);
        calendar.Events[1].End.Hour.Should().Be(16);

        Assert.Equal(14, calendar.Events[1].Start.Hour);
        Assert.Equal(16, calendar.Events[1].End.Hour);
    }

    [Fact]
    public async Task Keeps_Non_Overlapping_Events_Separate()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();

        var group = CreateGroup("Team", user1, user2);

        groupRepository
            .GetGroupById(group.Id)
            .Returns(group.ToOption());

        calendarService.GetPersonalCalendar(user1)
            .Returns(Task.FromResult(
                CreateCalendar(Event(9, 10))));

        calendarService.GetPersonalCalendar(user2)
            .Returns(Task.FromResult(
                CreateCalendar(Event(13, 14))));

        // Act
        var result = await sut.GetGroupCalendar(group.Id);

        // Assert
        var calendar = result.Should().BeSome().Which;
        calendar.Events.Count.Should().Be(2);
    }

    [Fact]
    public async Task Returns_None_When_Any_Personal_Calendar_Is_Missing()
    {
        // Arrange
        var user1 = Guid.NewGuid();
        var user2 = Guid.NewGuid();

        var group = CreateGroup("Team", user1, user2);

        groupRepository
            .GetGroupById(group.Id)
            .Returns(group.ToOption());

        calendarService.GetPersonalCalendar(user1)
            .Returns(Task.FromResult(CreateCalendar(Event(9, 10))));

        calendarService.GetPersonalCalendar(user2)
            .Returns(Task.FromResult(Result<Calendar>.Error("")));

        // Act
        var result = await sut.GetGroupCalendar(group.Id);

        // Assert
        result.Should().BeNone();
    }

    private static Lernzeit.Domain.Group CreateGroup(
        string name,
        params Guid[] memberIds)
    {
        return new Group(
            Guid.NewGuid(),
            name,
            memberIds.Select(id => new User(id, new(""), "", "", "")).ToList());
    }

    private static Result<Calendar> CreateCalendar(params Event[] events)
        => Result.Ok(new Calendar(CalendarType.Group, "personal", events.ToList()));

    private static Event Event(int startHour, int endHour)
        => new(
            "",
            new DateTime(2026, 6, 14, startHour, 0, 0),
            new DateTime(2026, 6, 14, endHour, 0, 0));
}