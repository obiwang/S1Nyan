using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace ObiWang.Controls
{
    [TemplatePart(Name = Expander, Type = typeof(ExpanderButton))]
    [TemplatePart(Name = ChildItems, Type = typeof(ListBox))]
    public class ExpandableItem : ItemsControl
    {
        private const string Expander = "Expander";
        private ExpanderButton _expander;

        private const string ChildItems = "ChildItems";
        private ListBox _childItems;

        public ExpandableItem()
        {
            DefaultStyleKey = typeof(ExpandableItem);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _expander = GetTemplateChild(Expander) as ExpanderButton;

            _childItems = GetTemplateChild(ChildItems) as ListBox;
            if (_childItems != null)
            {
                _childItems.SelectionChanged += (o, e) =>
                {
                    if (SelectionChanged != null)
                    {
                        SelectionChanged(this, e);
                    }
                };
            }
            OnExpandChanged();
        }

        // Summary:
        //     Occurs when the currently selected item changes.
        public event SelectionChangedEventHandler SelectionChanged;

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ExpandableItem), new PropertyMetadata(false, OnExpandChanged));

        public bool HasItems
        {
            get { return (bool)GetValue(HasItemsProperty); }
            set { SetValue(HasItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasItemsProperty =
            DependencyProperty.Register("HasItems", typeof(bool), typeof(ExpandableItem), new PropertyMetadata(false));

        public string TitleText
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register("TitleText", typeof(string), typeof(ExpandableItem), new PropertyMetadata(null));

        public static void OnExpandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ExpandableItem).OnExpandChanged();
        }

        private void OnExpandChanged()
        {
            if (_childItems == null) return;
            _childItems.Visibility = IsExpanded ? Visibility.Visible : Visibility.Collapsed;
        }

        #region ItemsControl overriden methods

        /// <summary>
        /// Updates the HasItems property.
        /// </summary>
        /// <param name="e">The event information.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            HasItems = Items.Count > 0;
        }

        #endregion

        public object SelectedItem
        {
            get
            {
                if (_childItems != null)
                    return _childItems.SelectedItem;
                else return null;
            }
            set
            {
                if (_childItems != null)
                    _childItems.SelectedItem = value;
                else return;
            }
        }
    }
}
