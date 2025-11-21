namespace KarenKrill.UniCore.UI.Views.Abstractions
{
    public interface IView
    {
        bool Interactable { get; set; }
        int SortOrder { get; }

        void Show(bool smoothly = true);
        void Close(bool smoothly = true);
        void SetFocus(bool isFocused);
    }
}