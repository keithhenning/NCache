using System;
using System.Collections.Generic;

namespace NCache
{
    /// <summary>
    /// Model for cache record
    /// </summary>
    /// <typeparam name="mKey"></typeparam>
    /// <typeparam name="mValue"></typeparam>
    public class CacheItemModel<mKey, mValue>
    {
        public mKey Key { get; set; }
        public mValue Value { get; set;}

        // DateTime Created and Last Accessed are intended
        // for internal performace analysis and monitoring,
        // though could be exposed to clients at a later time
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeLastAccessed { get; set; }
    }
}
