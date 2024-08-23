using System;

namespace BraketsEngine;

public class Randomize
{
    public static float FloatInRange(float minInclude, float maxExclude)
    {
        return (float)new Random().NextDouble() * (maxExclude - minInclude) + minInclude;
    }

    public static int IntInRange(int minInclude, int maxExclude)
    {
        return new Random().Next(minInclude, maxExclude);
    }
}