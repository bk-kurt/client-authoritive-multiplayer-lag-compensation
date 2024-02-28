public class SimulationStateCache
{
    private readonly SimulationState[] _cache;
    private readonly int _size;
    private int _startIndex;
    private int _count;

    public SimulationStateCache(int cacheSize)
    {
        _size = cacheSize;
        _cache = new SimulationState[_size];
        _startIndex = 0;
        _count = 0;
    }

    public void StoreState(ushort tick, SimulationState state)
    {
        int index = (_startIndex + _count) % _size;
        _cache[index] = state;
        
        if (_count == _size)
        {
            _startIndex = (_startIndex + 1) % _size;
        }
        else
        {
            _count++;
        }
    }

    public SimulationState GetState(ushort tick)
    {
        if (_count == 0)
        {
            return null;
        }
        
        int index = (_startIndex + (tick % _count)) % _size;
        return _cache[index];
    }
}