using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Rendering;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] private float speed;
    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Player() 
            { 
                speed = authoring.speed,
            });
            AddComponent(entity, new PlayerTeam()
            {
                team = TeamTag.None
            });
            AddComponent(entity, new PlayerInput());
            AddComponent(entity, new CubeColor());
            AddComponent(entity, new InitializePlayerTag());
            AddBuffer<Points>(entity);
        }
    }
}

public enum TeamTag : byte
{
    Blue,
    Red,
    None,
}

public struct InitializePlayerTag : IComponentData,IEnableableComponent{}

[GhostComponent]
public struct Player : IComponentData
{
    [GhostField] public float speed;
}

[GhostComponent]
public struct PlayerTeam : IComponentData
{
    [GhostField] public TeamTag team;
}

[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct PlayerInput : IInputComponentData
{
    [GhostField] public float2 movementDirection;
}

[MaterialProperty("_BaseColor")]
public struct CubeColor : IComponentData
{
    public Color color;
}

[GhostComponent]
public struct Points : IBufferElementData
{
    [GhostField] public NetworkTick tick;
}