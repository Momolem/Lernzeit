using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FunicularSwitch;
using Lernzeit.Application.Contracts;
using Lernzeit.Domain.Calendar;

namespace Lernzeit.RaumzeitAPI;

public class RaumzeitService : ICalendarService
{
    private readonly HttpClient httpClient;
    private readonly IRaumzeitTokenRepository raumzeitTokenRepository;
    private readonly TimeProvider timeProvider;

    public RaumzeitService(
        HttpClient httpClient, IRaumzeitTokenRepository raumzeitTokenRepository, TimeProvider timeProvider)
    {
        this.httpClient = httpClient;
        this.raumzeitTokenRepository = raumzeitTokenRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<string>> GetToken(string username, string password)
    {
        var authRequest = new AuthenticationRequest(username, password);

        var response =
            await this.httpClient.PostAsJsonAsync("https://raumzeit.hka-iwi.de/private/api/v1/authentication",
                authRequest);
        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
            return authResponse?.AccessToken ?? string.Empty;
        }

        return Result.Error("Authentication unsuccessful");
    }

    public async Task<Result<Unit>> Login(string userId, string username, string password)
        => await
            from token in await GetToken(username, password)
            from unit in this.raumzeitTokenRepository.SaveRaumzeitToken(
                userId: new Guid(userId),
                token: token,
                expiration: timeProvider.GetUtcNow().AddDays(185))
            select unit;

    public async Task<Result<Calendar>> GetPersonalCalendar(Guid userId)
        => await
            from token in this.raumzeitTokenRepository.GetRaumzeitToken(userId)
            from iCalStream in RequestRaumzeitICal(token)
            from calendar in Ical.Net.Calendar.Load(iCalStream).ToOption().ToResult(() => "Error Reading ICal")
            select MapToDomain(calendar);

    
    private async Task<Result<Stream>> RequestRaumzeitICal(string token)
    {
        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await this.httpClient.GetAsync("https://raumzeit.hka-iwi.de/cal");

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return Result.Error($"Error getting raumzeit calendar: {response.ReasonPhrase}");
        }

        return Result.Ok(await response.Content.ReadAsStreamAsync());
    }

    private Calendar MapToDomain(Ical.Net.Calendar calendar)
    {
        var events = calendar.Events;
        return new Calendar(CalendarType.Raumzeit, "Raumzeit", events.Select(e => new Event(e.Summary,
            new DateTimeOffset(e.Start.AsUtc, TimeSpan.FromHours(0)),
            new DateTimeOffset(e.End.AsUtc, TimeSpan.FromHours(0)))).ToImmutableList());
    }

    private record AuthenticationRequest(string Login, string Password);

    private record AuthenticationResponse(string AccessToken, string TokenExpiration);
}