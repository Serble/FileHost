using System.Text.Json.Serialization;

namespace FileHostingApi.Data.Schemas; 

public class User {
    
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("verifiedEmail")]
    public bool VerifiedEmail { get; set; }

    // 0=Disabled Account 1=Normal, 2=Admin
    [JsonPropertyName("permLevel")]
    public int? PermLevel { get; set; }
    
    [JsonPropertyName("permString")]
    public string? PermString { get; set; }
    
    [JsonPropertyName("premiumLevel")]
    public int PremiumLevel { get; set; }
    
    [JsonPropertyName("authorizedApps")]
    public object[]? AuthorizedApps { get; set; }
    
    [JsonPropertyName("stripeCustomerId")]
    [Obsolete("This is no longer provided.")]
    public string? StripeCustomerId { get; set; }

}