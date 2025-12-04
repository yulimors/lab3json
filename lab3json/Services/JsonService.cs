using System.Text.Json;
using lab3json.Models;
using Microsoft.Maui.Storage;

namespace lab3json.Services;

public static class JsonService
{
    private static string _filePath = Path.Combine(FileSystem.AppDataDirectory, "dormitory.json");

    public static async Task<string?> PickFileAsync()
    {
        return await PickJsonFileAsync();
    }

    private static async Task<string?> PickJsonFileAsync()
    {
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.MacCatalyst, new[] { "public.json" } },
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть JSON файл",
                FileTypes = customFileType
            });

            return result?.FullPath;
        }
        catch
        {
            return null;
        }
    }

    public static async Task<Dormitory> LoadDormitoryAsync(string? path = null)
    {
        var file = path ?? _filePath;
        if (!File.Exists(file))
            return new Dormitory();

        var text = await File.ReadAllTextAsync(file);
        return JsonSerializer.Deserialize<Dormitory>(text) ?? new Dormitory();
    }

    public static async Task SaveDormitoryAsync(Dormitory dormitory, string path)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
       
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(
                System.Text.Unicode.UnicodeRanges.BasicLatin,
                System.Text.Unicode.UnicodeRanges.Cyrillic)
        };

        var json = JsonSerializer.Serialize(dormitory, options);
        await File.WriteAllTextAsync(path, json);
    }
}