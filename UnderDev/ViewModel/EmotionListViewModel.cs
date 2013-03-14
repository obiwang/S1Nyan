using System.Collections;
using System.Collections.Generic;
using System.Linq;
using S1Parser.Emotion;

namespace UnderDev.ViewModel
{
    public class EmotionListViewModel : IEnumerable<EmotionItemViewModel>
    {
        List<EmotionItemViewModel> _source;
        List<EmotionItemViewModel> _outList;
        public EmotionListViewModel(IEnumerable<EmotionItemViewModel> source)
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
            }

            return _outList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
