using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoader : MonoBehaviour
{
    [SerializeField] private SceneLoader UIScene;

    private void Start()
    {
        UIScene.LoadScene();
    }
}
