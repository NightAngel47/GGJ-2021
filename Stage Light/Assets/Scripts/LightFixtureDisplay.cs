using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LightFixtureDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text textDisplay = null;
    private Slider slider = null;

    private int previousValue = -1;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();

        UpdateDisplay();
    }

    private void LateUpdate()
    {
        if (previousValue != (int)slider.value)
            UpdateDisplay();
    }

    public void ShowOrHideContent(bool show)
    {
        for (int index = 0; index < transform.childCount; index++)
        {
            transform.GetChild(index).gameObject.SetActive(show);
        }
    }

    public void UpdateDisplay()
    {
        previousValue = (int)slider.value;
        textDisplay.text = (previousValue + 1).ToString();
        PuzzleManager.Instance.NewSelectedFixture(previousValue);
    }

    public void UpdateSliderMax(int newMax)
    {
        slider.maxValue = newMax;
    }
}