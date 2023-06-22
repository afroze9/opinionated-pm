namespace Nexus.Common.UnitTests;

public class HttpResponseModelTests
{
    [Fact]
    public void HttpResponseModel_WithSuccess_ReturnsCorrectOutput()
    {
        HttpResponseModel<string> sut = HttpResponseModel<string>.Success("data");

        Assert.Equal("data", sut.Data);
        Assert.False(sut.IsError);
        Assert.Empty(sut.ErrorMessage);
    }

    [Fact]
    public void HttpResponseModel_WithFailure_ReturnsCorrectOutput()
    {
        HttpResponseModel<string> sut = HttpResponseModel<string>.Failure("data", "error");

        Assert.Equal("data", sut.Data);
        Assert.True(sut.IsError);
        Assert.Equal("error", sut.ErrorMessage);
    }
}