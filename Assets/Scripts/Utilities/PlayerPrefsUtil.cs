using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsUtil
{
    private const string PlayerPrefsKey = "MyGameData";

    // Method to save a dictionary to PlayerPrefs as a JSON string
    public static void SaveData(Dictionary<string, string> data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        PlayerPrefs.SetString(PlayerPrefsKey, jsonData);
        PlayerPrefs.Save();
    }

    // Method to load data from PlayerPrefs
    public static Dictionary<string, string> LoadData()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string jsonData = PlayerPrefs.GetString(PlayerPrefsKey);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
        }
        return new Dictionary<string, string>();
    }

    // Method to add or update a key-value pair in the PlayerPrefs data
    public static void AddOrUpdateData(string key, string value)
    {
        var data = LoadData();
        if (data.ContainsKey(key))
        {
            data[key] = value;
        }
        else
        {
            data.Add(key, value);
        }
        SaveData(data);
    }

    // Method to get a value by key from the PlayerPrefs data
    public static string GetData(string key)
    {
        var data = LoadData();
        if (data.ContainsKey(key))
        {
            return data[key];
        }
        return null;
    }
}
