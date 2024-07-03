namespace OpenWeatherTest;

public class OpenWeatherTests
{
    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { new List<string> { "CAN", "US", "MEX", "NEWZEA", "AUSTRAL", "BANANA REPUBLIC" }, "NEWZEA" };
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void ShouldReturnNameInList(List<string> distinctList, string expected)
    {
        // Arrange -> static method

        // Act
        var list = new List<string>() { "CAN", "US", "MEX", "NEWZEA", "AUSTRAL", "BANANA REPUBLIC" };
        var actual = OpenWeather.Program.GetNameInList(list);

        // Assert
        Assert.Equal(expected, actual);
    }
}