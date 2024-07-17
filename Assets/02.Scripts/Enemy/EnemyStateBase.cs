//using Unity.VisualScripting;

//public interface IState
//{
//    void Enter();
//    void ExecuteOnUpdate();
//    void Exit();
//}

//public abstract class StateBase : IState
//{
//    protected StateBase(PlayerController player, AttackController attackController)
//    {
//        Player = player;
//        AttackController = attackController;
//    }

//    protected StateBase(PlayerController player)
//    {
//        Player = player;
//        AttackController = player.AttackController;
//        StatController = player.StatController;
//    }

//    public virtual void Enter()
//    {
//        if (Player == null)
//        {
//            return;
//        }
//        Player.BindInputCallback(true, OnInputCallback);
//        isTransitioning = true;
//        TransitionDelay().Forget();
//    }

//    public virtual void Exit()
//    {
//        if (Player == null)
//        {
//            return;
//        }
//        Player.BindInputCallback(false, OnInputCallback);
//    }

//    public virtual void ExecuteOnUpdate() { }

//    public virtual void OnInputCallback(InputAction.CallbackContext context)
//    {
//        if (isTransitioning) return; // 상태 전환 중에는 입력 무시
//    }

//    public abstract bool IsTransitioning { get; }

//    private async UniTask TransitionDelay()
//    {
//        await UniTask.Delay(100); // 상태 전환 후 0.1초 딜레이
//        isTransitioning = false;
//    }
//}