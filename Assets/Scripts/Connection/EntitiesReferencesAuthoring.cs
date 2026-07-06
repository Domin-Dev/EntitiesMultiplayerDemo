using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


public class EntitiesReferencesAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                playerEntity = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic),          
            });
        }
    }
}
public struct EntitiesReferences : IComponentData
{
    public Entity playerEntity;
}