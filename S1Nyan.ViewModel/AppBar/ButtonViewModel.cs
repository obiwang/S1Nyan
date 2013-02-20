using System;

namespace S1Nyan.ViewModel
{
    public class ButtonViewModel : MenuItemViewModel
    {
        #region IconUri
        private Uri uri;

        /// <summary>
        /// Gets or sets the icon URI.
        /// </summary>
        /// <value>
        /// The icon URI.
        /// </value>
        public Uri IconUri
        {
            get
            {
                return this.uri;
            }
            set
            {
                if (this.uri != value)
                {
                    this.uri = value;
                    RaisePropertyChanged("IconUri");
                }
            }
        }
        #endregion
    }
}