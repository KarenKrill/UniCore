using System;
using UnityEngine;

namespace KarenKrill.UniCore.Storytelling
{
    [CreateAssetMenu(fileName = "QuestTask", menuName = nameof(Storytelling) +"/QuestTask")]
    public class QuestTaskScriptableObject : ScriptableObject
    {
        [field: SerializeField]
        public string Description { get; private set; } = string.Empty;
        [field: SerializeField]
        public bool IsTransitionTask { get; private set; } = false;
        [field: SerializeField]
        public ActorScriptableObject Actor { get; private set; } = null;
        [field: SerializeField]
        public float PreDelay { get; private set; } = 0f;
        [field: SerializeField]
        public float PostDelay { get; private set; } = 0f;
        [field: SerializeField]
        public string IntroCutSceneId { get; private set; } = string.Empty;
        [field: SerializeField]
        public string CompletionOutroCutSceneId { get; private set; } = string.Empty;
        [field: SerializeField]
        public string FailureOutroCutSceneId { get; private set; } = string.Empty;
        [field: SerializeField]
        public QuestTaskScriptableObject NextTask { get; private set; } = null;
        [field: SerializeField]
        public QuestTaskScriptableObject NextFailureTask { get; private set; } = null;
    }
}
