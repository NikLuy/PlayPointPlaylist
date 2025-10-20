using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace PlayPointPlaylist.Services;

public class AdminAuthService
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly IConfiguration _configuration;
    private const string AuthSessionKey = "AdminAuthenticated";
    
    public AdminAuthService(ProtectedSessionStorage sessionStorage, IConfiguration configuration)
    {
        _sessionStorage = sessionStorage;
        _configuration = configuration;
    }
    
    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var result = await _sessionStorage.GetAsync<bool>(AuthSessionKey);
            return result.Success && result.Value;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> LoginAsync(string password)
    {
        var adminPassword = _configuration["AdminSettings:Password"];
        
        if (string.IsNullOrEmpty(adminPassword))
        {
            return false;
        }
        
        if (password == adminPassword)
        {
            await _sessionStorage.SetAsync(AuthSessionKey, true);
            return true;
        }
        
        return false;
    }
    
    public async Task LogoutAsync()
    {
        await _sessionStorage.DeleteAsync(AuthSessionKey);
    }
}
