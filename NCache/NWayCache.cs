using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NCache
{
    /// <summary>
    /// N-Way Set Associative Cache Using Generics
    /// </summary>
    /// <typeparam name="mKey"></typeparam>
    /// <typeparam name="mValue"></typeparam>
    public class NWayCache<mKey, mValue> : INWayCacheSet<mKey, mValue>, IDisposable
    {
        
        private int _setCount;
        private int _blockSize;
        // ToDo: Some testing will be needed to tell if this is the fastest access mechanism for this solution. 
        private Dictionary<int, LinkedList<CacheItemModel<mKey, mValue>>> _cacheSets;
        ReplacementTypes.ReplacementType<mKey, mValue> _replacementType; 
        
        /// <summary>
        /// Initilizes the class with number of sets, size of sets, and replacment type
        /// </summary>
        /// <param name="setCountSize"></param>
        /// <param name="setBlockSize"></param>
        /// <param name="replacementType"></param>
        public NWayCache(int setCountSize, int setBlockSize, ReplacementTypes.ReplacementType<mKey, mValue> replacementType)
        {
            _setCount = setCountSize;
            _blockSize = setBlockSize;
            _cacheSets = new Dictionary<int, LinkedList<CacheItemModel<mKey, mValue>>>();
            BuildCacheSets(_setCount, _blockSize);
            _replacementType = replacementType;
		}

        /// <summary>
        /// Build out defined number of sets
        /// </summary>
        /// <param name="setCount"></param>
        /// <param name="blockSize"></param>
        private void BuildCacheSets(int setCount, int blockSize)
        {
            for (int i = 0; i < setCount; i++)
            {
                _cacheSets[i] = new LinkedList<CacheItemModel<mKey, mValue>>();
            }
        }

        /// <summary>
        /// Allows the user to determine the number of sets. 
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return (_cacheSets == null) ? 0 : _cacheSets.Count();
        }

        /// <summary>
        /// Allows the user to clear the class. This will remove all sets. 
        /// </summary>
        public void Clear()
        {
           _cacheSets.Clear();
        }

        /// <summary>
        /// Both the add and update methods are combined here as we will need
        /// to check if the value exists before adding. Seperate methods 
        /// allow this to be changed later without issues to the end user. 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void Add(mKey Key, mValue Value)
        {
            if (!String.IsNullOrEmpty(Key.ToString()) && !String.IsNullOrEmpty(Value.ToString()))
            {
                LinkedList<CacheItemModel<mKey, mValue>> cacheSet = GetCacheSet(Key);
                {
                    // Does record already exist? If yes update it, else add it
                    CacheItemModel<mKey, mValue> foundRecord = GetRecord(Key, cacheSet);
                    if (foundRecord == null)
                    {
                        CacheItemModel<mKey, mValue> newEntry = new CacheItemModel<mKey, mValue>();
                        newEntry.Key = Key;
                        newEntry.Value = Value;
                        newEntry.DateTimeCreated = DateTime.Now;
                        lock (cacheSet)
                        {
                            // Remove a record according to replacment type
                            // if the cache set is at its max blocksize
                            if (cacheSet.Count >= _blockSize)
                            {
                                _replacementType.Remove(cacheSet);
                                cacheSet.AddFirst(newEntry);
                            }
                            else
                            {
                                // By default using FIFO
                                cacheSet.AddFirst(newEntry);
                            }
                        }
                    }
                    else
                    {
                        // update record, updating its last accessed timestamp and 
                        // moving it to the top of the list
                        cacheSet.Remove(foundRecord);
                        foundRecord.Value = Value;
                        foundRecord.DateTimeLastAccessed = DateTime.Now;
                        cacheSet.AddFirst(foundRecord);
                    }
                }
            }
            else
            {
                throw new ArgumentNullException(String.Format("Key and Value must be supplied."));
            }
        }

        /// <summary>
        /// See: Add()
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void Update(mKey Key, mValue Value)
        {
            Add(Key, Value);
        }

        /// <summary>
        /// Removes a record by key. 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool Remove(mKey Key)
		{
            if (!String.IsNullOrEmpty(Key.ToString()))
            {
               
                LinkedList<CacheItemModel<mKey, mValue>> cacheSet = GetCacheSet(Key);
                CacheItemModel<mKey, mValue> foundRecord = GetRecord(Key, cacheSet);
                if (foundRecord != null)
                { 
                    cacheSet.Remove(foundRecord);
                }
                // We return true here to indicate that no record exists. 
                return true;
            }
            else
            {
                throw new ArgumentNullException(String.Format("Key must be supplied."));
            }
		} 

        /// <summary>
        /// Returns the value for a key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public mValue Get(mKey Key)
        {
             if (!String.IsNullOrEmpty(Key.ToString()))
             {
                LinkedList<CacheItemModel<mKey, mValue>> cacheSet = GetCacheSet(Key);
                CacheItemModel<mKey, mValue> foundRecord = GetRecord(Key, cacheSet);
                if (foundRecord != null)
                {
                    foundRecord.DateTimeLastAccessed = DateTime.Now;
                    cacheSet.Remove(foundRecord);
                    cacheSet.AddFirst(foundRecord);
                    return foundRecord.Value;
                }
                else
                {
                    return default(mValue);
                }
             }
            else
             {
                 throw new ArgumentNullException(String.Format("Key must be supplied."));
             }
        }

        /// <summary>
        /// Building out Cache Sets as LinkedLists for access to top and bottom
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        internal LinkedList<CacheItemModel<mKey, mValue>> GetCacheSet(mKey Key)
        {
            var setIndex = Math.Abs(Key.GetHashCode() % _setCount);
            return _cacheSets[setIndex];
        }

        /// <summary>
        /// Find mechanism to retrieve a record
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="cacheSet"></param>
        /// <returns></returns>
        internal CacheItemModel<mKey, mValue> GetRecord(mKey Key, LinkedList<CacheItemModel<mKey, mValue>> cacheSet)
        {
            CacheItemModel<mKey, mValue> record = null;
            record = cacheSet.Where(x => EqualityComparer<mKey>.Default.Equals(x.Key, Key)).FirstOrDefault() as CacheItemModel<mKey, mValue>;
            return record;
        }

        /// <summary>
        /// Dispose of class. 
        /// </summary>
        public void Dispose()
        {
            if (_cacheSets == null)
            {
                return;
            }
            this.Clear();
            _cacheSets = null;
        }
    }
}
