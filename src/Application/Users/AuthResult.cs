namespace CaseSimulator.Application.Users;

public record AuthResult(
    string AccessToken,
    string RefreshToken
);