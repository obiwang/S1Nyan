using System.Collections.Generic;
using System.Linq;

namespace S1Nyan.ViewModels
{
    public class ListWithGroupKey<TKey, TValue> : List<TValue>, IGrouping<TKey, TValue>
    {

        public ListWithGroupKey(TKey key)
        {
            Key = key;
        }

        public ListWithGroupKey(TKey key, IEnumerable<TValue> list)
            : base(list)
        {
            Key = key;
        }

        public TKey Key { get; private set; }
    }
}