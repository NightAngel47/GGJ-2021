using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LightPositionDisplay : MonoBehaviour
{
    private readonly List<Button> numpadButtons = new List<Button>();

    private void Awake()
    {
        for(int i = 0; i < transform.GetChild(0).childCount; ++i)
        {
            numpadButtons.Add(transform.GetChild(0).GetChild(i).GetComponent<Button>());
        }
    }

    private void OnEnable()
    {
        PuzzleManager.SelectedFixtureChanged.AddListener(UpdateControlsForSelectedLight);
    }
    private void OnDisable()
    {
        PuzzleManager.SelectedFixtureChanged.RemoveListener(UpdateControlsForSelectedLight);
    }

    private void UpdateControlsForSelectedLight(int newIndex)
    {
        // reset buttons to not interactable
        foreach (var button in numpadButtons)
        {
            button.interactable = false;
        }
        
        // disable buttons that don't match this light's stuff
        LightFixtureBehavior selected = PuzzleManager.SelectedFixture;
        for(int i = 0; i < numpadButtons.Count; ++i)
        {
            if (selected.PossiblePositions.All(pos => i != pos)) continue;
            numpadButtons[i].interactable = true;
            if (i == selected.CurrentPoint.positionIndex)
            {
                numpadButtons[i].Select();
            }
        }
    }
    
    public void SetSelectedLightFixturePosition(int gridPositionIndex)
    {
        PuzzleManager.SelectedFixture.UpdatePosition(gridPositionIndex);
    }
}
