using System;
using System.Linq;

public static class InputCacheSizeAdjuster
{
    private const float UpperThreshold = 0.8f;
    private const float LowerThreshold = 0.2f;

    public static void AdjustCacheSize(ref SimulationState[] simulationStateCache,
        ref ClientInputState[] inputStateCache, int stateCacheSize)
    {
        float utilization = (float)simulationStateCache.Count(entry => entry != null) / simulationStateCache.Length;

        if (utilization is >= LowerThreshold and <= UpperThreshold)
        {
            return;
        }

        int newSize = (int)Math.Clamp(simulationStateCache.Length * (utilization > UpperThreshold ? 1.2 : 1.0), stateCacheSize, double.MaxValue);
        ResizeCache(ref simulationStateCache, ref inputStateCache, newSize);
    }

    private static void ResizeCache(ref SimulationState[] simulationStateCache,
        ref ClientInputState[] inputStateCache, int newSize)
    {
        if (newSize == simulationStateCache.Length)
        {
            return;
        }

        simulationStateCache = ArrayUtility.Resize(simulationStateCache, newSize);
        inputStateCache = ArrayUtility.Resize(inputStateCache, newSize);
    }
}