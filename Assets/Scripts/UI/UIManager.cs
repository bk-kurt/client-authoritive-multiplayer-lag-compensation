using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Text pingDisplay;

    void Update()
    {
        pingDisplay.text=($"Ping: {NetworkManager.Instance.Client.RTT.ToString()}");
    }
}         