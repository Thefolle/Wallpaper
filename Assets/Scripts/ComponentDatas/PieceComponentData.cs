using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PieceComponentData : IComponentData
{
    public int Row;
    public int Column;
    public Entity Node;
}
