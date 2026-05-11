using UnityEditor;
using UnityEngine;

namespace KarenKrill.UniCore.Movement.Editor
{
    [InitializeOnLoad]
    public static class PlayModeMoveAbilityValidator
    {
        static PlayModeMoveAbilityValidator()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                if (!ValidateScene())
                {
                    EditorApplication.isPlaying = false;
                }
            }
        }

        private static bool ValidateScene()
        {
            var abilityRunners = Object.FindObjectsByType<CharacterMoveBehaviour>(FindObjectsSortMode.None);
            foreach (var abilityRunner in abilityRunners)
            {
                var errors = MoveAbilityValidator.GetErrors(abilityRunner);
                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        Debug.LogError(error, abilityRunner);
                    }
                    return false;
                }
            }
            return true;
        }
    }
}
