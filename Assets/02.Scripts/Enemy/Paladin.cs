using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Paladin : MonoBehaviour
{
    #region PaladinComponent
    private NavMeshAgent _agent;
    private EnemyStateMachine _state;
    private Animator _animator;
    private SphereCollider _detectCollider;
    #endregion

    #region PaladinValue
    private float _moveSpeed = 1f;
    private float _traceSpeed = 5f;
    private float _traceDistance = 10f;
    private bool isAction;
    private bool isDie = false;
    private GameObject _player;
    //private float _detectAngle = 45f;
    #endregion

    #region Property
    public NavMeshAgent Agent { get { return _agent; } }
    public EnemyStateMachine State { get { return _state; } }
    public Animator Anim { get { return _animator; } }
    public SphereCollider DetectCollider { get { return _detectCollider; } }
    public GameObject Player { get { return _player; }  set { _player = value; } }
    public float MoveSpeed { get { return _moveSpeed; } }
    public float TraceSpeed { get { return _traceSpeed;} }
    public float TraceDistance {  get { return _traceDistance;} }
    public bool IsAction { get { return isAction; }  set { isAction = value; } }
    public bool IsDie { get {  return isDie; } }
    //public float DetectAngle { get { return _detectAngle; } }
    #endregion

    private void Awake()
    {
        InitializePaladin();
        InitializeState();
    }

    private void InitializePaladin()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _detectCollider = GetComponent<SphereCollider>();
        _agent.speed = _moveSpeed;
    }

    private void InitializeState()
    {
        _state = gameObject.AddComponent<EnemyStateMachine>();
        _state.AddState(EnemyState.Idle, new Paladin_Idle(this));
        _state.AddState(EnemyState.Move, new Paladin_Move(this));
        _state.AddState(EnemyState.Trace, new Paladin_Trace(this));
        _state.AddState(EnemyState.Attack, new Paladin_Attack(this));
    }

    private IEnumerator Die()
    {
        isDie = true;

        _animator.SetTrigger("Die");

        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        yield return new WaitForSeconds(2.0f);

        gameObject.SetActive(false);
    }
}

public abstract class PaladinState : EnemyBase
{
    protected Paladin _paladin;

    public PaladinState(Paladin paladin)
    {
        _paladin = paladin;
    }

    protected IEnumerator ChangeDelay(Collider other)
    {
        yield return new WaitForSeconds(0.25f);

        if (_paladin.Player == null)
        {
            _paladin.Player = other.gameObject;
        }
        _paladin.DetectCollider.enabled = false;

        _paladin.State.ChangeState(EnemyState.Trace);
    }

    protected void AnimationMoveMent()
    {
        Vector3 currentVelocity = _paladin.Agent.velocity;

        float speed = currentVelocity.magnitude;

        _paladin.Anim.SetFloat("Move", speed);
    }

}

public class Paladin_Idle : PaladinState
{
    public Paladin_Idle(Paladin paladin) : base(paladin) { }

    public override void StateEnter()
    {
        _paladin.DetectCollider.enabled = true;
        _paladin.StartCoroutine(ChangeMove());
    }

    private IEnumerator ChangeMove()
    {
        float timer = 0f;

        while(timer > 3.0f)
        {
            yield return null;
            
            timer += Time.deltaTime;
        }

        _paladin.State.ChangeState(EnemyState.Move);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Vector3 direction = (other.transform.position - _paladin.transform.position).normalized;

            //float angle = Vector3.Angle(_paladin.transform.forward, direction);

            //if(angle <= _paladin.DetectAngle)
            //{
            //    _paladin.DetectCollider.enabled = false;

            //    _paladin.State.ChangeState(EnemyState.Trace);
            //}
            _paladin.StartCoroutine(ChangeDelay(other));
        }
    }
}

public class Paladin_Move : PaladinState
{
    public Paladin_Move(Paladin paladin) : base(paladin) { }
    
    private List<Transform> _wayPointList = new List<Transform>();

    private int _order = -1;

    public override void StateEnter()
    {
        FindWayPoint();
    }

    public override void StateUpdate()
    {
        if(_paladin.Agent.remainingDistance < 0.1f)
        {
            _paladin.State.ChangeState(EnemyState.Idle);
        }

        AnimationMoveMent();
    }

    private void FindWayPoint()
    {
        GameObject wayPoints = _paladin.transform.parent.gameObject;

        foreach(Transform trans in wayPoints.transform)
        {
            _wayPointList.Add(trans);
        }

        _order++;

        if (_order >= _wayPointList.Count)
            _order = 0;

        _paladin.Agent.SetDestination(_wayPointList[_order].position);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Vector3 direction = (other.transform.position - _paladin.transform.position).normalized;

            //float angle = Vector3.Angle(_paladin.transform.forward, direction);

            //if (angle <= _paladin.DetectAngle)
            //{
            //    _paladin.DetectCollider.enabled = false;

            //    _paladin.State.ChangeState(EnemyState.Trace);
            //}
            _paladin.StartCoroutine(ChangeDelay(other));
        }
    }
}

public class Paladin_Trace : PaladinState
{
    public Paladin_Trace(Paladin paladin) : base(paladin) { }

    public override void StateEnter()
    {
        _paladin.Agent.speed = _paladin.TraceSpeed;
        _paladin.Agent.stoppingDistance = 1.5f;
        _paladin.Agent.SetDestination(_paladin.Player.transform.position);
    }

    public override void StateUpdate()
    {
        TraceToPlayer();
    }

    public override void StateExit()
    {
        _paladin.Agent.speed = _paladin.MoveSpeed;
        _paladin.Agent.stoppingDistance = 0;
        _paladin.DetectCollider.enabled = true;
    }

    private void TraceToPlayer()
    {
        if(_paladin.Agent.remainingDistance <= _paladin.Agent.stoppingDistance)
        {
            _paladin.State.ChangeState(EnemyState.Attack);
        }
        else
            _paladin.Agent.SetDestination(_paladin.Player.transform.position);

        ReturnMove();

        AnimationMoveMent();
    }

    private void ReturnMove()
    {
        if(Vector3.Distance(_paladin.transform.position, _paladin.Player.transform.position) >= _paladin.TraceDistance)
        {
            _paladin.State.ChangeState(EnemyState.Move);
        }
    }
}

public class Paladin_Attack : PaladinState
{
    public Paladin_Attack(Paladin paladin) : base(paladin) { }

    private Vector3 _target;

    public override void StateEnter()
    {
        if (!_paladin.IsDie)
        {
            _paladin.Agent.enabled = false;
            _paladin.Anim.applyRootMotion = true;

            AttackToPlayer();
        }
    }

    public override void StateUpdate()
    {
        RotateToPlayer();
    }

    public override void StateExit()
    {
        _paladin.Agent.enabled = true;
        _paladin.Anim.applyRootMotion = false;
    }

    private void RotateToPlayer()
    {
        if (!_paladin.IsDie)
        {
            _target = _paladin.Player.transform.position;

            Vector3 rotateDirection = (_target - _paladin.transform.position).normalized;

            Quaternion rotation = Quaternion.LookRotation(rotateDirection);

            _paladin.transform.rotation = Quaternion.Slerp(_paladin.transform.rotation, rotation, 5.0f * Time.deltaTime);
        }
    }

    private void AttackToPlayer()
    {
        _paladin.Anim.SetTrigger("Attack");
    }
}