using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Lernzeit.RaumzeitAPI;

public class RaumzeitService
{
    private readonly HttpClient httpClient;

    public RaumzeitService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<string> GetToken(RaumzeitCredentials credentials)
    {
        var authRequest = new AuthenticationRequest(credentials.Username, credentials.Password);

        var response = await this.httpClient.PostAsJsonAsync("https://raumzeit.hka-iwi.de/private/api/v1/authentication", authRequest);
        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
            return authResponse?.AccessToken ?? string.Empty;
        }
        
        return string.Empty;
    }

    public async Task<string> GetPersonalCalendar(RaumzeitCredentials credentials)
    {
        var token = await GetToken(credentials);
        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await this.httpClient.GetAsync("https://raumzeit.hka-iwi.de/cal");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return await response.Content.ReadAsStringAsync();
        }
        return string.Empty;
    }

    private record AuthenticationRequest(string Login, string Password);
    private record AuthenticationResponse(string AccessToken, string TokenExpiration);
}