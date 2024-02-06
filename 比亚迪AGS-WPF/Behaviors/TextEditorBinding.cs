using System;
using System.Windows;
using ICSharpCode.AvalonEdit;

namespace 比亚迪AGS_WPF.Behaviors;

// public static class TextEditorBinding
// {
//     public static readonly DependencyProperty TextProperty =
//         DependencyProperty.RegisterAttached("Text", typeof(string), typeof(TextEditorBinding), new PropertyMetadata(null, TextChanged));
//
//     public static string GetText(DependencyObject obj)
//     {
//         return (string)obj.GetValue(TextProperty);
//     }
//
//     public static void SetText(DependencyObject obj, string value)
//     {
//         obj.SetValue(TextProperty, value);
//     }
//
//     private static void TextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
//     {
//         if (obj is TextEditor textEditor)
//         {
//             if (e.OldValue != null)
//             {
//                 textEditor.TextChanged -= TextEditor_TextChanged;
//             }
//
//             if (e.NewValue != null)
//             {
//                 textEditor.TextChanged += TextEditor_TextChanged;
//             }
//         }
//     }
//
//     private static void TextEditor_TextChanged(object sender, EventArgs e)
//     {
//         if (sender is TextEditor textEditor)
//         {
//             SetText(textEditor, textEditor.Text);
//         }
//     }
// }