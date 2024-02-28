using UnityEngine;

public class InputHandler
{
    public static ClientInputState GetInputState(ushort currentTick)
    {
        return new ClientInputState
        {
            Vertical = Input.GetAxisRaw("Vertical"),
            Horizontal = Input.GetAxisRaw("Horizontal"),
            Jump = Input.GetKey(KeyCode.Space),
            CurrentTick = currentTick
        };
    }
}