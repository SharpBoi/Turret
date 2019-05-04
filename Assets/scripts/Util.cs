using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum MyOS { None = 0, Win = 1, Mac = 2, Ios = 3, Android = 4 }

public class Util {

    public static T CopyComponent<T> (T original, GameObject destination) where T : Component {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        for (int i = 0; i < fields.Length; i++)
            fields[i].SetValue(copy, fields[i].GetValue(original));

        return copy as T;
    }

    // ARRAYS
    /// <summary>
    /// Сортирует массив от большего к меньшему
    /// </summary>
    /// <typeparam name="Tobj">Тип элемента массива</typeparam>
    /// <param name="array">Массив</param>
    /// <param name="howToGetNum">Делегат, как получать числовое значение из элемента массива</param>
    /// <returns></returns>
    public static Tobj[] StairSort<Tobj> (ICollection<Tobj> array, Func<Tobj, float> howToGetNum) {
        List<Tobj> ret = new List<Tobj>(array.Count);
        ret.AddRange(array);

        float min = 0;
        int minI = 0;
        int offset = 0;

        float value = 0;

        while (offset < ret.Count) {
            min = howToGetNum(ret[offset]);
            minI = offset;
            for (int i = offset; i < ret.Count; i++) {
                value = howToGetNum(ret[i]);

                if (min > value) {
                    min = value;
                    minI = i;
                }
            }

            Swap(ret, offset, minI);
            offset++;
        }

        return ret.ToArray();
    }

    public static Tobj Min<Tobj> (ICollection<Tobj> array, Func<Tobj, float> howToGetFloat) {
        List<Tobj> ret = new List<Tobj>();
        ret.AddRange(array);

        float min = howToGetFloat(ret[0]);
        Tobj minObj = ret[0];
        for (int i = 1; i < ret.Count; i++) {
            float value = howToGetFloat(ret[i]);
            if (min >= value) {
                min = value;
                minObj = ret[i];
            }
        }

        return minObj;
    }

    /// <summary>
    /// Переводит из массива одного типа в массив другого типа
    /// </summary>
    /// <typeparam name="Tfrom"></typeparam>
    /// <typeparam name="Tto"></typeparam>
    /// <param name="arrFrom"></param>
    /// <param name="howToCast"></param>
    /// <returns></returns>
    public static Tto[] Cast<Tfrom, Tto> (ICollection<Tfrom> arrFrom, Func<Tfrom, Tto> howToCast) {
        List<Tfrom> from = new List<Tfrom>(arrFrom.Count);
        from.AddRange(arrFrom);
        Tto[] ret = new Tto[arrFrom.Count];

        for (int i = 0; i < ret.Length; i++) {
            ret[i] = howToCast(from[i]);
        }

        return ret;
    }

    /// <summary>
    /// Возвращает элемент из заданной коллекции, для которого удовлетворяет условие
    /// </summary>
    /// <typeparam name="Tobj"></typeparam>
    /// <param name="arr"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static Tobj GetSuitable<Tobj> (ICollection<Tobj> arr, Func<Tobj, bool> condition) {
        List<Tobj> list = new List<Tobj>(arr.Count);
        list.AddRange(arr);

        for (int i = 0; i < list.Count; i++) {
            if (condition(list[i]))
                return list[i];
        }

        return default(Tobj);
    }

    // VARIABLES
    public static void Swap<T> (List<T> arr, int a, int b) {
        T swap = arr[a];
        arr[a] = arr[b];
        arr[b] = swap;
    }
    public static void Swap<T> (ref T a, ref T b) {
        T swap = a;
        a = b;
        b = swap;
    }

    public delegate Tret Func<Targ, Tret> (Targ arg);

    // BEHAVIOR
    public static void SetTreeStatic (GameObject parent, bool isStatic) {
        Transform[] childs = parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++) {
            childs[i].gameObject.isStatic = isStatic;
        }
    }

    //MATH
    /// <summary>
    /// Как mod, но работает и с отрицательными значениями, возвращая им соответствующее значение с конца кольца
    /// </summary>
    /// <param name="number"></param>
    /// <param name="ringSize"></param>
    /// <returns></returns>
    public static int Ring (int number, int ringSize) {

        if (number >= 0) {
            return number % ringSize;
        }
        else {
            return number % ringSize + ringSize;
        }
    }
    /// <summary>
    /// Возвращает производную функции в данной точке
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static float Dx (Func<float, float> func, float x, float step = 0.0001f) {

        step = Mathf.Abs(step);
        float x0 = x - step;
        float x1 = x + step;

        float y0 = func(x0);
        float y1 = func(x1);

        return (y1 - y0) / (x1 - x0);
    }

    public static int GetResultMask (params int[] lms) {

        int rslt = 0;

        for (int i = 0; i < lms.Length; i++) {
            rslt |= ( 1 << lms[i]);
        }

        return rslt;
    }

    
    public static MyOS GetOS () {
        if (SystemInfo.operatingSystem.ToLower().IndexOf("windows") == 0)
            return MyOS.Win;

        else if (SystemInfo.operatingSystem.ToLower().IndexOf("mac") == 0)
            return MyOS.Mac;

        else if (SystemInfo.operatingSystem.ToLower().IndexOf("iphone") == 0)
            return MyOS.Ios;

        else if (SystemInfo.operatingSystem.ToLower().IndexOf("android") == 0)
            return MyOS.Android;

        return MyOS.None;
    }
    public static bool IsDesktop {
        get {
            if (GetOS() == MyOS.Mac || GetOS() == MyOS.Win)
                return true;

            return false;
        }
    }
    public static bool IsMobile {
        get {
            if (GetOS() == MyOS.Android || GetOS() == MyOS.Ios)
                return true;

            return false;
        }
    }
}
