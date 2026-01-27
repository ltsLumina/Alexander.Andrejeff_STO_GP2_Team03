using System.Collections;
using UnityEngine;

public class OpenLink : MonoBehaviour
{

    [SerializeField] bool isOpenWebsiteDelayed= false;
    public void OpenWebsite()
    {
        if (isOpenWebsiteDelayed)
        {
            StartCoroutine(DelayedURLOpener());
            return;
        }


        string url = "https://dailykitten.com/";
        Application.OpenURL(url);

        string url2 = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
        Application.OpenURL(url2);

        string url3 = "https://www.boredbutton.com/";
        Application.OpenURL(url3);
    }

    IEnumerator DelayedURLOpener()
    {
        string url = "https://dailykitten.com/";
        Application.OpenURL(url);
        yield return new WaitForSeconds(1);
        string url3 = "https://www.boredbutton.com/";
        Application.OpenURL(url3);
        yield return new WaitForSeconds(2);
        string url2 = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
        Application.OpenURL(url2);
    }
}
