using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Common.Behavior
{
    public class TextBoxTextChangedUpdateBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// Attach the Event. This will be fired when the user Enter some data in the TextBox
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.TextChanged += new TextChangedEventHandler(AssociatedObject_TextChanged);
        }

        /// <summary>
        /// The method implementation will detect the Binding value with the Assocated object
        /// and update the value in it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            BindingExpression sourceBinding = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            if (sourceBinding != null)
            {
                sourceBinding.UpdateSource();
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.TextChanged -= new TextChangedEventHandler(AssociatedObject_TextChanged);
        }
    }
}
