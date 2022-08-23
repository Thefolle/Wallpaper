using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SplineComponentData : IComponentData
{
    public float S;
    public Entity Start;
    public Entity End;

    public bool isArrived()
    {
        return S >= 1;
    }
}
