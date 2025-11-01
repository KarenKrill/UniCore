#nullable enable

using System;

namespace KarenKrill.Storytelling.Abstractions
{
    public interface IDialogueProvider
    {
        DialogueState DialogueState { get; }

        event Action<DialogueState>? DialogueStateChanged;
        event Action<string>? DialogueStarting;
        event Action<string>? DialogueStarted;
        event Action<string>? DialogueEnded;
    }
}