using System.Windows.Input;

namespace S1Nyan.ViewModel
{
    public class MenuItemViewModel : ViewModel
    {
        #region Command
        private ICommand command;

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command.
        /// </value>
        public ICommand Command
        {
            get { return this.command; }
            set
            {
                if(this.command != value)
                {
                    this.command = value;
                    RaisePropertyChanged("Command");
                }
            }
        }
        #endregion

        #region CommandParameter
        private object commandParameter;

        /// <summary>
        /// Gets or sets the command's parameter.
        /// </summary>
        /// <value>
        /// The command's parameter.
        /// </value>
        public object CommandParameter
        {
            get { return this.commandParameter; }
            set
            {
                if(this.commandParameter != value)
                {
                    this.commandParameter = value;
                    RaisePropertyChanged("CommandParameter");
                }
            }
        }
        #endregion

        #region Text
        private string text;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get { return this.text; }
            set
            {
                if(this.text != value)
                {
                    this.text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }
        #endregion
    }
}