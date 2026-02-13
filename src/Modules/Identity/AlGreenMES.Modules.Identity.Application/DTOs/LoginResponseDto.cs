namespace AlGreenMES.Modules.Identity.Application.DTOs;

public record LoginResponseDto(
    string Token,
    string RefreshToken,
    UserDto User);
