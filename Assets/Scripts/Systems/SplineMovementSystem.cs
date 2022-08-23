using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;

public partial class SplineMovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        Enabled = false;
    }
    protected override void OnUpdate()
    {
        var ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
        var deltaTime = Time.DeltaTime;

        var getTranslationComponentDataFromEntity = GetComponentDataFromEntity<Translation>();
        var getNodeComponentDataFromEntity = GetComponentDataFromEntity<NodeComponentData>();

        Dependency = Entities.ForEach((ref SplineComponentData splineComponentData, ref PieceComponentData pieceComponentDatain, in Entity entity) => {
            splineComponentData.S += 0.05f * deltaTime;

            if (splineComponentData.isArrived())
            {
                splineComponentData.S = 1;
                ecb.SetComponent(entity, getTranslationComponentDataFromEntity[splineComponentData.End]);
                ecb.RemoveComponent<SplineComponentData>(entity);

                var nodeToFreeComponentData = getNodeComponentDataFromEntity[splineComponentData.Start];
                nodeToFreeComponentData.Node = Entity.Null;
                ecb.SetComponent(splineComponentData.Start, nodeToFreeComponentData);

                pieceComponentDatain.Node = splineComponentData.End;
            } else
            {
                ecb.SetComponent(entity, new Translation
                {
                    Value = math.lerp(getTranslationComponentDataFromEntity[splineComponentData.Start].Value, getTranslationComponentDataFromEntity[splineComponentData.End].Value, splineComponentData.S)
                });
            }
            
        }).Schedule(Dependency);

        World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().AddJobHandleForProducer(Dependency);

    }
}
