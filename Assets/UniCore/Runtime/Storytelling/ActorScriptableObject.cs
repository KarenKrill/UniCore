using UnityEngine;

namespace KarenKrill.UniCore.Storytelling
{
    [CreateAssetMenu(fileName = "JaneDoe", menuName = nameof(Storytelling) +"/Actor")]
    public class ActorScriptableObject : ScriptableObject
    {
        [field: SerializeField]
        public string Firstname { get; private set; } = "Jane";
        [field: SerializeField]
        public string Secondname { get; private set; } = "Doe";
        [field: SerializeField]
        public uint Age { get; private set; } = 27;
        [field: SerializeField]
        public Texture2D Icon { get; private set; } = null;
        [field: SerializeField]
        public string Description { get; private set; } = string.Empty;
        [Header("Is this actor known to the player?")]
        [field: SerializeField]
        public bool IsKnown { get; private set; } = false;
        [field: SerializeField]
        public GameObject Prefab { get; private set; }
        [field: SerializeField]
        public string DialogueId { get; private set; }
    }
}
