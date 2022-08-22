using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

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

        Dependency = Entities.ForEach((ref SplineComponentData splineComponentData, ref Translation translation, in Entity entity) => {
            splineComponentData.S += 0.05f * deltaTime;

            if (splineComponentData.isArrived())
            {
                splineComponentData.S = 1;
                translation.Value = splineComponentData.End;
                ecb.RemoveComponent<SplineComponentData>(entity);
            } else
            {
                translation.Value = math.lerp(splineComponentData.Start, splineComponentData.End, splineComponentData.S);
            }
            
        }).Schedule(Dependency);

        World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().AddJobHandleForProducer(Dependency);

        //ecb.Playback(World.EntityManager);
    }
}
