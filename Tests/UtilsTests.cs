using Core.Utils.Converters;

namespace Tests;

[TestFixture]
public class UtilsTests {
         
    [TestCase("FFFFFF", 255, 255, 255)]
    [TestCase("000000", 0, 0, 0)]
    [TestCase("FF0000", 255, 0, 0)]
    [TestCase("00FF00", 0, 255, 0)]
    [TestCase("0000FF", 0, 0, 255)]
    public void HexStringToRgb_TestCase(string hex, int r, int g, int b) => Assert.That((r, g, b), Is.EqualTo(hex.HexStringToRgb()));

}