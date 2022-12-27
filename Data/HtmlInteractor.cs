using Microsoft.JSInterop;

namespace SerbleAi.Data; 

public class HtmlInteractor {
    private readonly IJSRuntime _jsRuntime;
    
    public HtmlInteractor(IJSRuntime jsRuntime) {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> GetValue(string id) {
        return await _jsRuntime.InvokeAsync<string>("eval", "document.getElementById('" + id + "').value");
    }
    
    public async Task<string> GetHtml(string id) {
        return await _jsRuntime.InvokeAsync<string>("eval", "document.getElementById('" + id + "').innerHTML");
    }
    
    public async Task<string> GetText(string id) {
        return await _jsRuntime.InvokeAsync<string>("eval", "document.getElementById('" + id + "').innerText");
    }
    
    public async Task SetValue(string id, string value) {
        await _jsRuntime.InvokeVoidAsync("eval", "document.getElementById('" + id + "').value = '" + value + "'");
    }
    
    public async Task SetHtml(string id, string value) {
        await _jsRuntime.InvokeVoidAsync("eval", "document.getElementById('" + id + "').innerHTML = '" + value + "'");
    }
    
    public async Task SubmitForm(string id) {
        await _jsRuntime.InvokeVoidAsync("eval", "document.getElementById('" + id + "').submit()");
    }
    
    public async Task Log(string value) {
        await _jsRuntime.InvokeVoidAsync("eval", "console.log('" + value + "')");
    }
    
    public async Task<string> GetStringVariable(string name) {
        return await _jsRuntime.InvokeAsync<string>("eval", "window." + name);
    }

    public async Task SaveToLocalStorage(string key, string value) {
        await _jsRuntime.InvokeVoidAsync("eval", "localStorage.setItem('" + key + "', '" + value + "')");
    }
    
    public async Task<string> GetFromLocalStorage(string key) {
        return await _jsRuntime.InvokeAsync<string>("eval", "localStorage.getItem('" + key + "')");
    }
    
    public async Task InvokeFunction(string functionName, params object[] args) {
        await _jsRuntime.InvokeVoidAsync(functionName, args);
    }
    
    public async Task InvokeCode(string code) {
        await _jsRuntime.InvokeVoidAsync("eval", code);
    }
    
    public async Task<string> GetTimeZone() {
        return await _jsRuntime.InvokeAsync<string>("eval", "Intl.DateTimeFormat().resolvedOptions().timeZone");
    }
    
    public async Task<string> GetLanguage() {
        return await _jsRuntime.InvokeAsync<string>("eval", "navigator.language");
    }
    
    public async Task<string> GetDateTime() {
        return await _jsRuntime.InvokeAsync<string>("eval", "new Date().toISOString()");
    }
    
    public async Task<bool> GetDaylightSavings() {
        return await _jsRuntime.InvokeAsync<bool>("eval", "new Date().getTimezoneOffset() < new Date(new Date().getFullYear(), 0, 1).getTimezoneOffset()");
    }

}