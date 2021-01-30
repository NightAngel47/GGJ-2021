using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFixtureBehavior : MonoBehaviour
{
    [SerializeField] private StagePoint currentPoint;

    [Space]

    [SerializeField] private List<StagePoint> possiblePoints;

    private void Awake()
    {
        currentPoint = possiblePoints[0];
    }
}