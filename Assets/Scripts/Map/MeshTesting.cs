using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MeshTesting : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;

    [Button]
    private void Test()
    {
        bool[,] heightMap = new bool[,]
        {
            { true, false, true, false, true, false, true },
            { false, true, false, true, false, true, false },
            { true, false, true, false, true, false, true },
            { false, true, false, true, false, true, false },
            { true, false, true, false, true, false, true },
            { false, true, false, true, false, true, false },
            { true, false, true, false, true, false, true },
        };
        meshFilter.mesh = MeshGenerator.MeshFromHeightMap(heightMap, 2);
    }
}