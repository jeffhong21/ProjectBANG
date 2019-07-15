namespace CharacterController
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Item Position", menuName = "Character Controller/Items/ItemType Position", order = 2100)]
    public class ItemIKAdjustment : ScriptableObject
    {
        public Vector3 position;

        public Vector3 rotation;

    }
}


