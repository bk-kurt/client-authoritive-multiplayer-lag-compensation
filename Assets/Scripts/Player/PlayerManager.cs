using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    [SerializeField] public SimplePlayerMovement serverPlayerMovement;
    [SerializeField] public SimplePlayerMovement clientServerMovement;
}
