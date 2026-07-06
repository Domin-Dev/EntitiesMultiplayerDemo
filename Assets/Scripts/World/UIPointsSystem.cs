


using System.Diagnostics;
using System.Drawing;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct UIPointsSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (points,owner) in SystemAPI.Query<DynamicBuffer<Points>,RefRO<PlayerTeam>>().WithChangeFilter<Points>().WithAll<Player>())
        {
            UIManager.Instance.SetPoints(points.Length,owner.ValueRO.team);
        }
    }
}