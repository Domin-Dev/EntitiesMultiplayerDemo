

using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics.Authoring;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct GoInGameServerSystem : ISystem
{

    private EntityQuery playersQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
        state.RequireForUpdate<GoInGameRequestRPC>();
        playersQuery = SystemAPI.QueryBuilder().WithAll<Player,GhostOwner>().Build();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        int players = playersQuery.CalculateEntityCount();

        foreach((RefRO<ReceiveRpcCommandRequest> request,Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().WithAll<GoInGameRequestRPC>().WithEntityAccess())
        {
            if(players < 2)
            {
                ecb.AddComponent<NetworkStreamInGame>(request.ValueRO.SourceConnection);
                Entity player = ecb.Instantiate(entitiesReferences.playerEntity);
                ecb.SetComponent<LocalTransform>(player,LocalTransform.FromPosition(new float3(UnityEngine.Random.Range(-4,4),0.6f,UnityEngine.Random.Range(-4,4))));
                var networkID = SystemAPI.GetComponent<NetworkId>(request.ValueRO.SourceConnection);
                ecb.AddComponent(player,new GhostOwner(){ NetworkId = networkID.Value});
                ecb.SetComponent(player,new PlayerTeam()
                {
                    team = players == 0 ? TeamTag.Blue : TeamTag.Red 
                });
                ecb.AppendToBuffer(request.ValueRO.SourceConnection,new LinkedEntityGroup(){ Value = player});
                players++;
            }

            ecb.DestroyEntity(entity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
