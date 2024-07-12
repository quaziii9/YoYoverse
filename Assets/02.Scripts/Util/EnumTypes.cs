using UnityEngine;

namespace EnumTypes
{
    public enum LookDirection { Up, Down, Left, Right }
    public enum PlayerState { Stand, Move, Run, Hold, Cook }

    public enum Layers
    {
        Default,
        TransparentFX,
        IgnoreRaycast,
        Reserved1,
        Water,
        UI,
        Reserved2,
        Reserved3,
        Player,
        Enemy,
    }

    public enum PlayerEvents
    {
        PlayerDead,
    }

    public enum YoYoEvents
    {
        YoYoAttached,
    }

    public enum UIEvents
    {
    }

    public enum DataEvents
    {
       
    }

    public class EnumTypes : MonoBehaviour { }
}