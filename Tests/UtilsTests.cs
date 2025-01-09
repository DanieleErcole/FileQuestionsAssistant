using Core.Utils;
using Core.Utils.Converters;
using Tests.Utils;

namespace Tests;

[TestFixture]
public class UtilsTests {
         
    [TestCase("FFFFFF", 255, 255, 255)]
    [TestCase("000000", 0, 0, 0)]
    [TestCase("FF0000", 255, 0, 0)]
    [TestCase("00FF00", 0, 255, 0)]
    [TestCase("0000FF", 0, 0, 255)]
    public void NumberHelper_HexStringToRgb_TestCase(string hex, int r, int g, int b) => Assert.That((r, g, b), Is.EqualTo(hex.HexStringToRgb()));

    [TestCase(2.30, 2.30, true)]
    [TestCase(2.30, 2.302222, true)]
    [TestCase(2.30, 2.31, false)]
    [TestCase(null, 5.22, false)]
    public void NumberHelper_DoubleEquals_TestCase(double? d1, double? d2, bool expected) => Assert.That(d1.DoubleEquals(d2), Is.EqualTo(expected));

    [TestCase("Document1.docx")]
    [TestCase("Presentation1.pptx")]
    public void MemoryFile_TestCase(string filename) {
        using var stream = File.OpenRead(TestConstants.TestFilesDirectory + filename);
        using var memStream = new MemoryStream();
        stream.CopyTo(memStream);
        
        var mem = new MemoryFile(filename, memStream.ToArray());
        Assert.That(mem.Data.LongLength, Is.EqualTo(stream.Length));
    }
    
    [Test]
    public void MemoryFile_Equals() {
        const string name = "Presentation1.pptx";
        
        using var stream = File.OpenRead(TestConstants.TestFilesDirectory + name);
        using var memStream = new MemoryStream();
        stream.CopyTo(memStream);
        
        var mem1 = new MemoryFile(name, memStream.ToArray());
        memStream.Position = 0;
        var mem2 = new MemoryFile(name, memStream.ToArray());
        
        Assert.That(mem1, Is.EqualTo(mem2));
    }

    [TestCase(6120000, 17, true)]
    [TestCase(null, 10, false)]
    public void EMUsHelper_ToCentimeters(long? emus, double centimeters, bool expected) =>
        Assert.That(EMUsHelper.ToCentimeters(emus).DoubleEquals(centimeters), Is.EqualTo(expected));
    
    [TestCase(17, 6120000, true)]
    [TestCase(null, 8910000, false)]
    public void EMUsHelper_FromCentimeters(double? centimeters, double emus, bool expected) =>
        Assert.That(EMUsHelper.FromCentimeters(centimeters).DoubleEquals(emus), Is.EqualTo(expected));

    [TestCase(600000, 10, true)]
    [TestCase(1200000, 20, true)]
    [TestCase(null, null, true)]
    [TestCase(600000, 5, false)]
    [TestCase(1200000, null, false)]
    [TestCase(null, 5, false)]
    public void DegreesHelper_ToDegrees(int? value, int? degrees, bool expected) =>
        Assert.That(DegreesHelper.ToDegrees(value) == degrees, Is.EqualTo(expected));
    
    [TestCase(10, "10 °", true)]
    [TestCase(75, "75 °", true)]
    [TestCase(null, null, true)]
    [TestCase(null, "10 °", false)]
    [TestCase(20, "15 °", false)]
    public void DegreesHelper_ToDegreeString(int? degrees, string? str, bool expected) =>
        Assert.That(DegreesHelper.ToDegreeString(degrees) == str, Is.EqualTo(expected));

}