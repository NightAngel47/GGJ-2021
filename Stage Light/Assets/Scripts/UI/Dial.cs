using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dial : Selectable
{
    private void LateUpdate()
    {
        if (IsPressed())
        {
            Debug.Log(1);
        }
    }
}