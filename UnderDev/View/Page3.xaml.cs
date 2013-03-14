using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using S1Nyan.Views;
using S1Parser.Emotion;
using UnderDev.ViewModel;

namespace UnderDev
{
    public partial class Page3 : PhoneApplicationPage
    {
        EmotionListViewModel source = new EmotionListViewModel(EmotionParser.EmotionList.Values);
        public Page3()
        {
            InitializeComponent();
            source.ItemsPerPage = 25;
            emotionList.ItemsSource = source;
            emotionList.SelectionChanged += emotionPanel_SelectionChanged;
            ImageResourceManager.Reset();
            DataContext = new UserTestViewModel();
            BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public UserTestViewModel Vm
        {
            get
            {
                return (UserTestViewModel)DataContext;
            }
        }

        ApplicationBarIconButton replyButton;
        ApplicationBarIconButton emotionButton;
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            replyButton = new ApplicationBarIconButton(new Uri("Resources/appbar.reply.email.png", UriKind.Relative));
            replyButton.Text = "Reply";
            replyButton.Click += replyButton_Click;
            ApplicationBar.Buttons.Add(replyButton);

            emotionButton = new ApplicationBarIconButton(new Uri("Resources/appbar.smiley.happy.png", UriKind.Relative));
            emotionButton.Text = "Emotion";
            emotionButton.Click += EmotionButton_Click;
            ApplicationBar.Buttons.Add(emotionButton);
        }

        private void replyButton_Click(object sender, EventArgs e)
        {
            ToggleReplyPanelVisible();
        }

        private void ToggleReplyPanelVisible()
        {
            if (ReplyPanel.Visibility == Visibility.Visible)
            {
                ReplyPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                replyText.Focus();
                ReplyPanel.Visibility = Visibility.Visible;
            }
        }

        void EmotionButton_Click(object sender, EventArgs e)
        {
            ToggleEmotionPanelVisible();
        }

        private void ToggleEmotionPanelVisible()
        {
            if (EmotionPanel.Visibility == Visibility.Visible)
            {
                //reply.Focus();
                EmotionPanel.Visibility =  Visibility.Collapsed;
            }
            else
            {
                EmotionPanel.Visibility = Visibility.Visible;
            }
            SendButton.Focus();
        }

        void emotionPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = emotionList.SelectedItem as EmotionItemViewModel;
            if (item != null)
            {
                replyText.Text = replyText.Text + string.Format("[s:{0}]", item.Id); ;
            }
        }

        private void Pre_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            source.Pre();
        }

        private void Next_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            source.Next();
        }
    }
}