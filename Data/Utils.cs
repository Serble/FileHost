using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FileHostingApi.Data; 

public static class Utils {
    
    public static string Hash(string str) {
        StringBuilder builder = new StringBuilder();
        foreach (byte t in SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(str))) {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString();
    }

    public static TR? IfNotNull<T, TR>(this T? value, Func<T, TR?> func) {
        return value == null ? default : func.Invoke(value);
    }

    public static T RunTaskSync<T>(this Task<T> task) {
        while (!task.IsCompleted) {
            // Just wait
        }
        return task.Result;
    }

    public static T? DefaultIfFalse<T>(this T? value, Func<T?, bool> condition) {
        if (value == null) {
            return default;
        }
        return condition.Invoke(value) ? value : default;
    }

    public static string ToFormat(this TimeSpan timeSpan, string format) {
        string result = format;
        result = result.Replace("{d}", timeSpan.Days.ToString());
        result = result.Replace("{h}", timeSpan.Hours.ToString());
        result = result.Replace("{m}", timeSpan.Minutes.ToString());
        result = result.Replace("{s}", timeSpan.Seconds.ToString());
        result = result.Replace("{ms}", timeSpan.Milliseconds.ToString());
        return result;
    }
    
    // get all query values
    public static NameValueCollection GetQueryStrings(this NavigationManager navigationManager) {
        return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
    }

    // get single querystring value with specified key
    public static string? GetQueryStrings(this NavigationManager navigationManager, string key) {
        return navigationManager.GetQueryStrings()[key];
    }
    
    public static MarkupString MarkupString(this string str) {
        return (MarkupString) str;
    }

}

public class Cookie {
    readonly IJSRuntime _jsRuntime;
    string _expires = "";

    public Cookie(IJSRuntime jsRuntime) {
        _jsRuntime = jsRuntime;
        ExpireDays = 300;
    }

    public async Task SetValue(string key, string value, int? hours = null) {
        string curExp = hours != null ? hours > 0 ? DateToUTC(hours.Value) : "" : _expires;
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
        if (_jsRuntime == null) {
            return "";
        }
        return await _jsRuntime.InvokeAsync<string>("eval", $"document.cookie");
    }

    public int ExpireDays {
        set => _expires = DateToUTC(value);
    }

    private static string DateToUTC(int h) => DateTime.Now.AddHours(h).ToUniversalTime().ToString("R");
}