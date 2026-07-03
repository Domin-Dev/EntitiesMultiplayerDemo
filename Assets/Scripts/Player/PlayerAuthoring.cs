using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] private float speed;
    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Player() { speed = authoring.speed });
            AddComponent(entity, new PlayerInput());
        }
    }
}

[GhostComponent]
public struct Player : IComponentData
{
    [GhostField] public float speed;
}


[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct PlayerInput : IInputComponentData
{
    [GhostField(Quantization = 0)] public float2 movementDirection;
}
 