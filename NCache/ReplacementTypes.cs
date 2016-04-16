using System;
using System.Collections.Generic;
using System.Linq;

namespace NCache
{

    public class ReplacementTypes
    {

        /// <summary>
        /// Abstract class for removal of records, allows for custom replacement type
        /// </summary>
        /// <typeparam name="mKey"></typeparam>
        /// <typeparam name="mValue"></typeparam>
        public abstract class ReplacementType<mKey, mValue>
        {
            public abstract void Remove(LinkedList<CacheItemModel<mKey, mValue>> cacheSet);
        }

        /// <summary>
        /// MRU implementation
        /// </summary>
        /// <typeparam name="mKey"></typeparam>
        /// <typeparam name="mValue"></typeparam>
        public class Mru<mKey, mValue> : ReplacementType<mKey, mValue>
        {
            public override void Remove(LinkedList<CacheItemModel<mKey, mValue>> cacheSet)
            {
                if (cacheSet.Any())
                {
                    cacheSet.RemoveLast();
                }
            }
        }

        /// <summary>
        /// LRU implementation 
        /// </summary>
        /// <typeparam name="mKey"></typeparam>
        /// <typeparam name="mValue"></typeparam>
        public class LRU<mKey, mValue> : ReplacementType<mKey, mValue>
        {
            public override void Remove(LinkedList<CacheItemModel<mKey, mValue>> cacheSet)
            {
                if (cacheSet.Any())
                {
                    cacheSet.RemoveFirst();
                }
            }
        }
    }
}
