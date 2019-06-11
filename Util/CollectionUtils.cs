using System.Collections.Generic;

namespace MirRemakeBackend.Util {
    class CollectionUtils {
        public static List<T> GetDictKeyList<T, U> (Dictionary<T, U> dict) {
            var res = new List<T> (dict.Count);
            var en = dict.GetEnumerator ();
            while (en.MoveNext ())
                res.Add (en.Current.Key);
            return res;
        }
        public static List<U> GetDictValueList<T, U> (Dictionary<T, U> dict) {
            var res = new List<U> (dict.Count);
            var en = dict.GetEnumerator ();
            while (en.MoveNext ())
                res.Add (en.Current.Value);
            return res;
        }
        public static List<T> GetSetList<T> (HashSet<T> set) {
            var res = new List<T> (set.Count);
            var en = set.GetEnumerator ();
            while (en.MoveNext ())
                res.Add (en.Current);
            return res;
        }
    }
}