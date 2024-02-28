using System;

public static class ArrayUtility
{
    public static T[] Resize<T>(T[] array, int newSize)
    {
        T[] newArray = new T[newSize];
        Array.Copy(array, newArray, Math.Min(array.Length, newSize));
        return newArray;
    }
}