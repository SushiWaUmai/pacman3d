using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ScriptableObjectArchitecture;

public class UIManager : MonoBehaviour
{
    [SerializeField] private IntVariable totalScore;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI totalScoreDisplay;

    private void Start()
    {
        totalScore.AddListener(() => totalScoreDisplay.text = totalScore.Value.ToString());
    }
}