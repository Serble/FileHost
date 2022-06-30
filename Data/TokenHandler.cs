using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FileHostingApi.Data; 

public class TokenHandler {
    private readonly Dictionary<string, string> _config;

    public TokenHandler(Dictionary<string, string> config) {
        _config = config;
    }
    
    public string GenerateToken(long maxUpload, bool isAdmin = false) {
        string mySecret = _config["token_secret"];
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(new[] {
                new Claim("maxUpload", maxUpload.ToString()),
                new Claim("isAdmin", isAdmin.ToString())
            }),
            Expires = DateTime.Now.AddYears(1),
            Issuer = _config["token_issuer"],
            Audience = _config["token_audience"],
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
        };
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
        
    public bool ValidateCurrentToken(string? token, out long maxUpload, out bool isAdmin) {
        string mySecret = _config["token_secret"];
        SymmetricSecurityKey mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken validToken;
        try {
            tokenHandler.ValidateToken(token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["token_issuer"],
                ValidAudience = _config["token_audience"],
                IssuerSigningKey = mySecurityKey
            }, out validToken);
        }
        catch (Exception) {
            maxUpload = -1;
            isAdmin = false;
            return false;
        }
        JwtSecurityTokenHandler tokenHandler2 = new JwtSecurityTokenHandler();
        JwtSecurityToken? securityToken = tokenHandler2.ReadToken(token) as JwtSecurityToken;
        string? maxup = securityToken?.Claims.Aggregate<Claim, string>(null!, (current, claim) => claim.Type switch {
            "maxUpload" => claim.Value,
            _ => current
        });
        if (!long.TryParse(maxup, out maxUpload)) {
            // wtf
            maxUpload = -1;
        }
        
        string? admin = securityToken?.Claims.Aggregate<Claim, string>(null!, (current, claim) => claim.Type switch {
            "isAdmin" => claim.Value,
            _ => current
        });
        if (!bool.TryParse(admin, out isAdmin)) {
            // wtf
            isAdmin = false;
        }
        
        return true;
    }
        
}