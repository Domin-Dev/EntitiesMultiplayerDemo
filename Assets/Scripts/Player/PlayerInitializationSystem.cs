using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[RequireMatchingQueriesForUpdate]
public partial struct PlayerInitializationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {    
        foreach (var (playerTeam,color, tag)
            in SystemAPI.Query<RefRO<PlayerTeam>,RefRW<CubeColor>,EnabledRefRW<InitializePlayerTag>>())
        {
            switch(playerTeam.ValueRO.team)
            {
                case TeamTag.Blue:
                    color.ValueRW.color = UnityEngine.Color.blue;
                    break;
                case TeamTag.Red:
                    color.ValueRW.color = UnityEngine.Color.red;
                    break;
            }
            tag.ValueRW = false;
        }
    }
}

