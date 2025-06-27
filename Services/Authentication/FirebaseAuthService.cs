using Microsoft.JSInterop;

namespace CarHostingWeb.Services.Authentication;

public class FirebaseAuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IJSRuntime _jsRuntime;
    
    private string? _currentUserEmail;
    private string? _currentToken;
    
    public event Action? AuthStateChanged;

    public string? CurrentUserEmail => _currentUserEmail;
    public string? CurrentToken => _currentToken;
    public bool IsAuthenticated => !string.IsNullOrEmpty(_currentUserEmail);

    public FirebaseAuthService(HttpClient httpClient, IConfiguration configuration, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Firebase:ApiKey"] ?? throw new Exception("Missing Firebase API key");
        _jsRuntime = jsRuntime;
    }

    // Call this on component initialization to restore auth state
    public async Task InitializeAsync()
    {
        try
        {
            var storedEmail = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "user_email");
            var storedToken = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "user_token");
        
            // Only update if different from current state
            if (storedEmail != _currentUserEmail || storedToken != _currentToken)
            {
                _currentUserEmail = storedEmail;
                _currentToken = storedToken;
            
                // Only trigger event if we actually have auth data
                if (!string.IsNullOrEmpty(_currentUserEmail))
                {
                    AuthStateChanged?.Invoke();
                }
            }
        }
        catch
        {
            // JS interop not available yet (prerendering)
        }
    }

    public async Task<string?> SignInWithEmailAndPasswordAsync(string email, string password)
    {
        var payload = new
        {
            email = email,
            password = password,
            returnSecureToken = true
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}",
            payload
        );

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
            
            _currentToken = result?.IdToken;
            _currentUserEmail = email;
            
            // Store in browser localStorage (per-user)
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user_email", email);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user_token", result?.IdToken);
            
            AuthStateChanged?.Invoke();
            
            return result?.IdToken;
        }

        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Failed to sign in: " + error);
        return null;
    }

    public async Task SignOut()
    {
        _currentUserEmail = null;
        _currentToken = null;
        
        // Clear from browser localStorage
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "user_email");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "user_token");
        
        AuthStateChanged?.Invoke();
    }

    private class FirebaseAuthResponse
    {
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public string LocalId { get; set; }
    }
}