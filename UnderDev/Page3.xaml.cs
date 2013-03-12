using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using S1Nyan.Views;
using S1Parser.Emotion;
using System.Linq;

namespace UnderDev
{
    public class WrappedEmotionSource : IEnumerable<List<EmotionItemViewModel>>
    {
        IEnumerable<EmotionItemViewModel> _source;
        public WrappedEmotionSource(IEnumerable<EmotionItemViewModel> source)
        {
            _source = source;
        }

        public IEnumerator<List<EmotionItemViewModel>> GetEnumerator()
        {
            int sum = 0;
            List<EmotionItemViewModel> line = null;
            foreach (var item in _source)
            {
                sum++;
                if (line == null) line = new List<EmotionItemViewModel>();
                line.Add(item);
                if (sum % 5 == 0)
                {
                    yield return line;
                    line = null;
                }
            }
            if (line != null)
                yield return line;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class EmotionSource : IEnumerable<EmotionItemViewModel>//, INotifyCollectionChanged
    {
        List<EmotionItemViewModel> _source;
        List<EmotionItemViewModel> _outList;
        public EmotionSource(IEnumerable<EmotionItemViewModel> source)
        {
            _source = source.ToList<EmotionItemViewModel>();
            total = _source.Count;
        }

        public int ItemsPerPage { get; set; }

        private int total = 0;
        private int currentPage = 0;

        /// <summary>
        /// 0 based
        /// </summary>
        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (total == 0 || ItemsPerPage <= 0 || value < 0 || value > total / ItemsPerPage) return;
                currentPage = value;
                NotifyCollectionChanged();
            }
        }

        public void Next()
        {
            CurrentPage++;
        }

        public void Pre()
        {
            CurrentPage--;
        }

        private void NotifyCollectionChanged()
        {
            var offset = currentPage * ItemsPerPage;
            var end = offset + ItemsPerPage;
            end = end < total ? end : total;
            var i = offset;
            foreach (var item in this)
            {
                if (i < end)
                {
                    item.Id = _source[i].Id;
                    item.Path = _source[i].Path;
                }
                else
                {
                    item.Id = "";
                }
                item.NotifyChanged();
                i++;
            }
        }

        public IEnumerator<EmotionItemViewModel> GetEnumerator()
        {
            if (_outList == null)
            {
                _outList = new List<EmotionItemViewModel>();
                var end = ItemsPerPage < total ? ItemsPerPage : total;
                for (int i = 0; i < end; i++)
                {
                    _outList.Add(_source[i].Clone());
                }
                //var offset = currentPage * ItemsPerPage;
                //var end = offset + ItemsPerPage;
                //end = end < total ? end : total;
                //for (int i = offset; i < end; i++)
                //    yield return _source[i];
            }

            return _outList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //public event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    public partial class Page3 : PhoneApplicationPage
    {
        EmotionSource source = new EmotionSource(EmotionParser.EmotionList.Values);
        public Page3()
        {
            InitializeComponent();
            source.ItemsPerPage = 25;
            emotionPanel.ItemsSource = source;
            emotionPanel.SelectionChanged += emotionPanel_SelectionChanged;

            ImageResourceManager.Reset();
        }

        void emotionPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = emotionPanel.SelectedItem as EmotionItemViewModel;
            if (item != null)
            {
                echo.Text = item.Path;
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