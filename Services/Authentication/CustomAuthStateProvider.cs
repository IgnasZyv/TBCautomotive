using System.Security.Claims;
using CarHostingWeb.Services.Authentication;
using Microsoft.AspNetCore.Components.Authorization;


namespace CarHostingWeb.Services.Authentication;
public class CustomAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly FirebaseAuthService _firebaseAuthService;
    private bool _initialized = false;

    public CustomAuthStateProvider(FirebaseAuthService firebaseAuthService)
    {
        _firebaseAuthService = firebaseAuthService;
        _firebaseAuthService.AuthStateChanged += OnAuthStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Only initialize once
        if (!_initialized)
        {
            await _firebaseAuthService.InitializeAsync();
            _initialized = true;
        }
        
        var user = GetCurrentUser();
        return new AuthenticationState(user);
    }

    private ClaimsPrincipal GetCurrentUser()
    {
        if (_firebaseAuthService.IsAuthenticated)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, _firebaseAuthService.CurrentUserEmail!),
                new Claim(ClaimTypes.Role, "Admin")
            }, "firebase");

            return new ClaimsPrincipal(identity);
        }

        return new ClaimsPrincipal(new ClaimsIdentity());
    }

    public void ClearUser()
    {
        _ = _firebaseAuthService.SignOut();
    }

    private void OnAuthStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void Dispose()
    {
        _firebaseAuthService.AuthStateChanged -= OnAuthStateChanged;
    }
}