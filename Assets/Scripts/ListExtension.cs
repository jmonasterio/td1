using System.Collections.Generic;

namespace Assets.Scripts
{
    static public class ListExtension
    {
        public static void Merge<T>(this List<T> list, T t)
        {
            if (!list.Contains(t))
            {
                list.Add(t);
            }
        }
    }
}