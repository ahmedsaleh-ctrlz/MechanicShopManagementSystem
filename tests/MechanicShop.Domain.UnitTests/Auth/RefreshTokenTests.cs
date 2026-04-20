
using MechanicShop.Domain.Identity;
using MechanicShop.Tests.Common.Auth;
using Microsoft.AspNetCore.Components;

namespace MechanicShop.Domain.UnitTests.Auth;

public class RefreshTokenTests
{
    [Fact]

    public void Create_ShouldSucceed_WhenValidParameters()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        string tokenValue = "Token";
        var expiresOnUtc = DateTimeOffset.UtcNow.AddDays(7);

        // Act
        var result = RefreshToken.Create(id, tokenValue, userId, expiresOnUtc);
        var token = result.Value;

        // Assert
        Assert.NotNull(token);
        Assert.Equal(tokenValue, token.Token);
        Assert.False(string.IsNullOrWhiteSpace(token.UserId));
        Assert.True(token.ExpiresOnUtc > DateTimeOffset.UtcNow);
        Assert.Equal(userId, token.UserId);

        
    }

    [Fact]
    public void Create_ShouldFail_WhenIdIsEmpty()
    {

        // Act
        var result = RefreshTokenFactory.CreateRefreshToken(id: Guid.Empty);
       
        // Assert
        Assert.True(result.IsError);

        Assert.Equal("RefreshToken_Id_Required", result.TopError.Code);

    }


    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateRefreshToken_ShouldFail_WhenTokenInvalid(string? invalidToken)
    {
        var result = RefreshTokenFactory.CreateRefreshToken(token: invalidToken);

        Assert.True(result.IsError);

        Assert.Equal("RefreshToken_Token_Required", result.TopError.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateRefreshToken_ShouldFail_WhenUserIdInvalid(string? invalidUserId)
    {
        var result = RefreshTokenFactory.CreateRefreshToken(userId: invalidUserId);

        Assert.True(result.IsError);

        Assert.Equal("RefreshToken_UserId_Required", result.TopError.Code);
    }

    [Fact]
    public void CreateRefreshToken_ShouldFail_WhenExpiresOnUtcIsInPast()
    {
        var result = RefreshTokenFactory.CreateRefreshToken(expiresOnUtc: DateTimeOffset.UtcNow.AddMinutes(-1));

        Assert.True(result.IsError);

        Assert.Equal("RefreshToken_Expiry_Invalid", result.TopError.Code);
    }
}
