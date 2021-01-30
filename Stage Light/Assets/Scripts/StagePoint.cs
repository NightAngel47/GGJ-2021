using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public struct StagePoint
{
    public int position;
    public Shapes shape;
    public Color color;
    
    public enum Shapes
    {
        Square,
    }
}