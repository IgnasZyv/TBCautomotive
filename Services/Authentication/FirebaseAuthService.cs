namespace CarHostingWeb.Services.Authentication;

public class FirebaseAuthService(HttpClient httpClient, IConfiguration configuration)
{
    private readonly string _apiKey = configuration["Firebase:ApiKey"] ?? throw new Exception("Missing Firebase API key");
    
    // Add auth state management
    private string? _currentUserEmail;
    private string? _currentToken;
    
    public event Action? AuthStateChanged;

    public string? CurrentUserEmail => _currentUserEmail;
    public string? CurrentToken => _currentToken;
    public bool IsAuthenticated => !string.IsNullOrEmpty(_currentUserEmail);

    public async Task<string?> SignInWithEmailAndPasswordAsync(string email, string password)
    {
        var payload = new
        {
            email = email,
            password = password,
            returnSecureToken = true
        };

        var response = await httpClient.PostAsJsonAsync(
            $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}",
            payload
        );

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();
            
            // Store the auth state
            _currentToken = result?.IdToken;
            _currentUserEmail = email;
            
            // Notify auth state changed
            AuthStateChanged?.Invoke();
            
            return result?.IdToken;
        }

        var error = await response.Content.ReadAsStringAsync();
        Console.WriteLine(@"Failed to sign in: " + error);
        return null;
    }

    public void SignOut()
    {
        _currentUserEmail = null;
        _currentToken = null;
        AuthStateChanged?.Invoke();
    }

    private class FirebaseAuthResponse
    {
        public required string IdToken { get; init; }
        public required string RefreshToken { get; init; }
        public required string LocalId { get; init; }
    }
}