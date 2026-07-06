using System.Net.NetworkInformation;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class PlayerInputSystem : SystemBase
{
    private NewInput input;

    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<NetworkStreamInGame>();
        RequireForUpdate<PlayerInput>();

        input = new NewInput();
        input.Enable();
    } 
    
    [BurstCompile]
    protected override void OnDestroy()
    {
        input.Disable();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        float2 move = input.Player.Move.ReadValue<Vector2>();
        foreach(RefRW<PlayerInput> input in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>())
        {
            input.ValueRW.movementDirection = move;
        }
    }
}
