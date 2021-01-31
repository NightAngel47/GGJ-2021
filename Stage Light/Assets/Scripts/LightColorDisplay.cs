using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightColorDisplay : MonoBehaviour
{
    [SerializeField] private Image selectedColorImage;
    [SerializeField] private GameObject sliderBackgroundColorPrefab;
    [SerializeField] private Transform sliderBackground;
    private Slider slider = null;

    private int previousValue = -1;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
    }
    
    private void OnEnable()
    {
        PuzzleManager.SelectedFixtureChanged.AddListener(UpdateSliderData);
    }

    private void LateUpdate()
    {
        if (previousValue != (int)slider.value)
            UpdateDisplay();
    }
    
    private void OnDisable()
    {
        PuzzleManager.SelectedFixtureChanged.RemoveListener(UpdateSliderData);
    }

    private void UpdateSliderData(int newIndex)
    {
        // clear color UI indicators for slider background
        for (int i = 0; i < sliderBackground.childCount; ++i)
        {
            Destroy(sliderBackground.GetChild(i).gameObject);
        }
        
        LightFixtureBehavior selectedLight = PuzzleManager.SelectedFixture;
        
        slider.maxValue = selectedLight.PossibleColors.Count;
        // Spawn color UI indicators for slider background
        foreach (var color in selectedLight.PossibleColors)
        {
            Image newColor = Instantiate(sliderBackgroundColorPrefab, sliderBackground).GetComponent<Image>();
            newColor.color = color;

            if (color == selectedLight.CurrentPoint.color)
            {
                slider.value = selectedLight.PossibleColors.IndexOf(color) + 1;
                selectedColorImage.color = color;
            }
        }
    }
    
    public void UpdateDisplay()
    {
        previousValue = (int)slider.value;
        LightFixtureBehavior selectedLight = PuzzleManager.SelectedFixture;
        selectedColorImage.color = selectedLight.PossibleColors[previousValue - 1];
        selectedLight.UpdateColor(previousValue - 1);
    }
}
