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
                                                                   a.color.ToHex() == b.color.ToHex();
    public static bool operator != (StagePoint a, StagePoint b) => a.positionIndex != b.positionIndex || 
                                                                   a.shape != b.shape ||
                                                                   a.color.ToHex() != b.color.ToHex();

    public override bool Equals(object obj)
    {
        return obj != null && this == (StagePoint)obj;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"{positionIndex}, {shape}, {"#" + ColorUtility.ToHtmlStringRGBA(color)}";
    }

    public enum Shapes
    {
        Square,
    }
}

public static class ColorUtil
{
    /// <summary> Gives the hex code of a color. </summary>
    /// <param name="original"> This color. </param>
    /// <returns> The hex code of this color. </returns>
    public static string ToHex(this Color original)
    {
        return "#" + ColorUtility.ToHtmlStringRGBA(original);
    }
}