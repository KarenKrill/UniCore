using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace KarenKrill.UniCore.Movement.Editor
{
    public class BuildMoveAbilityValidator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!Validate())
            {
                throw new BuildFailedException("Ability validation failed");
            }
        }

        private bool Validate()
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
