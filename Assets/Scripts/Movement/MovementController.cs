using UnityEngine;

public class MovementController : MonoBehaviour
{
    private InputProcessor _inputProcessor;
    private InputSender _inputSender;
    private Reconciler _reconciler;
    private SimplePlayerMovement _movement;
    
    private SimulationState[] _simulationStateCache = new SimulationState[StateCacheSize];
    private ClientInputState[] _inputStateCache = new ClientInputState[StateCacheSize];
    private ClientInputState _lastSentInputState;
    private readonly SimulationState _serverSimulationState = new();
    
    private ushort _cspTick;
    private static readonly int StateCacheSize = NetworkManager.SimulationStateCacheSize;
    
    private int _lastCorrectedFrame;
    private float _timer;
    private float _minTimeBetweenTicks;

    


    private void Awake()
    {
        _movement = GetComponent<SimplePlayerMovement>();
    }

    private void Start()
    {
        _minTimeBetweenTicks = 1f / NetworkManager.Instance.serverTickRate;
        _inputProcessor = new InputProcessor();
        _reconciler = new Reconciler();
        _inputSender = new InputSender();
    }


    private float _cacheCheckTimer;

    private void Update()
    {
        _timer += Time.deltaTime;

        while (_timer >= _minTimeBetweenTicks)
        {
            _timer -= _minTimeBetweenTicks;
            ProcessInput();
        }

        if (_serverSimulationState.CurrentTick > 0)     
        {
            _reconciler.Reconciliate(_serverSimulationState, _simulationStateCache, _lastCorrectedFrame, _cspTick,
                transform);
        }
        
        _lastCorrectedFrame = _serverSimulationState.CurrentTick;
        
        if (_lastSentInputState != null)
        {
            _inputSender.SendInput(_lastSentInputState, _cspTick, _lastSentInputState);
        }

        AdjustCacheSize();
    }

    private void ProcessInput()
    {
        _lastSentInputState =
            _inputProcessor.ProcessInput(_cspTick, _inputStateCache, _simulationStateCache, _movement, transform);
        _cspTick++;
    }

    private void AdjustCacheSize()
    {
        InputCacheSizeAdjuster.AdjustCacheSize(ref _simulationStateCache, ref _inputStateCache, StateCacheSize);
    }
    
    private void OnDrawGizmos()
    {
        if (_serverSimulationState != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_serverSimulationState.Position, 1f);
        }
    }
}