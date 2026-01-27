using System.Collections;
using TMPro;
using UnityEngine;

public class IngameMenu : MonoBehaviour
{
    public static IngameMenu Instance;

    [SerializeField] GameObject goGameIsPaused;
    [SerializeField] TextMeshProUGUI textMeshPro;

    [SerializeField] GameObject goPAUSEBLOCKER;

    [Header("Buttons")]
    [SerializeField] GameObject goPauseButton;
    [SerializeField] GameObject goResumeButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        goGameIsPaused.SetActive(false);
        goPAUSEBLOCKER.SetActive(false);
    }

    // Called from networking layer
    public void StartPauseFlow()
    {
        Pause();
        //StartCoroutine(PauseCoroutine());
    }

    public void StartResumeFlow()
    {
        Resume();
        //StartCoroutine(ResumeCoroutine());
    }

    IEnumerator PauseCoroutine()
    {
        goPauseButton.SetActive(false);
        textMeshPro.text = "Pausing in 3...";
        Debug.Log("TMP set to 3: " + textMeshPro.text);
        yield return new WaitForSecondsRealtime(1);
        textMeshPro.text = "Pausing in 2...";
        yield return new WaitForSecondsRealtime(1);
        textMeshPro.text = "Pausing in 1...";
        yield return new WaitForSecondsRealtime(1);

        textMeshPro.text = "Pausing Game...";
        yield return new WaitForSecondsRealtime(1);
        textMeshPro.text = "";

        Pause();
    }

    IEnumerator ResumeCoroutine()
    {
        goResumeButton.SetActive(false);
        textMeshPro.text = "Resuming in 3...";
        yield return new WaitForSecondsRealtime(1);
        textMeshPro.text = "Resuming in 2...";
        yield return new WaitForSecondsRealtime(1);
        textMeshPro.text = "Resuming in 1...";
        yield return new WaitForSecondsRealtime(1);

        textMeshPro.text = "Resuming Game...";
        yield return new WaitForSecondsRealtime(1);
        textMeshPro.text = "";

        Resume();
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        goResumeButton.SetActive(true);

        goGameIsPaused.SetActive(true);
        goPAUSEBLOCKER.SetActive(true);
    }

    private void Resume()
    {
        Time.timeScale = 1f;
        goPauseButton.SetActive(true);

        goGameIsPaused.SetActive(false);
        goPAUSEBLOCKER.SetActive(false);
    }
}
