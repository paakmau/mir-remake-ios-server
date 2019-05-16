using System.Collections.Generic;

namespace MirRemakeBackend {
    class CollectionUtils {
        public static List<KeyValuePair<T, K>> DictionaryToList<T, K> (Dictionary<T, K> dict) {
            List<KeyValuePair<T, K>> res = new List<KeyValuePair<T, K>> ();
            var dictEn = dict.GetEnumerator ();
            while (dictEn.MoveNext ()) 
                res.Add (dictEn.Current);
            return res;
        }
        public static List<K> DictionaryToValueList<T, K> (Dictionary<T, K> dict) {
            List<K> res = new List<K> ();
            var valueEn = dict.Values.GetEnumerator ();
            while (valueEn.MoveNext ())
                res.Add (valueEn.Current);
            return res;
        }
    }
}