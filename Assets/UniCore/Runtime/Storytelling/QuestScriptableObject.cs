using System.Collections.Generic;
using UnityEngine;

namespace KarenKrill.UniCore.Storytelling
{
    [CreateAssetMenu(fileName = "Quest", menuName = nameof(Storytelling) +"/Quest")]
    public class QuestScriptableObject : ScriptableObject
    {
        [field: SerializeField]
        public string QuestName { get; private set; } = string.Empty;
        [field: SerializeField]
        public string Description { get; private set; } = string.Empty;
        [field: SerializeField]
        public string IntroCutSceneId { get; private set; } = string.Empty;
        [field: SerializeField]
        public string CompletionOutroCutSceneId { get; private set; } = string.Empty;
        [field: SerializeField]
        public string FailureOutroCutSceneId { get; private set; } = string.Empty;
        public IReadOnlyList<QuestTaskScriptableObject> QuestTasks => _questTasks;

        [SerializeField]
        private List<QuestTaskScriptableObject> _questTasks = new();
    }
}
