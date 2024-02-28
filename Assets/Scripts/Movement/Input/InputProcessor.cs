using UnityEngine;

public class InputProcessor
{
    public ClientInputState ProcessInput(ushort cspTick, ClientInputState[] inputStateCache,
        SimulationState[] simulationStateCache, SimplePlayerMovement movement, Transform transform)
    {
        int cacheIndex = cspTick % inputStateCache.Length;
        inputStateCache[cacheIndex] = InputHandler.GetInputState(cspTick);
        simulationStateCache[cacheIndex] = GetCurrentSimulationState(cspTick, transform);

        movement.SetInput(inputStateCache[cacheIndex].Vertical, inputStateCache[cacheIndex].Horizontal,
            inputStateCache[cacheIndex].Jump);

        return inputStateCache[cacheIndex];
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