using FileHostingApi.Data.Storage;

namespace FileHostingApi;

using GeneralPurposeLib;
using LogLevel = GeneralPurposeLib.LogLevel;

public static class Program {

    private static ConfigManager? _configManager;
    
    // UPLOAD LIMITS ARE IN MB
    private static readonly Dictionary<string, string> ConfigDefaults = new() {
        { "bind_url", "http://*:5000" },
        { "storage_service", "http" },
        { "http_authorization_token", "my very secure auth token" },
        { "http_url", "https://myverysecurestoragebackend.io/" },
        { "my_host" , "https://theplacewherethisappisaccessable.com/" },
        { "local_store_path", "/filehost" },
        { "not_logged_in_upload_limit", "10" },
        { "logged_in_upload_limit", "50" },
        { "premium_upload_limit", "100" },
        { "token_issuer", "CoPokBl" },
        { "token_audience", "Privileged Users" },
        { "token_secret" , Guid.NewGuid().ToString() },
        { "serble_api_url", "https://api.serble.net/api/v1/" },
        { "serble_appid", "" },
        { "serble_appsecret", "The client secret provided by Serble" },
        { "oauth_url", "https://serble.net/oauth/authorize" }
    };
    public static Dictionary<string, string>? Config;
    public static IStorageService? StorageService;

    private static int Main(string[] args) {
        // Logger
        Logger.Init(LogLevel.Debug);

        // Config
        Logger.Info("Loading config...");
        _configManager = new ConfigManager("config.json", ConfigDefaults);
        Config = _configManager.LoadConfig();
        Logger.Info("Config loaded.");
        
        // Storage service
        StorageService = Config["storage_service"] switch {
            "http" => new HttpStorageService(),
            "local" => new LocalStorageService(),
            _ => throw new Exception("Unknown storage service")
        };

        if (args.Length != 0) {

            switch (args[0]) {
                
                default:
                    Console.WriteLine("Unknown command");
                    return 1;
                
                case "delete":
                    // delete <file>
                    if (args.Length < 2) {
                        Console.WriteLine("Missing file name");
                        return 1;
                    }

                    try {
                        StorageService.DeleteFile(args[1]);
                    }
                    catch (Exception e) {
                        Console.WriteLine("Failed: " + e.Message);
                        return 1;
                    }
                    Console.WriteLine("File deleted");
                    return 0;
                
                case "exists":
                    // exists <file>
                    if (args.Length < 2) {
                        Console.WriteLine("Missing file name");
                        return 1;
                    }

                    Stream? file;
                    try {
                        file = StorageService.GetFile(args[1]).Result;
                    }
                    catch (Exception e) {
                        Console.WriteLine("Failed: " + e.Message);
                        return 1;
                    }
                    Console.WriteLine(file == null ? "File not found" : "File found");
                    return 0;
                
                case "upload":
                    // upload <file> <name>
                    if (args.Length < 3) {
                        Console.WriteLine("Missing file name");
                        return 1;
                    }
                    
                    // load file into stream
                    Stream? fileStream;
                    try {
                        fileStream = File.OpenRead(args[1]);
                    }
                    catch (Exception e) {
                        Console.WriteLine("Failed: " + e.Message);
                        return 1;
                    }
                    try {
                        StorageService.UploadFile(args[1], fileStream);
                    }
                    catch (Exception e) {
                        Console.WriteLine("Failed: " + e.Message);
                        return 1;
                    }
                    Console.WriteLine("File uploaded");
                    return 0;

            }
        }

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSingleton<HttpStorageService>();
        builder.Services.AddControllers();
        builder.WebHost.UseUrls(Config["bind_url"]);

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        bool didError = false;
        try {
            app.Run();
        }
        catch (Exception e) {
            Logger.Error(e);
            didError = true;
        }

        Logger.WaitFlush();
        return didError ? 1 : 0;
    }
    
}