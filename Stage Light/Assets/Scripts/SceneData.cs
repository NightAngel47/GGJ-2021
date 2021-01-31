using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "New SceneData", menuName = "SceneData")]
public class SceneData : ScriptableObject
{
    public List<Color> colors;
    public List<Vector3> positions = new List<Vector3>();
    [SerializeField] private List<int> lightIndexes;
    [SerializeField] private List<StagePoint> requestsForScene;

    public List<Color> Colors => colors;
    public List<Vector3> Positions => positions;
    
    public List<int> LightIndexes => lightIndexes;


    public List<StagePoint> RequestsForScene => requestsForScene;
}