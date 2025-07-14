using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Pikol93.CJ14;

public partial class GameSettings : Node
{
    private const string SettingsPath = "user://settings.json";
    private static JsonSerializerOptions ReadOptions { get; } = new JsonSerializerOptions { WriteIndented = true };
    private static JsonSerializerOptions WriteOptions { get; } = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    
    public static bool EnableGore { get; set; } = true;
    public static int SoundVolume { get; set; } = 7;
    public static int MusicVolume { get; set; } = 7;

    public override void _Ready()
    {
        Load();
    }

    public static void Save()
    {
        var data = new Dictionary<string, object>
        {
            { "gore", EnableGore },
            { "soundVolume", SoundVolume },
            { "musicVolume", MusicVolume },
        };
        var json = JsonSerializer.Serialize(data, ReadOptions);
        using var file = FileAccess.Open(SettingsPath, FileAccess.ModeFlags.Write);
        if (file == null)
        {
            GD.PrintErr("Failed to open file for writing: ", SettingsPath);
            return;
        }

        file.StoreString(json);
        GD.Print($"Saved data at {SettingsPath}: {json}");

        ProcessSettings();
    }

    public static void Load()
    {
        var dict = LoadDict();

        try
        {
            var value = dict["gore"].GetBoolean();
            GD.Print($"Loading gore: {value}");
            EnableGore = value;
        }
        catch (Exception) { }

        try
        {
            var value = dict["soundVolume"].GetInt32();
            GD.Print($"Loading sound volume: {value}");
            SoundVolume = value;
        }
        catch (Exception) { }

        try
        {
            var value = dict["musicVolume"].GetInt32();
            GD.Print($"Loading music volume: {value}");
            MusicVolume = value;
        }
        catch (Exception) { }

        ProcessSettings();
    }

    public static void ProcessSettings()
    {
        var soundBusIndex = AudioServer.GetBusIndex("Sound");
        AudioServer.SetBusVolumeDb(soundBusIndex, LinearToDb(SoundVolume));

        var musicBusIndex = AudioServer.GetBusIndex("Music");
        AudioServer.SetBusVolumeDb(musicBusIndex, LinearToDb(MusicVolume));
    }

    public static Dictionary<string, JsonElement> LoadDict()
    {
        if (!FileAccess.FileExists(SettingsPath))
        {
            GD.PrintErr("File not found: ", SettingsPath);
            return [];
        }

        using var file = FileAccess.Open(SettingsPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr("Failed to open file for reading: ", SettingsPath);
            return [];
        }

        string jsonString = file.GetAsText();

        try
        {
            var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString, WriteOptions);
            GD.Print($"Data loaded successfully: {jsonString}");
            return result ?? [];
        }
        catch (Exception e)
        {
            GD.PrintErr("Failed to parse JSON: ", e.Message);
            return [];
        }
    }

    public static float LinearToDb(int value)
    {
        // Normalize from 0–10 to 0.0–1.0
        var linear = Math.Clamp(value / 9.0f, 0.0001f, 1.0f);

        // Convert to decibels
        return 20.0f * (float)Math.Log10(linear);
    }
}
