using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using UnityTimer;

[CreateAssetMenu]
public class SceneLoader : ScriptableObject
{
    #region Variables

    [Header("Scene Loading")]
    [Scene, SerializeField] private string sceneToLoad;
    [SerializeField] private LoadSceneMode loadMode;

    [Header("Loading Screen")]
    [SerializeField] private bool hasLoadingScreen;
    [ShowIf(nameof(hasLoadingScreen)), SerializeField] private GameObject loadingScreen;
    [ShowIf(nameof(hasLoadingScreen)), SerializeField] private bool dontDestroyOnLoad;
    [ShowIf(nameof(dontDestroyOnLoad)), SerializeField] private float loadingScreenLifeTime;

    [Header("Loading Time")]
    [SerializeField] private bool fixedLoadingTime;
    [ShowIf(nameof(fixedLoadingTime)), SerializeField] private float loadTime;

    [Header("Time Scale")]
    [SerializeField] private bool setTimeScale;
    [ShowIf(nameof(setTimeScale)), SerializeField] private float targetTimeScale;

    [Space]
    [SerializeField] private bool debug;

    #endregion

    #region LoadScene

    [Button]
    public void LoadScene()
    {
        if (debug)
            Debug.Log($"Load {sceneToLoad} scene...");

        LoadScene(sceneToLoad, loadMode);

        if (setTimeScale)
            Time.timeScale = targetTimeScale;
    }

    public static void LoadScene(int buildIndex) => LoadScene(buildIndex, LoadSceneMode.Single);
    public static void LoadScene(string sceneToLoad) => LoadScene(sceneToLoad, LoadSceneMode.Single);
    public static void LoadScene(int buildIndex, LoadSceneMode loadMode) => LoadScene(NameFromIndex(buildIndex), loadMode);

    public static void LoadScene(string sceneToLoad, LoadSceneMode loadMode)
    {
        SceneManager.LoadScene(sceneToLoad, loadMode);
    }

    #endregion

    #region LoadSceneAsync

    [Button]
    public void LoadSceneAsync() => LoadSceneAsync(sceneToLoad, loadMode);
    public void LoadSceneAsync(int buildIndex) => LoadSceneAsync(buildIndex, LoadSceneMode.Single);
    public void LoadSceneAsync(string sceneToLoad) => LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
    public void LoadSceneAsync(int buildIndex, LoadSceneMode loadMode) => LoadSceneAsync(NameFromIndex(buildIndex), loadMode);

    public void LoadSceneAsync(string sceneToLoad, LoadSceneMode loadMode)
    {
        if(debug)
            Debug.Log($"Loading {sceneToLoad} scene...");
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneToLoad, loadMode);

        if (hasLoadingScreen)
        {
            GameObject go = Instantiate(loadingScreen);
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(go);
                Timer.Register(loadingScreenLifeTime, () => Destroy(go), null, false, true);
            }
        }

        if (fixedLoadingTime)
        {
            ao.allowSceneActivation = false;
            Timer.Register(loadTime, () => ao.allowSceneActivation = true, null, false, true);
        }

        ao.completed += _ =>
        {
            if (setTimeScale)
                Time.timeScale = targetTimeScale;
        };
    }

    #endregion

    #region RemoveScene

    [Button]
    public void RemoveScene()
    {
        if (debug)
            Debug.Log($"Remove {sceneToLoad} scene...");

        RemoveScene(sceneToLoad);
    }

    public void RemoveScene(int buildIndex)
    {
        RemoveScene(NameFromIndex(buildIndex));
    }

    public void RemoveScene(string sceneToRemove)
    {
        SceneManager.UnloadSceneAsync(sceneToRemove);
    }

    #endregion

    #region Reloading

    [Button]
    public void ReloadScene()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        LoadScene(buildIndex);
    }

    [Button]
    public void ReloadSceneAsync()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        LoadSceneAsync(buildIndex);
    }

    #endregion

    #region LoadNextScene

    public static bool LoadNextScene()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (buildIndex < SceneManager.sceneCountInBuildSettings)
        {
            LoadScene(buildIndex);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool LoadNextSceneAsync()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (buildIndex < SceneManager.sceneCountInBuildSettings)
        {
            LoadSceneAsync(buildIndex);
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    private static string NameFromIndex(int BuildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }
}
