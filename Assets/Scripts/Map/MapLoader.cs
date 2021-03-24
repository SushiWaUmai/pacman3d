using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class MapLoader : MonoBehaviour
{
    [SerializeField] private Map mapToLoad;

    private void Awake()
    {
        mapToLoad.LoadMap();
    }
}