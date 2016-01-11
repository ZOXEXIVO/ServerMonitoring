using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ServerMonitoring.Data.Collections
{
    public class FixedSizeConcurrentQueue<T> : ConcurrentQueue<T>
    {
        public string Name { get; set; }
        public string Display { get; set; }

        public int CssScreen { get; set; }
        public int TotalCount { get; set; }

        public FixedSizeConcurrentQueue(int limit) : this(limit, Enumerable.Empty<T>())
        {
        }

        public FixedSizeConcurrentQueue(int limit, IEnumerable<T> initialValues)
        {
            Limit = limit;

            foreach (var val in initialValues)
                Enqueue(val);
        }

        public new void Enqueue(T val)
        {
            base.Enqueue(val);

            T temp;
            if (Count > Limit)
                TryDequeue(out temp);
        }

        public int Limit { get; set; }
    }
}
