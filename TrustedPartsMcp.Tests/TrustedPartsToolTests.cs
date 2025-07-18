using System.Threading.Tasks;
using TrustedPartsMcp;
using Xunit;

public class TrustedPartsToolTests
{
    [Fact]
    public async Task GetParts_ReturnsContent()
    {
        // Arrange
        var query = "resistor";

        // Act
        var result = await TrustedPartsTool.GetParts(query);

        // Assert
        Assert.NotEmpty(result);
    }
}
