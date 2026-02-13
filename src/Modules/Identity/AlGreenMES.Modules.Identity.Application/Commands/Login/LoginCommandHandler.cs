using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Identity.Application.DTOs;
using AlGreenMES.Modules.Identity.Application.Interfaces;
using AlGreenMES.Modules.Identity.Application.Services;
using AlGreenMES.Modules.Identity.Domain.Repositories;
using Mapster;
using RefreshTokenEntity = AlGreenMES.Modules.Identity.Domain.Entities.RefreshToken;
using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITenantLookupService _tenantLookupService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IIdentityUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ITenantLookupService tenantLookupService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _tenantLookupService = tenantLookupService;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantLookupService.GetTenantByCodeAsync(request.TenantCode, cancellationToken)
            ?? throw new NotFoundException("Tenant", request.TenantCode);

        if (!tenant.IsActive)
            throw new DomainException("TENANT_INACTIVE", "The tenant is not active.");

        var user = await _userRepository.GetByEmailAsync(request.Email, tenant.Id, cancellationToken)
            ?? throw new DomainException("INVALID_CREDENTIALS", "Invalid email or password.");

        if (!user.IsActive)
            throw new DomainException("USER_INACTIVE", "The user account is not active.");

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new DomainException("INVALID_CREDENTIALS", "Invalid email or password.");

        var token = _jwtTokenService.GenerateToken(user);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = RefreshTokenEntity.Create(
            tenant.Id,
            user.Id,
            refreshTokenValue,
            DateTime.UtcNow.AddDays(7));

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = user.Adapt<UserDto>();

        return new LoginResponseDto(token, refreshTokenValue, userDto);
    }
}
