using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace ObiWang.Controls
{
    [TemplatePart(Name = Expander, Type = typeof(ExpanderButton))]
    [TemplatePart(Name = TitleContent, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = ChildItems, Type = typeof(ListBox))]
    public class ExpandableItem : ItemsControl
    {
        private const string Expander = "Expander";
        private ExpanderButton _expander;

        private const string TitleContent = "TitleContent";
        private ContentPresenter _titleContent;

        private const string ChildItems = "ChildItems";
        private ListBox _childItems;

        public event RoutedEventHandler HeaderTapped;

        public ExpandableItem()
        {
            DefaultStyleKey = typeof(ExpandableItem);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _expander = GetTemplateChild(Expander) as ExpanderButton;
            _titleContent = GetTemplateChild(TitleContent) as ContentPresenter;
            if (_titleContent != null)
            {
                _titleContent.Tap -= OnHeaderTapped;
                _titleContent.Tap += OnHeaderTapped;
            }

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

        private void OnHeaderTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (HeaderTapped != null) HeaderTapped(sender, e);
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

        public object Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(ExpandableItem), new PropertyMetadata(null));

        public static void OnExpandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ExpandableItem).OnExpandChanged();
        }

        public static readonly DependencyProperty TitleContentTemplateProperty =
            DependencyProperty.Register("TitleContentTemplate", typeof (DataTemplate), typeof (ExpandableItem), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate TitleContentTemplate
        {
            get { return (DataTemplate) GetValue(TitleContentTemplateProperty); }
            set { SetValue(TitleContentTemplateProperty, value); }
        }

        private void OnExpandChanged()
        {
            if (_childItems == null) return;
            _childItems.Visibility = IsExpanded ? Visibility.Visible : Visibility.Collapsed;
            UpdateLayout();
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
