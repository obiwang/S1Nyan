using System;
using System.Windows;
using S1Nyan.Model;
using S1Parser.User;

namespace S1Nyan.Views
{
    partial class ThreadView
    {
        private void OnReplyButton(object sender, EventArgs e)
        {
            ToggleReplyPanelVisible();
            if (savedReply != null)
            {
                replyText.Text = savedReply;
                savedReply = null;
            }
        }

        bool IsReplyPanelVisible { get { return ReplyPanel.Visibility == Visibility.Visible; } }
        private void ToggleReplyPanelVisible()
        {
            if (IsNavigatorVisible) 
                ShowHideNavi(true);
            ShowHideReplyPanel(IsReplyPanelVisible);
        }

        private void ShowHideReplyPanel(bool hide)
        {
            if (hide)
            {
                ReplyPanel.Visibility = Visibility.Collapsed;
                replyButton.IconUri = replyIcon;
            }
            else
            {
                ApplicationBar.IsVisible = false;
                ReplyPanel.Visibility = Visibility.Visible;
                replyButton.IconUri = replyIconInvert;
                replyText.Focus();
            }
        }

        #region SIP margin walkaround

        void replyText_LostFocus(object sender, RoutedEventArgs e)
        {
            ApplicationBar.IsVisible = true;
        }

        void replyText_GotFocus(object sender, RoutedEventArgs e)
        {
            ApplicationBar.IsVisible = false;
            replyText.Focus();
        }
        #endregion
    }
}
