using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*namespace OS_2
{
    class PriorityQueue<T>
    {
        IComparer<T> comparer;
        public T[] heap;
        int now;

        public int Count { get; private set; }

        public PriorityQueue(IComparer<T> comparer)
        {
            this.comparer = comparer;
            this.heap = new T[0];
        }

        public void Enqueue(T v)
        {
            Count++;
            if (Count >= heap.Length) Array.Resize(ref heap, Count);
            heap[Count - 1] = v;
            Array.Sort(heap,1,Count - 1, comparer);
        }

        public T Dequeue(int nowTime)
        {
            var v = Peek();
            heap[0] = heap[--Count];
            Array.Resize(ref heap, Count);

            if (Count > 0)
            {
                Array.Sort(heap, comparer);
            }
            
            return v;
        }

        public T Peek()
        {
            if (Count > 0) return heap[0];
            throw new InvalidOperationException("优先队列为空");
        }
    }

    class Compare : IComparer<FCFS_Exexute_unit>
    {
        int mode;
        public Compare(int i)
        {
            mode = i;
        }
        int IComparer<FCFS_Exexute_unit>.Compare(FCFS_Exexute_unit x, FCFS_Exexute_unit y)
        {
            if (mode == 0)
            {
                if (x.node.service_time < y.node.service_time) return -1;
                else if (x.node.service_time < y.node.service_time) return 0;
                else return 1;
            }

            else
            {
                //float x_weight = x.node.arrive_time
            }
            return 1;
            
        }
    }
}

*/

namespace OS_2
{
    class PriorityQueue
    {
        IComparer<FCFS_Exexute_unit> comparer;
        public FCFS_Exexute_unit[] heap;
        int now;

        public int Count { get; private set; }

        public PriorityQueue(IComparer<FCFS_Exexute_unit> comparer)
        {
            this.comparer = comparer;
            this.heap = new FCFS_Exexute_unit[0];
        }

        public void Enqueue(FCFS_Exexute_unit v, int nowTime)
        {
            Count++;
            if (Count >= heap.Length) Array.Resize(ref heap, Count);
            heap[Count - 1] = v;
            for (int i = 0; i < Count; i++) heap[i].nowTime = nowTime;
            Array.Sort(heap, 1, Count - 1, comparer);
        }

        public FCFS_Exexute_unit Dequeue()
        {
            var v = Peek();
            int nowTime = v.finish_time;
            heap[0] = heap[--Count];
            Array.Resize(ref heap, Count);

            for (int i = 0; i < Count; i++) heap[i].nowTime = nowTime;

            if (Count > 0)
            {
                Array.Sort(heap, comparer);
            }

            return v;
        }

        public FCFS_Exexute_unit Peek()
        {
            if (Count > 0) return heap[0];
            throw new InvalidOperationException("优先队列为空");
        }
    }

    class Compare : IComparer<FCFS_Exexute_unit>
    {
        int mode;
        public Compare(int i)
        {
            mode = i;
        }
        int IComparer<FCFS_Exexute_unit>.Compare(FCFS_Exexute_unit x, FCFS_Exexute_unit y)
        {
            if (mode == 0)
            {
                if (x.node.service_time < y.node.service_time) return -1;
                else if (x.node.service_time == y.node.service_time) return 0;
                else return 1;
            }

            else
            {
                float x_w = (float)((x.nowTime - x.node.arrive_time) + x.node.service_time) / (float)x.node.service_time;
                float y_w = (float)((y.nowTime - y.node.arrive_time) + y.node.service_time) / (float)y.node.service_time;
                if (x_w < y_w) return 1;
                else if (x_w == y_w) return 0;
                else return -1;
            }
        }
    }
}