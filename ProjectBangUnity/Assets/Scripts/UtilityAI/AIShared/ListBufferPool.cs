using System;
using System.Collections;
using System.Collections.Generic;

namespace AtlasAI
{
    /// <summary>
    /// A buffer pool for C# lists to avoid untimely garbage collection.
    /// </summary>
    public static class ListBufferPool
    {
        //
        // Static Fields
        //
        private static readonly Dictionary<Type, Queue<IList>> _pool;

        //
        // Static Methods
        //

        // get a list allocate from the buffer with a capacity of 5
        public static List<T> GetBuffer<T>(int capacityHint)
        {
            //  var gameObjects = ListBufferPool.GetBuffer<GameObject>(5)
            throw new NotImplementedException();
        }

        public static void PreAllocate<T>(int capacity, int number = 1)
        {
            throw new NotImplementedException();
        }
            
        //  return the list to the buffer pool after usage, so other parts of the code can reuse the list
        public static void ReturnBuffer<T>(List<T> buffer)
        {
            throw new NotImplementedException();
        }
    }
}
