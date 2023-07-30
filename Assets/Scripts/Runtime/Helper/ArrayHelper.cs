using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PaperSouls.Runtime.Helpers
{
    internal sealed class ArrayHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static T[] Concat<T>(T[] x, T[] y)
        {
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");
            int oldLen = x.Length;
            Array.Resize<T>(ref x, x.Length + y.Length);
            Array.Copy(y, 0, x, oldLen, y.Length);
            return x;
        }

        /// <summary>
        /// Fill an array with a single value
        /// </summary>
        public static void Fill<T>(ref T[] arr, T val)
        {
            for (int i = 0; i < arr.Length; i++) arr[i] = val;
        }

        /// <summary>
        /// Fill a 2D array with a single value
        /// </summary>
        public static void Fill<T>(ref T[,] arr, T val)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    arr[i, j] = val;
                }
            }
        }

        /// <summary>
        /// Creates an array and fills it with a default value 
        /// </summary>
        public static T[] CreateAndFill<T>(int size, T val)
        {
            T[] arr = new T[size];
            Fill(ref arr, val);
            return arr;
        }

        /// <summary>
        /// Creates a 2D array and fills it with a default value 
        /// </summary>
        public static T[,] CreateAndFill<T>(int sizeX, int sizeY, T val)
        {
            T[,] arr = new T[sizeX, sizeY];
            Fill(ref arr, val);
            return arr;
        }

        /// <summary>
        /// Creates a 2D array and fills it with a default value 
        /// </summary>
        public static T[,] Count<T>(int sizeX, int sizeY, T val)
        {
            T[,] arr = new T[sizeX, sizeY];
            Fill(ref arr, val);
            return arr;
        }
    }
}
