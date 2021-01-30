using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "New SceneData", menuName = "SceneData")]
public class SceneData : ScriptableObject
{
    [SerializeField] private List<Vector3> positions;
    public List<Vector3> Positions => positions;

    // Queue of actors
}