using System.Text;
using System.Text.Json;
using FileHostingApi.Data.Schemas;
using GeneralPurposeLib;

namespace FileHostingApi.Data;
public static class SerbleApiHandler {

    public static async Task<SerbleApiResponse<User>> GetUser(string token) {
        // Send HTTP request to API
        HttpClient client = new();
        client.DefaultRequestHeaders.Add("SerbleAuth", "App " + token);
        HttpResponseMessage response;
        try {
            response = await client.GetAsync(Program.Config!["serble_api_url"] + "account");
        }
        catch (Exception e) {
            return new SerbleApiResponse<User>(false, "Failed: " + e);
        }
        if (!response.IsSuccessStatusCode) {
            return new SerbleApiResponse<User>(false, $"Failed: {response.StatusCode}");
        }
        // Parse response
        string json = await response.Content.ReadAsStringAsync();
        User user;
        try {
            user = JsonSerializer.Deserialize<User>(json).ThrowIfNull();
        }
        catch (Exception e) {
            return new SerbleApiResponse<User>(false, $"Failed to parse response: {e.Message}");
        }
        return new SerbleApiResponse<User>(user);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="oauthCode"></param>
    /// <returns>(Refresh Token, Access Token)</returns>
    public static async Task<SerbleApiResponse<(string, string)>> GetRefreshToken(string oauthCode) {
        // Send HTTP request to API
        HttpClient client = new();
        HttpResponseMessage response;
        try {
            response = await client.PostAsync(Program.Config!["serble_api_url"] + 
                                              $"oauth/token/refresh?" +
                                              $"code={oauthCode}" +
                                              $"&client_id={Program.Config!["serble_appid"]}" +
                                              $"&client_secret={Program.Config!["serble_appsecret"]}" +
                                              $"&grant_type=authorization_code", new StringContent(""));
        }
        catch (Exception e) {
            return new SerbleApiResponse<(string, string)>(false, "Error: " + e);
        }
        if (!response.IsSuccessStatusCode) {
            return new SerbleApiResponse<(string, string)>(false, $"Non Success Code: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
        }
        // Parse response
        string refreshToken;
        string accessToken;
        try {
            JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            refreshToken = doc.RootElement.GetProperty("refresh_token").GetString()!;
            accessToken = doc.RootElement.GetProperty("access_token").GetString()!;
        }
        catch (Exception e) {
            return new SerbleApiResponse<(string, string)>(false, $"Failed to parse response: {e.Message}");
        }
        return new SerbleApiResponse<(string, string)>((refreshToken, accessToken));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="refreshToken">The user's oauth refresh token</param>
    /// <returns>Access Token</returns>
    public static async Task<SerbleApiResponse<string>> GetAccessToken(string refreshToken) {
        // Send HTTP request to API
        HttpClient client = new();
        HttpResponseMessage response;
        try {
            response = await client.PostAsync(Program.Config!["serble_api_url"] + 
                                              $"oauth/token/access?" +
                                              $"refresh_token={refreshToken}" +
                                              $"&client_id={Program.Config!["serble_appid"]}" +
                                              $"&client_secret={Program.Config!["serble_appsecret"]}" +
                                              $"&grant_type=authorization_code", new StringContent(""));
        }
        catch (Exception e) {
            return new SerbleApiResponse<string>(false, "Error: " + e);
        }
        if (!response.IsSuccessStatusCode) {
            return new SerbleApiResponse<string>(false, $"Non Success Code: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
        }
        // Parse response
        string accessToken;
        try {
            JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            accessToken = doc.RootElement.GetProperty("access_token").GetString()!;
        }
        catch (Exception e) {
            return new SerbleApiResponse<string>(false, $"Failed to parse response: {e.Message}");
        }
        return new SerbleApiResponse<string>(accessToken);
    }
    
    public static async Task<SerbleApiResponse<User>> RegisterUser(string username, string password, string recaptchaToken) {
        // Send HTTP request to API
        HttpClient client = new();
        client.DefaultRequestHeaders.Add("SerbleAntiSpam", $"recaptcha {recaptchaToken}");
        HttpResponseMessage response;
        try {
            response = await client.PostAsync(Program.Config!["serble_api_url"] + "account", new StringContent(new {
                username,
                password
            }.ToJson(), Encoding.UTF8, "application/json"));
        }
        catch (Exception e) {
            return new SerbleApiResponse<User>(false, "Failed: " + e);
        }
        if (!response.IsSuccessStatusCode) {
            return new SerbleApiResponse<User>(false, $"Failed: {response.StatusCode} ({await response.Content.ReadAsStringAsync()})");
        }
        // Parse response
        string json = await response.Content.ReadAsStringAsync();
        User user;
        try {
            user = JsonSerializer.Deserialize<User>(json).ThrowIfNull();
        }
        catch (Exception e) {
            return new SerbleApiResponse<User>(false, $"Failed to parse response: {e.Message}");
        }
        return new SerbleApiResponse<User>(user);
    }

}

public class SerbleApiResponse<T> {
    
    public bool Success { get; }
    public T? ResponseObject { get; }
    public string ErrorMessage { get; }
    public string ErrorFlag { get; }
    
    public SerbleApiResponse(T responseObject) {
        ResponseObject = responseObject;
        Success = true;
        ErrorFlag = "";
        ErrorMessage = "";
    }
    
    public SerbleApiResponse(bool success, string errorMessage, string errorFlag = "") {
        Success = false;
        ResponseObject = default;
        ErrorMessage = errorMessage;
        ErrorFlag = errorFlag;
    }
    
    public static implicit operator T(SerbleApiResponse<T> response) {
        return response.ResponseObject.ThrowIfNull();
    }

}