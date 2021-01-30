using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "New SceneData", menuName = "SceneData")]
public class SceneData : ScriptableObject
{
    [SerializeField] private List<int> lightIndexes;
    [SerializeField] private List<Vector3> positions;
    [SerializeField] private List<StagePoint> requestsForScene;

    public List<int> LightIndexes => lightIndexes;
    public List<Vector3> Positions => positions;
    public List<StagePoint> RequestsForScene => requestsForScene;
}