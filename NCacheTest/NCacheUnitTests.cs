using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NCache;

namespace NCacheTest
{
    [TestClass]
    public class NCacheUnitTests
    {
        const int SetCount = 10;
        const int BlockCount = 100;
        
        private static NWayCache<int, string> testCache = null;

        [ClassInitialize]
        public static void CacheInitialize(TestContext testContext)
        {
            ReplacementTypes.ReplacementType<int, string> replacementType = new ReplacementTypes.LRU<int, string>();
            testCache = new NWayCache<int, string>(SetCount, BlockCount, replacementType);
        }

        [TestMethod]
        public void AddItemsToCache()
        {
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    testCache.Add(i, RandomString(100));
                }
            }
            catch
            {
                Assert.Fail("Count is off");
            }
        }

        [TestMethod]
        public void GetItemFromCache()
        {
            string valueOut;
            for (int i = 0; i < 1000; i++)
            {
                valueOut = testCache.Get(i).ToString();
                if (valueOut == "")
                { 
                    Assert.Fail("Data Item not found!");
                }
            }
        }

        [TestMethod]
        public void UpdateItemsInCache()
        {
            try
            { 
            for (int i = 0; i < 1000; i++)
                {
                    testCache.Update(i, RandomString(100));
                }
            }
            catch
            {
                Assert.Fail("Count is off");
            }
        }

        [TestMethod]
        public void AddGetItemsCache()
        {

            string valueIn; 
            string valueOut;

            for (int i = 0; i < 1000; i++)
            {
                valueIn = RandomString(100);
                testCache.Add(i, valueIn);
                valueOut = testCache.Get(i).ToString();
                Assert.AreEqual(valueIn, valueOut, "Items not the same");
            }
        }

        /// <summary>
        /// Not really a test, but an example for custom replacement type. 
        /// </summary>
        [TestMethod]
        public void InitializeCustomReplacementType()
        {
            ReplacementTypes.ReplacementType<int, string> customReplacementType = new MyCustomReplacer<int, string>();
            testCache.Dispose();
            testCache = new NWayCache<int, string>(SetCount, BlockCount, customReplacementType);
        }

        public partial class MyCustomReplacer<mKey, mValue> : ReplacementTypes.ReplacementType<mKey, mValue>
        {
            // Custom code here
            // Remove method required 
            public override void Remove(LinkedList<CacheItemModel<mKey, mValue>> cacheSet)
            {
                cacheSet.Reverse();
                cacheSet.RemoveFirst();
            }
        }
 
        private static string RandomString(int stringLength)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, stringLength)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
            return result;
        }

        [ClassCleanupAttribute]
        public static void ClassCleanup()
        {
            testCache.Dispose();
        }
    }
}