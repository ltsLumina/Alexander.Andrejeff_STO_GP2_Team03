using System;
using UnityEngine;

[Serializable]
public sealed class SaveProfile<T> where T : SaveLevelIndex
{
    public string profileName;
    public T saveData;
    
    private SaveProfile() { }
    
    public SaveProfile(string profileName, T saveData)
    {
        this.profileName = profileName;
        this.saveData = saveData;
    }
}



// inherent from this to create diff save stuff
public abstract record SaveProfileData { }


// one example of saving highsocre and uses this for saving 
/*
var playerScoreSave = new SaveHighScoreData{ HighScore = _currentScore };
var saveProfile = new SaveProfile<SaveHighScoreData>("playerHighScore", playerScoreSave);
SaveManager.Save(saveProfile);
 */

public record SaveLevelIndex : SaveProfileData
{
    public int LevelIndex;
}


// one example of saving player stat and uses this for saving and how to use it
/*
var playerSave = new SavePlayer(){ PlayerHealth = playerHealth, PlayerPosition = Vector2.one * 4f, PlayerScore = _currentScore, PlayerStrenght = playerSrenght};
var saveProfile = new SaveProfile<SavePlayer>("playerHighScore", playerSave);
SaveManager.Save(saveProfile);
 */

public record SavePlayer : SaveProfileData
{
    public Vector2 PlayerPosition;
    public int PlayerHealth;
    public int PlayerScore;
    public int PlayerStrenght;
}