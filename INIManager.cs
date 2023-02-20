using System.Runtime.InteropServices;
using System.Text;

namespace kursovaya;

public class INIManager
{
    private const int SIZE = 1024;

    public INIManager(string aPath)
    {
        Path = aPath;
    }

    public string Path { get; set; }

    public string GetPrivateString(string aSection, string aKey)
    {
        var buffer = new StringBuilder(SIZE);

        GetPrivateString(aSection, aKey, null, buffer, SIZE, Path);

        return buffer.ToString();
    }

    public void WritePrivateString(string aSection, string aKey, string aValue)
    {
        WritePrivateString(aSection, aKey, aValue, Path);
    }

    [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
    private static extern int GetPrivateString(string section, string key, string def, StringBuilder buffer, int size,
        string path);

    [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
    private static extern int WritePrivateString(string section, string key, string str, string path);
}