

using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
partial struct GoInGameClientSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach((RefRO<NetworkId> networkid,Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>().WithEntityAccess())
        {
            ecb.AddComponent<NetworkStreamInGame>(entity);
            Entity rpcEntity = ecb.CreateEntity();
            ecb.AddComponent(rpcEntity,new GoInGameRequestRPC());
            ecb.AddComponent(rpcEntity,new SendRpcCommandRequest());
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

public struct GoInGameRequestRPC : IRpcCommand
{
}