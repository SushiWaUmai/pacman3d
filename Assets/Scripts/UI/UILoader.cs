using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class UILoader : MonoBehaviour
{
    [Scene, SerializeField] private string UIScene;

    private void Start()
    {
        SceneManager.LoadScene(UIScene, LoadSceneMode.Additive);
    }
}
