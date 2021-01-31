using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProgrammedLightFixture
{
    public LightFixtureBehavior fixture;
    public StagePoint point;

    public ProgrammedLightFixture(LightFixtureBehavior selectedFixture, StagePoint currentPoint)
    {
        fixture = selectedFixture;
        point = currentPoint;
    }
}