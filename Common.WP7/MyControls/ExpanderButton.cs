using System.Windows;
using System.Windows.Controls.Primitives;

namespace ObiWang.Controls
{
    [TemplateVisualState(Name = UnexpandedState, GroupName = ExpansionStates)]
    [TemplateVisualState(Name = ExpandedState, GroupName = ExpansionStates)]
    public class ExpanderButton : ToggleButton
    {
        private const string ExpansionStates = "ExpansionStates";
        private const string UnexpandedState = "Unexpanded";
        private const string ExpandedState = "Expanded";


        public ExpanderButton()
        {
            DefaultStyleKey = typeof(ExpanderButton);
        }

        /// <summary>
        /// Change the visual state.
        /// </summary>
        /// <param name="useTransitions">Indicates whether to use animation transitions.</param>
        private void ChangeVisualState(bool useTransitions)
        {
            if (IsChecked ?? false)
            {
                VisualStateManager.GoToState(this, ExpandedState, useTransitions);
            }
            else
            {
                VisualStateManager.GoToState(this, UnexpandedState, useTransitions);
            }
        }


        /// <summary>
        /// Called by the OnClick method to implement toggle behavior.
        /// </summary>
        protected override void OnToggle()
        {
            IsChecked = !(IsChecked ?? false);
            ChangeVisualState(true);
        }

        /// <summary>
        /// Gets all the template parts and initializes the corresponding state.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ChangeVisualState(false);
        }


    }
}
