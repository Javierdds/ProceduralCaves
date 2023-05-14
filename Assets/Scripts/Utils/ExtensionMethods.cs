using System;
using System.Collections.Generic;

public static class ExtensionMethods
{
    public static void Shuffle<T>(this IList<T> list, Random rnd)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            var k = rnd.Next(i + 1);
            var value = list[k];
            list[k] = list[i];
            list[i] = value;
        }
        //for (var i = list.Count; i > 0; i--)
        //    list.Swap(0, rnd.Next(0, i));
    }

    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
}
