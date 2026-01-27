using UnityEngine;
using UnityEngine.UI;


public class LoadLevelSelection : MonoBehaviour
{

    public int levelIndex;
    private string saveProfileString = "SaveLevelIndex";
    
    private void Awake()
    {
        levelIndex = GetLevelIndex();

        Debug.Log(levelIndex);
    }

    private int GetLevelIndex()
    {
        var sd = SaveManager.Load<SaveLevelIndex>(saveProfileString);
        Debug.Log(sd);
        if (sd == null)
        {
            var newSaveFile = new SaveLevelIndex() { LevelIndex = 0 };
            var saveProfile = new SaveProfile<SaveLevelIndex>("LevelIndex", newSaveFile);
            SaveManager.Save(saveProfile);
            sd = SaveManager.Load<SaveLevelIndex>(saveProfileString);
        }

        return sd.saveData.LevelIndex;

    }
    
    
}
