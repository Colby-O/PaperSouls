using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Helpers
{
    public static class RandomGenerator
    {
        /// <summary>
        /// Returns a random number between [min, max] biased towards values closer to min. 
        /// </summary>
        public static int GetRandomSkewed(int min, int max)
        {
            return Mathf.FloorToInt(Mathf.Abs(Random.value - Random.value) * (1 + max - min) + min);
        }

    }
}
