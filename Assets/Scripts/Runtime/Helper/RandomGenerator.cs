using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomGenerator
{
    public static float GetRandomSkewed(float min, float max)
    {
        return Mathf.Abs(Random.value - Random.value) * (1 + max - min) + min;
    }

    public static int GetRandomSkewed(int min, int max)
    {
        return Mathf.FloorToInt(Mathf.Abs(Random.value - Random.value) * (1 + max - min) + min);
    }

}
