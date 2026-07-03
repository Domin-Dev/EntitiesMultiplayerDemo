

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics.Authoring;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct GoInGameServerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach((RefRO<ReceiveRpcCommandRequest> request,Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<GoInGameRequestRPC>().WithEntityAccess())
        {
            ecb.AddComponent<NetworkStreamInGame>(request.ValueRO.SourceConnection);
            Entity player = ecb.Instantiate(entitiesReferences.playerEntity);
            ecb.SetComponent<LocalTransform>(player,LocalTransform.FromPosition(new float3(0,3,0)));
            var networkID = SystemAPI.GetComponent<NetworkId>(request.ValueRO.SourceConnection);
            ecb.AddComponent(player,new GhostOwner(){ NetworkId = networkID.Value});
            ecb.AppendToBuffer(request.ValueRO.SourceConnection,new LinkedEntityGroup(){ Value = player});
            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
