using System;
using System.Collections.Generic;

namespace NCache
{
    /// <summary>
    /// Interface for NWayCache class
    /// </summary>
    /// <typeparam name="mKey"></typeparam>
    /// <typeparam name="mValue"></typeparam>
    public interface INWayCacheSet<mKey, mValue>
    {
        int Count();
        void Clear();
        void Add(mKey Key, mValue Value);
        void Update(mKey Key, mValue Value);
        bool Remove(mKey Key);
        mValue Get(mKey Key);
    }
}
