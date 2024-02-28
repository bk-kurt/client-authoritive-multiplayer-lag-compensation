using UnityEngine;
using Riptide;

public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController controller;

    [Header("Settings")]
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    [SerializeField] private bool serverPlayer;

    private ClientInputState _lastReceivedInputs = new();
    private bool _groundedPlayer;
    private Vector3 _playerVelocity;
    private float _vertical;
    private float _horizontal;
    private bool _jump;

    public void SetInput(float ver, float hor, bool jmp)
    {
        _vertical = ver;
        _horizontal = hor;
        _jump = jmp;

        HandleTick();
    }

    private void HandleClientInput(ClientInputState[] inputs)
    {
        if (!serverPlayer || inputs.Length == 0) return;

        if (inputs[^1].CurrentTick >= _lastReceivedInputs.CurrentTick)
        {

            int start = _lastReceivedInputs.CurrentTick > inputs[0].CurrentTick ? (_lastReceivedInputs.CurrentTick - inputs[0].CurrentTick) : 0;
            
            for (int i = start; i < inputs.Length - 1; i++)
            {
                SetInput(inputs[i].Vertical, inputs[i].Horizontal, inputs[i].Jump);
            }
            
            _lastReceivedInputs = inputs[^1];
            
            SendMovement();
        }
    }

    private void HandleTick()
    {
        _groundedPlayer = controller.isGrounded;
        if (_groundedPlayer && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(_horizontal, 0, _vertical);
        controller.Move(move * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
        
        if (_jump && _groundedPlayer)
        {
            _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        _playerVelocity.y += gravityValue;
        controller.Move(_playerVelocity);
    }

    private void SendMovement()
    {
        if (!serverPlayer) return;
        Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.Movement);
        message.AddUShort(_lastReceivedInputs.CurrentTick);
        message.AddVector3(transform.position);
        NetworkManager.Instance.Server.SendToAll(message);
    }

    [MessageHandler((ushort)ClientToServerId.Input)]
    private static void Input(ushort fromClientId, Message message)
    {
        byte inputsQuantity = message.GetByte();
        ClientInputState[] inputs = new ClientInputState[inputsQuantity];
        
        for (int i = 0; i < inputsQuantity; i++)
        {
            inputs[i] = new ClientInputState
            {
                Horizontal = message.GetFloat(),
                Vertical = message.GetFloat(),
                Jump = message.GetBool(),
                CurrentTick = message.GetUShort()
            };
        }
        
        PlayerManager.Instance.serverPlayerMovement.HandleClientInput(inputs);
    }
}