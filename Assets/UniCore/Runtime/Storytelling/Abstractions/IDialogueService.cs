using System;

namespace KarenKrill.UniCore.Storytelling.Abstractions
{
    public interface IDialogueService
    {
        bool CanContinue { get; }
        bool IsEnds { get; }

        public event Action<int, int, float> ClientServed; // конечно здесь не должно быть такиех конкретных событий, но сроки душат
        
        void StartDialogue(string id);
        void MakeDialogueChoice(int index);
        void NextDialogueLine();
        void SkipDialogue();
        void SetVariable(string name, bool state = true);
        string GetVariable(string name);
        void AddVariableListener(string varName, Action<object> callback);
        void RemoveVariableListener(string varName);
    }
}
