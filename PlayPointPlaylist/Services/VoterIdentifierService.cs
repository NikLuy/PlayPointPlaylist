using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace PlayPointPlaylist.Services;

public class VoterIdentifierService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public VoterIdentifierService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string GetVoterIdentifier()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return GenerateRandomIdentifier();
        
        // Combine multiple factors for fingerprinting
        var factors = new StringBuilder();
        
        // IP Address
        var ipAddress = GetClientIpAddress();
        factors.Append(ipAddress);
        factors.Append('|');
        
        // User Agent
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        factors.Append(userAgent);
        factors.Append('|');
        
        // Accept Language
        var acceptLanguage = httpContext.Request.Headers["Accept-Language"].ToString();
        factors.Append(acceptLanguage);
        factors.Append('|');
        
        // Accept Encoding
        var acceptEncoding = httpContext.Request.Headers["Accept-Encoding"].ToString();
        factors.Append(acceptEncoding);
        
        // Create hash
        return ComputeHash(factors.ToString());
    }
    
    public string GetClientIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return "unknown";
        
        // Check for X-Forwarded-For header (proxy/load balancer)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',');
            return ips[0].Trim();
        }
        
        // Check for X-Real-IP header
        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
            return realIp;
        
        // Use RemoteIpAddress
        var remoteIp = httpContext.Connection.RemoteIpAddress;
        return remoteIp?.ToString() ?? "unknown";
    }
    
    private string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
    
    private string GenerateRandomIdentifier()
    {
        return Guid.NewGuid().ToString();
    }
}
