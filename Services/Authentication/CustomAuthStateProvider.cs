using System.Security.Claims;
using CarHostingWeb.Services.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace CarHostingWeb.Services.Authentication;

public class CustomAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly FirebaseAuthService _firebaseAuthService;

    public CustomAuthStateProvider(FirebaseAuthService firebaseAuthService)
    {
        _firebaseAuthService = firebaseAuthService;
        _firebaseAuthService.AuthStateChanged += OnAuthStateChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = GetCurrentUser();
        return Task.FromResult(new AuthenticationState(user));
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

    public void SetUser(string email)
    {
        // This is now handled automatically by the FirebaseAuthService
        // when SignInWithEmailAndPasswordAsync succeeds
    }

    public void ClearUser()
    {
        _firebaseAuthService.SignOut();
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