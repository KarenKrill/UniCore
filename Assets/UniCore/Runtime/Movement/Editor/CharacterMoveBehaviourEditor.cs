using UnityEditor;

namespace KarenKrill.UniCore.Movement.Editor
{
    [CustomEditor(typeof(CharacterMoveBehaviour))]
    public class CharacterMoveBehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var abilityRunner = (CharacterMoveBehaviour)target;
            var errors = MoveAbilityValidator.GetErrors(abilityRunner);
            EditorGUILayout.BeginHorizontal();
            foreach (var error in errors)
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
