using System.Windows;
using Caliburn.Micro.BindableAppBar;
using Microsoft.Phone.Controls;

namespace ObiWang.Controls 
{
    public class ThemeableBindableAppBar : BindableAppBar
    {

        public ThemeableBindableAppBar()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Light theme
            MatchOverriddenTheme();
            StateChanged += (o, args) => MatchOverriddenTheme();
            Invalidated += (o, args) => MatchOverriddenTheme();
        }

        private void MatchOverriddenTheme()
        {
            ThemeManager.MatchOverriddenTheme(this);
        }
    }
}