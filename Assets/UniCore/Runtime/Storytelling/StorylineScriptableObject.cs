using System.Collections.Generic;
using UnityEngine;

namespace KarenKrill.UniCore.Storytelling
{
    [CreateAssetMenu(fileName = "Storyline", menuName = nameof(Storytelling) + "/Storyline")]
    public class StorylineScriptableObject : ScriptableObject
    {
        [field: SerializeField]
        public string Description { get; private set; } = string.Empty;
        [field: SerializeField]
        public bool IsMain { get; private set; } = true;
        public IReadOnlyList<QuestScriptableObject> Quests => _quests;

        [SerializeField]
        private List<QuestScriptableObject> _quests = new();
    }
}
