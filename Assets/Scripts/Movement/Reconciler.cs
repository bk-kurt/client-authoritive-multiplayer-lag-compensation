using UnityEngine;

public class Reconciler
{
    public void Reconciliate(SimulationState serverSimulationState, SimulationState[] simulationStateCache,
        int lastCorrectedFrame, ushort cspTick, Transform transform)
    {
        if (serverSimulationState.CurrentTick <= lastCorrectedFrame) return;

        int cacheIndex = serverSimulationState.CurrentTick % simulationStateCache.Length;
        float posDif = Vector3.Distance(simulationStateCache[cacheIndex].Position, serverSimulationState.Position);

        if (posDif > 0.001f)
        {
            transform.position = serverSimulationState.Position;
            ushort rewindTick = serverSimulationState.CurrentTick;

            while (rewindTick < cspTick)
            {
                int rewindCacheIndex = rewindTick % simulationStateCache.Length;
                if (simulationStateCache[rewindCacheIndex] == null)
                {
                    rewindTick++;
                    continue;
                }

                simulationStateCache[rewindCacheIndex] = GetCurrentSimulationState(rewindTick, transform);
                simulationStateCache[rewindCacheIndex].CurrentTick = rewindTick;

                rewindTick++;
            }
        }
    }

    private SimulationState GetCurrentSimulationState(ushort cspTick, Transform transform)
    {
        return new SimulationState
        {
            Position = transform.position,
            CurrentTick = cspTick
        };
    }
}