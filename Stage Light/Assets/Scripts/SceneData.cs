using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "New SceneData", menuName = "SceneData")]
public class SceneData : ScriptableObject
{
    [SerializeField] private List<int> lightIndexes;
    [SerializeField] private List<StagePoint> requestsForScene;

    public List<int> LightIndexes => lightIndexes;
    public List<Vector3> Positions => new List<Vector3>()
    {
        new Vector3(-4.5f, 3f, 0f), new Vector3(0, 3f, 0f), new Vector3(4.5f, 3f, 0f),
        new Vector3(-4.5f, 0.63f, 0f), new Vector3(0, 0.63f, 0f), new Vector3(4.5f, 0.63f, 0f),
        new Vector3(-4.5f, -1.6f, 0f), new Vector3(0, -1.6f, 0f), new Vector3(4.5f, -1.6f, 0f),
    };
    public List<StagePoint> RequestsForScene => requestsForScene;
}