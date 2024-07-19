using EnumTypes;
using EventLibrary;
using UnityEngine;

public class CompleteGame : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            EventManager<GameEvents>.TriggerEvent(GameEvents.GameComplete);
            DebugLogger.Log("게임 클리어");
        }
    }
}
