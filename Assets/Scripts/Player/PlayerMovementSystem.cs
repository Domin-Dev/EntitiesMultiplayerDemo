
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct PlayerMovementSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {    
        foreach (var (playerInput, player, velocity, entity)
            in SystemAPI.Query<RefRO<PlayerInput>, RefRO<Player>, RefRW<PhysicsVelocity>>().WithAll<Simulate>().WithEntityAccess())
        {
            float2 dir = math.normalizesafe(playerInput.ValueRO.movementDirection);
            velocity.ValueRW.Linear = new float3(0,velocity.ValueRO.Linear.y,0) +  new float3(dir.x,0,dir.y) * player.ValueRO.speed;         
        }
    }
}

