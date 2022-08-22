using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class MovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        Enabled = false;
    }

    protected override void OnUpdate()
    {
        var ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
        var em = World.EntityManager;

        var getNodeComponentDataFromEntity = GetComponentDataFromEntity<NodeComponentData>();
        var getTranslationComponentDataFromEntity = GetComponentDataFromEntity<Translation>();

        Dependency = Entities.ForEach((ref PieceComponentData pieceComponentData, in Entity entity, in PieceTag pieceTag) => {
            var currentNodeComponentData = getNodeComponentDataFromEntity[pieceComponentData.Node];
            if (currentNodeComponentData.Up != Entity.Null)
            {
                var movedNodeComponentData = getNodeComponentDataFromEntity[currentNodeComponentData.Up];
                if (movedNodeComponentData.Node == Entity.Null)
                {
                    /*
                    * go upwards since it is free
                    */
                    ecb.AddComponent(entity, new SplineComponentData
                    {
                            Start = getTranslationComponentDataFromEntity[pieceComponentData.Node].Value,
                            End = getTranslationComponentDataFromEntity[currentNodeComponentData.Up].Value
                    });
                    movedNodeComponentData.Node = entity;
                    ecb.SetComponent(currentNodeComponentData.Up, movedNodeComponentData);
                }
            }
        }).Schedule(Dependency);

        World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().AddJobHandleForProducer(Dependency);
    }
}
