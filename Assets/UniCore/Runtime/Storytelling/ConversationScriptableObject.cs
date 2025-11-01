using UnityEngine;

namespace KarenKrill.UniCore.Storytelling
{
    [CreateAssetMenu(fileName = "Conversation", menuName = nameof(Storytelling) + "/Conversation")]
    public class ConversationScriptableObject : ScriptableObject
    {
        public ActorScriptableObject[] actors;
        public string conversation;
    }
}
