using System;
using System.Collections.Generic;
using System.Reflection;

namespace KarenKrill.UniCore.Movement
{
    public static class MoveAbilityValidator
    {
        public static List<string> GetErrors(CharacterMoveBehaviour abilityRunner)
        {
            var errors = new List<string>();
            var providedContexts = new HashSet<Type>();
            foreach (var provider in abilityRunner.Providers)
            {
                var attr = provider?.GetType().GetCustomAttribute<ProvidesContextAttribute>();
                if (attr != null)
                {
                    providedContexts.Add(attr.ContextType);
                }
            }
            foreach (var ability in abilityRunner.Abilities)
            {
                if (ability != null)
                {
                    var required = ability.GetType().GetCustomAttributes<RequiredContextAttribute>();
                    foreach (var req in required)
                    {
                        if (!providedContexts.Contains(req.ContextType))
                        {
                            errors.Add($"{ability.GetType().Name} requires {req.ContextType.Name}, but no provider found.");
                        }
                    }
                }
            }
            return errors;
        }
    }
}
