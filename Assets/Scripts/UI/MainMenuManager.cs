using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
