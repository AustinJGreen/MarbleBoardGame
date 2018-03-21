using System;
using System.Collections.Generic;

namespace MarbleBoardGame
{
    public class NodeStack<T>
    {
        private int[] stackCounts;
        private Queue<T>[] queues;

        public int Stacks { get; private set; }

        public int Count { get; private set; }

        public void Enqueue(T item, int depth)
        {
            queues[depth].Enqueue(item);
            stackCounts[depth]++;
            Count++;
        }

        public T Dequeue(ref int depth)
        {
            if (Count == 0)
            {
                return default(T);
            }

            for (int p = 0; p < Stacks; p++)
            {
                if (stackCounts[p] != 0)
                {
                    depth = p;
                    stackCounts[p]--;
                    Count--;
                    return queues[p].Dequeue();
                }
            }

            throw new Exception("Corrupted stack.");
        }

        public NodeStack(int stacks)
        {
            stackCounts = new int[stacks];
            queues = new Queue<T>[stacks];
            for (int i = 0; i < stacks; i++)
            {
                queues[i] = new Queue<T>();
            }

            Stacks = stacks;
        }
    }
}
