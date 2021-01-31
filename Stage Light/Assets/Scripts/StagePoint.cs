using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public struct StagePoint
{
    public int positionIndex;
    public Shapes shape;
    public Color color;

    public static bool operator == (StagePoint a, StagePoint b) => a.positionIndex == b.positionIndex && 
                                                                   a.shape == b.shape && 
                                                                   a.color == b.color;
    public static bool operator != (StagePoint a, StagePoint b) => a.positionIndex != b.positionIndex || 
                                                                   a.shape != b.shape ||
                                                                   a.color != b.color;

    public override bool Equals(object obj)
    {
        return obj != null && this == (StagePoint)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public enum Shapes
    {
        Square,
    }
}