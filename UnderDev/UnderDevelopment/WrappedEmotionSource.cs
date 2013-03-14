using System.Collections;
using System.Collections.Generic;
using S1Parser.Emotion;

namespace UnderDev.UnderDevelopment
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
}
