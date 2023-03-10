using System.Text.RegularExpressions;

List<string> includedHeaders = new();
string input = Console.ReadLine();
string projectName = Console.ReadLine();
if (string.IsNullOrWhiteSpace(projectName))
{
    projectName = "src";
}
Console.Clear();
string rootPath = Path.Combine(input, projectName);
Process(new(rootPath), "*.cpp", ProcessFile);
Process(new(rootPath), "*.hpp", (file) =>
{
    if (!includedHeaders.Contains(file.FullName.ToUpperInvariant()))
    {
        Console.WriteLine(file.FullName);
    }
});

void ProcessFile(FileInfo file)
{
    string fileData = File.ReadAllText(file.FullName);
    foreach ((FileInfo header, string upperHeaderPath) in from FileInfo header in
                                                              from Match match in Regex1().Matches(fileData).Cast<Match>()
                                                              let headerPath = match.Groups[1].Value
                                                              let headerFullPath = headerPath.StartsWith(projectName)
                                                                  ? Path.Combine(input, headerPath)
                                                                  : Path.Combine(file.DirectoryName, headerPath)
                                                              let header = new FileInfo(headerFullPath)
                                                              select header
                                                          let upperHeaderPath = header.FullName.ToUpperInvariant()
                                                          select (header, upperHeaderPath))
    {
        if (!header.Exists || includedHeaders.Contains(upperHeaderPath))
        {
            continue;
        }
        includedHeaders.Add(upperHeaderPath);
        ProcessFile(header);
    }
}

static void Process(DirectoryInfo dirInfo, string pattern, Action<FileInfo> action)
{
    foreach (FileInfo file in dirInfo.GetFiles(pattern))
    {
        action(file);
    }
    foreach (DirectoryInfo dir in dirInfo.GetDirectories())
    {
        Process(dir, pattern, action);
    }
}

internal partial class Program
{
    [GeneratedRegex("#include [\"<](.+?)[\">]")]
    private static partial Regex Regex1();
}
