using System.Windows;
using ICSharpCode.AvalonEdit;

namespace 比亚迪AGS_WPF.Behaviors;

public static class TextEditorExtensions
{
    public static readonly DependencyProperty CodeTextProperty =
        DependencyProperty.RegisterAttached(
            "CodeText",
            typeof(string),
            typeof(TextEditorExtensions),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                PropertyChangedCallback));

    private static void PropertyChangedCallback(DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var textEditor = dependencyObject as TextEditor;
        if (textEditor != null)
        {
            if (dependencyPropertyChangedEventArgs.NewValue != null)
            {
                // Save the caret offset.
                var caretOffset = textEditor.CaretOffset;

                // Update the text.
                textEditor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();

                // Restore the caret offset.
                textEditor.CaretOffset = caretOffset;
            }

            textEditor.TextChanged += (sender, args) =>
            {
                // Save the caret offset.
                var caretOffset = textEditor.CaretOffset;

                // Update the CodeText property.
                SetCodeText(textEditor, textEditor.Document.Text);

                // Restore the caret offset.
                textEditor.CaretOffset = caretOffset;
            };
        }
    }

    public static void SetCodeText(UIElement element, string value)
    {
        element.SetValue(CodeTextProperty, value);
    }

    public static string GetCodeText(UIElement element)
    {
        return (string)element.GetValue(CodeTextProperty);
    }
}