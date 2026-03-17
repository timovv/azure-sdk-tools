using Azure.Sdk.Tools.Cli.Helpers;

namespace Azure.Sdk.Tools.Cli.Tests.Helpers;

[TestFixture]
internal class EnvFileHelperTests
{
    private EnvFileHelper _helper = null!;
    private string _tempDir = null!;

    [SetUp]
    public void SetUp()
    {
        _helper = new EnvFileHelper();
        _tempDir = Path.Combine(Path.GetTempPath(), $"envfile-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    private string CreateEnvFile(string content)
    {
        var path = Path.Combine(_tempDir, ".env");
        File.WriteAllText(path, content);
        return path;
    }

    [Test]
    public void ParseEnvFile_ThrowsFileNotFound_WhenFileDoesNotExist()
    {
        Assert.Throws<FileNotFoundException>(() => _helper.ParseEnvFile("/nonexistent/.env"));
    }

    [Test]
    public void ParseEnvFile_ReturnsEmpty_WhenFileIsEmpty()
    {
        var path = CreateEnvFile("");
        var result = _helper.ParseEnvFile(path);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ParseEnvFile_ParsesSimpleKeyValuePairs()
    {
        var path = CreateEnvFile("KEY1=value1\nKEY2=value2");
        var result = _helper.ParseEnvFile(path);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result["KEY1"], Is.EqualTo("value1"));
            Assert.That(result["KEY2"], Is.EqualTo("value2"));
        });
    }

    [Test]
    public void ParseEnvFile_SkipsCommentsAndEmptyLines()
    {
        var path = CreateEnvFile("# This is a comment\n\nKEY=value\n\n# Another comment\n");
        var result = _helper.ParseEnvFile(path);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result["KEY"], Is.EqualTo("value"));
    }

    [Test]
    public void ParseEnvFile_HandlesDoubleQuotedValues()
    {
        var path = CreateEnvFile("KEY=\"my value with spaces\"");
        var result = _helper.ParseEnvFile(path);

        Assert.That(result["KEY"], Is.EqualTo("my value with spaces"));
    }

    [Test]
    public void ParseEnvFile_HandlesSingleQuotedValues()
    {
        var path = CreateEnvFile("KEY='my value with spaces'");
        var result = _helper.ParseEnvFile(path);

        Assert.That(result["KEY"], Is.EqualTo("my value with spaces"));
    }

    [Test]
    public void ParseEnvFile_TrimsWhitespace()
    {
        var path = CreateEnvFile("  KEY  =  value  ");
        var result = _helper.ParseEnvFile(path);

        Assert.That(result["KEY"], Is.EqualTo("value"));
    }

    [Test]
    public void ParseEnvFile_HandlesValuesWithEquals()
    {
        var path = CreateEnvFile("CONNECTION_STRING=Endpoint=https://foo.bar;Key=abc123");
        var result = _helper.ParseEnvFile(path);

        Assert.That(result["CONNECTION_STRING"], Is.EqualTo("Endpoint=https://foo.bar;Key=abc123"));
    }

    [Test]
    public void ParseEnvFile_SkipsLinesWithoutEquals()
    {
        var path = CreateEnvFile("VALID=value\nINVALID_LINE\n=no_key\nALSO_VALID=test");
        var result = _helper.ParseEnvFile(path);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result["VALID"], Is.EqualTo("value"));
            Assert.That(result["ALSO_VALID"], Is.EqualTo("test"));
        });
    }

    [Test]
    public void ParseEnvFile_IsCaseInsensitive()
    {
        var path = CreateEnvFile("MyKey=value");
        var result = _helper.ParseEnvFile(path);

        Assert.That(result["mykey"], Is.EqualTo("value"));
    }
}
