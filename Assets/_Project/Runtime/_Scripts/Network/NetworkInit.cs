using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkInit : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene("Main-Menu");
    }
}