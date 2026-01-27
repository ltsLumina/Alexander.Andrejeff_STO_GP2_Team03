using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveManager
{
    private static readonly string saveFolder = Application.persistentDataPath + "/GameData";

    
    // Delete
    /// <summary>
    /// Delete given saveProfile
    /// </summary>
    /// <param name="profileName">String name of the profileName</param>
    public static void Delete(string profileName)
    {
        if (!File.Exists($"{saveFolder}/{profileName}"))
        {
            Debug.LogWarning($"Save Profile {profileName} does not exist");
            return;
        }
        Debug.Log($"Successfully deleted {profileName}");
        File.Delete($"{saveFolder}/{profileName}");
    }
    
    // Load
    /// <summary>
    /// Loads a given save profile
    /// </summary>
    /// <param name="profileName">Name of the save profile to load</param>
    /// <typeparam name="T">Save Profile data type</typeparam>
    /// <returns>Json DeserializeObject content of the file to load; Null if no saveProfile found</returns>
    public static SaveProfile<T> Load<T>(string profileName) where T : SaveLevelIndex
    {
        if (!File.Exists($"{saveFolder}/{profileName}"))
        {
            Debug.LogWarning("No save profile found");
            return null;
        }
        
        var fileContents = File.ReadAllText($"{saveFolder}/{profileName}");
        
        Debug.Log($"Loading Profile {profileName}");
        
        return JsonConvert.DeserializeObject<SaveProfile<T>>(fileContents);
    }

    // Save
    
    /// <summary>
    /// Saves given saveProfile to file and write 
    /// </summary>
    /// <param name="save">Save content data with it </param>
    /// <typeparam name="T">Uses SaveProfileData profile as a type</typeparam>
    public static void Save<T>(SaveProfile<T> save) where T : SaveLevelIndex
    {

        var jsonString = JsonConvert.SerializeObject(save, Formatting.Indented, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        
        // if add encrypt method
        
        
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }
        File.WriteAllText($"{saveFolder}/{save.profileName}", jsonString);
        Debug.Log($"Saved to {save.profileName}");
    }

    public static bool TryGetSave<T>(string profileName, out SaveProfile<T> saveData) where T : SaveLevelIndex
    {
        if (!File.Exists($"{saveFolder}/{profileName}"))
        {
            Debug.LogWarning("No save profile found");
            saveData = null;
            return false;
        }
        
        saveData = Load<T>(profileName);
        return saveData != null;
        
    }
    
}