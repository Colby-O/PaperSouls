using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Helpers
{
    internal static class RandomGenerator
    {
        /// <summary>
        /// Returns a random number between [min, max] biased towards values closer to min. 
        /// </summary>
        public static int GetRandomSkewed(int min, int max)
        {
            return Mathf.FloorToInt(Mathf.Abs(Random.value - Random.value) * (1 + max - min) + min);
        }

        /// <summary>
        /// Samples random numbers from a Gaussian distribution using the 
        /// Box-Muller Transform.
        /// </summary>
        public static float DrawSampleFromNormal(float mean = 0, float std = 1)
        {
            float u1 = 1.0f - Random.value;
            float u2 = 1.0f - Random.value;
            float r = Mathf.Sqrt(-2.0f * Mathf.Log(u1));
            float theta = 2.0f * Mathf.PI * u2;
            float randStandardNormal = r * Mathf.Sin(theta);
            float randNormal = mean + std * randStandardNormal;
            return randNormal;
        }

        /// <summary>
        /// Draws N samples random numbers from a Gaussian distribution using the 
        /// Box-Muller Transform.
        /// </summary>
        public static List<float> DrawNSampleFromNormal(int num, float mean = 0, float std = 1)
        {
            List<float> randList = new();

            for (int i = 0; i < num; i++) randList.Add(DrawSampleFromNormal(mean, std));

            return randList;
        }

        /// <summary>
        /// Samples random numbers from a skewed Gaussian distribution using a
        /// modified version of the Box-Muller Transform.
        /// </summary>
        public static float DrawSampleFromSkewedNormal(float mean = 0, float std = 1, float skewness = 0)
        {
            if (skewness == 0) return DrawSampleFromNormal(mean, std);
            float r1 = 1.0f - Random.value;
            float r2 = 1.0f - Random.value;
            float r = Mathf.Sqrt(-2.0f * Mathf.Log(r1));
            float theta = 2.0f * Mathf.PI * r2;
            float u0 = r * Mathf.Cos(theta);
            float v = r * Mathf.Sin(theta);

            float delta = skewness / Mathf.Sqrt(1 + skewness * skewness);
            float u1 = delta * u0 + Mathf.Sqrt(1 - delta * delta) * v;
            float z = u0 >= 0 ? u1 : -u1;
            return mean + std * z;
        }

        /// <summary>
        /// Draws N samples random numbers from a skewed Gaussian distribution using a
        /// modified version of the Box-Muller Transform.
        /// </summary>
        public static List<float> DrawNSampleFromSkewedNormal(int num, float mean = 0, float std = 1, float skewness = 0)
        {
            List<float> randList = new();

            for (int i = 0; i < num; i++) randList.Add(DrawSampleFromSkewedNormal(mean, std, skewness));

            return randList;
        }
    }
}
