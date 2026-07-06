


using System.Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.NetCode.HostMigration;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct OutOfBoundsSystem : ISystem
{
    private float minY;

    public void OnCreate(ref SystemState state)
    {
        minY = -5f;
    }

    public void OnUpdate(ref SystemState state)
    {
        var time = SystemAPI.GetSingleton<NetworkTime>();

        foreach (var (transform,velocity,owner) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<PhysicsVelocity>,RefRO<GhostOwner>>().WithAll<Player>())
        {
            if (transform.ValueRO.Position.y < minY)
            {      
                transform.ValueRW = LocalTransform.FromPosition(new float3(UnityEngine.Random.Range(-4,4),0.8f,UnityEngine.Random.Range(-4,4)));
                velocity.ValueRW = PhysicsVelocity.Zero;

                foreach (var (player,points) in SystemAPI.Query<RefRO<GhostOwner>,DynamicBuffer<Points>>().WithAll<Player>())
                {   
                    if(player.ValueRO.NetworkId != owner.ValueRO.NetworkId)
                    {
                        points.Add(new Points(){ tick = time.ServerTick});
                        break;
                    }
                }
            }
        }
    }
}