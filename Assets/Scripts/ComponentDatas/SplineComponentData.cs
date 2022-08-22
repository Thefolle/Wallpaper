using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SplineComponentData : IComponentData
{
    public float3 Start;
    public float3 End;
    public float S;

    public bool isArrived()
    {
        return S >= 1;
    }
}
