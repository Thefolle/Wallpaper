using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct NodeComponentData : IComponentData
{
    public Entity Node;
    public Entity Up;
    public Entity Right;
    public Entity Bottom;
    public Entity Left;
}
