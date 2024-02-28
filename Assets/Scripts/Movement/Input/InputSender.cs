using System;
using Riptide;
using UnityEngine;

public class InputSender
{
    public void SendInput(ClientInputState currentInput, ushort cspTick, ClientInputState lastSentInputState)
    {
        if (!HasInputChanged(currentInput, lastSentInputState)) return;

        if (!NetworkManager.Instance.Client.IsConnected)
        {
            // todo Attempt to reconnect or notify the user
            return;
        }

        try
        {
            Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.Input);
            message.AddUShort(cspTick);
            message.AddFloat(currentInput.Horizontal);
            message.AddFloat(currentInput.Vertical);
            message.AddBool(currentInput.Jump);
            NetworkManager.Instance.Client.Send(message);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to send message: {ex.Message}");
            // todo Handle failure (e.g., retry logic, error notification)
        }
    }

    private bool HasInputChanged(ClientInputState currentInput, ClientInputState lastSentInputState)
    {
        return lastSentInputState == null ||
               Mathf.Abs(currentInput.Horizontal - lastSentInputState.Horizontal) > 0.01f ||
               Mathf.Abs(currentInput.Vertical - lastSentInputState.Vertical) > 0.01f ||
               currentInput.Jump != lastSentInputState.Jump;
    }
}