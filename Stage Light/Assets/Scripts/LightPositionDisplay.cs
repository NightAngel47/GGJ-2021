using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPositionDisplay : MonoBehaviour
{
    
    public void SetSelectedLightFixturePosition(int index)
    {
        LightFixtureBehavior selected = PuzzleManager.SelectedFixture;
        selected.UpdatePosition(index);
    }
}
