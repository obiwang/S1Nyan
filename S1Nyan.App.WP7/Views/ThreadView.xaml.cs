using System.Windows;
using System.Windows.Controls;
using S1Parser;

namespace S1Nyan.App.Views
{
    public partial class ThreadPage
    {
        private void StackPanelLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            StackPanel panel = sender as StackPanel;
            if (panel == null) return;
            S1ThreadItem thread = panel.DataContext as S1ThreadItem;
            if (thread != null)
            {
                RichTextBox r = new RichTextBox { TextWrapping = TextWrapping.Wrap, FontSize = 25 };
                r.Blocks.Add(BuildRichText(thread.Content));
                panel.Children.Add(r);
            }
        }

    }
}