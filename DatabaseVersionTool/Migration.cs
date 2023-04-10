namespace DatabaseVersionTool;

internal class Migration
{
    public string Name { get; }
    public string FilePath { get; }
    public int Version { get; }

    public Migration(FileInfo fileInfo)
    {
        var version = fileInfo.Name.Split('_').First();
        Version = int.Parse(version);
        Name = fileInfo.Name;
        FilePath= fileInfo.FullName;
    }

    internal string GetContent()
    {
        return File.ReadAllText(FilePath);
    }
}