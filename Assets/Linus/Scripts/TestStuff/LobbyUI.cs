using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : NetworkBehaviour
{

    [Header("Levels")]
    [SerializeField] private List<LevelEntry> levels = new List<LevelEntry>();

    [System.Serializable]
    public class LevelEntry
    {
        //Check the index to get correct level...
        public int indexint; //To make it easier to read for Gamedesigners(?)...
        public string displayName;
        public string sceneName;
    }

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void OnClickRoleA()
    {
        LobbyState.Instance.SelectRole(1);
    }

    public void OnClickRoleB()
    {
        LobbyState.Instance.SelectRole(2);
    }

    public void LoadLevelWithIndex(int index)
    {
        
        if (!LobbyState.Instance.ReadyToStart())
        {
            Debug.Log("Players do not have different Roles");
            return;
        }

        if (index < 0 || index > levels.Count)
        {
            Debug.Log("Too high or low index");
            return;
        }

        string sceneName = levels[index].sceneName;

        if (LobbyState.Instance.ReadyToStart() && !string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"Loading level: {levels[index].displayName}");
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
        StartCoroutine(LoadSceneNextFrame("Main-Menu"));
    }

    private IEnumerator LoadSceneNextFrame(string sceneName)
    {
        yield return new WaitForEndOfFrame(); //Just waiting a frame for safety...
        SceneManager.LoadScene(sceneName);
    }

    //public void Level1()
    //{
    //    if (LobbyState.Instance.ReadyToStart())
    //    {
    //        Debug.Log("Start level1");
    //        //NetworkManager.Singleton.SceneManager.LoadScene
    //        //Load the Gameplay-Scene.
    //        //remove the canvas start a timer, whatever fits best.
    //        NetworkManager.Singleton.SceneManager.LoadScene("Main", UnityEngine.SceneManagement.LoadSceneMode.Single); //makes sure all clients enter the new scene.
    //    }
    //    else
    //    {
    //        Debug.Log("Game is not ready to start, check roles.");
    //    }
    //}

    //public void Level2()
    //{
    //    if (LobbyState.Instance.ReadyToStart())
    //    {
    //        Debug.Log("Start level2");
    //        //NetworkManager.Singleton.SceneManager.LoadScene
    //        //Load the Gameplay-Scene.
    //        //remove the canvas start a timer, whatever fits best.
    //        NetworkManager.Singleton.SceneManager.LoadScene("TestScene2", UnityEngine.SceneManagement.LoadSceneMode.Single); //makes sure all clients enter the new scene.
    //    }
    //    else
    //    {
    //        Debug.Log("Game is not ready to start, check roles.");
    //    }
    //}
    //public void Level3()
    //{
    //    if (LobbyState.Instance.ReadyToStart())
    //    {
    //        Debug.Log("Start level3");
    //        //NetworkManager.Singleton.SceneManager.LoadScene
    //        //Load the Gameplay-Scene.
    //        //remove the canvas start a timer, whatever fits best.
    //        NetworkManager.Singleton.SceneManager.LoadScene("TestScene2", UnityEngine.SceneManagement.LoadSceneMode.Single); //makes sure all clients enter the new scene.
    //    }
    //    else
    //    {
    //        Debug.Log("Game is not ready to start, check roles.");
    //    }
    //}
    //public void Level4()
    //{
    //    if (LobbyState.Instance.ReadyToStart())
    //    {
    //        Debug.Log("start level4");
    //        //NetworkManager.Singleton.SceneManager.LoadScene
    //        //Load the Gameplay-Scene.
    //        //remove the canvas start a timer, whatever fits best.
    //        NetworkManager.Singleton.SceneManager.LoadScene("TestScene2", UnityEngine.SceneManagement.LoadSceneMode.Single); //makes sure all clients enter the new scene.
    //    }
    //    else
    //    {
    //        Debug.Log("Game is not ready to start, check roles.");
    //    }
    //}

}
