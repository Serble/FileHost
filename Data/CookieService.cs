using Microsoft.JSInterop;

namespace FileHostingApi.Data; 

public class CookieService {
    readonly IJSRuntime _jsRuntime;
    string _expires = "";

    public CookieService(IJSRuntime jsRuntime) {
        _jsRuntime = jsRuntime;
        ExpireDays = 300;
    }

    public async Task SetValue(string key, string value, int? days = null) {
        string curExp = days != null ? days > 0 ? DateToUtc(days.Value) : "" : _expires;
        await SetCookie($"{key}={value}; expires={curExp}; path=/");
    }

    public async Task<string> GetValue(string key, string def = "") {
        string cValue = await GetCookie();
        if (string.IsNullOrEmpty(cValue)) return def;                

        string[] cookies = cValue.Split(';');
        foreach (string cookie in cookies) {
            string[] c = cookie.Split('=');
            if (c[0].Trim() == key) return c[1];
        }
        return def;
    }

    private async Task SetCookie(string value) {
        await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{value}\"");
    }

    private async Task<string> GetCookie() {
        return await _jsRuntime.InvokeAsync<string>("eval", $"document.cookie");
    }

    public int ExpireDays {
        set => _expires = DateToUtc(value);
    }

    private static string DateToUtc(int days) => DateTime.Now.AddDays(days).ToUniversalTime().ToString("R");
}