#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace KarenKrill.Storytelling.Abstractions
{
    public class DialogueState
    {
        public string FlowName
        {
            get => _flowName;
            set
            {
                if (_flowName != value)
                {
                    _flowName = value;
                    NameChanged?.Invoke();
                }
            }
        }
        public string Line
        {
            get => _line;
            set
            {
                if (_line != value)
                {
                    _line = value;
                    LineChanged?.Invoke();
                }
            }
        }
        public IReadOnlyList<string> Choices
        {
            get => _choices;
            set
            {
                if (_choices != value)
                {
                    if (_choices.Count == value.Count)
                    {
                        bool areListsEqual = true;
                        for (int i = 0; i < _choices.Count; i++)
                        {
                            if (_choices[i] != value[i])
                            {
                                areListsEqual = false;
                                break;
                            }
                        }
                        if (areListsEqual)
                        {
                            _choices = value;
                            return;
                        }
                    }
                    _choices = value;
                    ChoicesChanged?.Invoke();
                }
            }
        }
        public IReadOnlyList<string> Tags
        {
            get => _tags;
            set
            {
                if (_tags != value)
                {
                    if (_tags.Count == value.Count)
                    {
                        bool areListsEqual = true;
                        for (int i = 0; i < _tags.Count; i++)
                        {
                            if (_tags[i] != value[i])
                            {
                                areListsEqual = false;
                                break;
                            }
                        }
                        if (areListsEqual)
                        {
                            _tags = value;
                            return;
                        }
                    }
                    _tags = value;
                    TagsChanged?.Invoke();
                }
            }
        }

        public event Action? NameChanged;
        public event Action? LineChanged;
        public event Action? ChoicesChanged;
        public event Action? TagsChanged;

        public DialogueState(string flowName,
            string line,
            IReadOnlyList<string> choices,
            IReadOnlyList<string> tags)
        {
            _flowName = flowName;
            _line = line;
            _choices = choices;
            _tags = tags;
        }

        private string _flowName;
        private string _line;
        private IReadOnlyList<string> _choices;
        private IReadOnlyList<string> _tags;
    }
}
