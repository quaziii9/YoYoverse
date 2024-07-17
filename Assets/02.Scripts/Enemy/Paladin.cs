using System.Collections;
using System.Collections.Generic;
using EnumTypes;
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
    private bool _isAction;
    private bool _isDie = false;
    private GameObject _player;

    private static readonly int DieParam = Animator.StringToHash("Die");
    
    #endregion

    #region Property
    
    public NavMeshAgent Agent => _agent;
    public EnemyStateMachine State => _state;
    public Animator Anim => _animator;
    public SphereCollider DetectCollider => _detectCollider;
    public GameObject Player { get { return _player; }  set { _player = value; } }
    public float MoveSpeed => _moveSpeed;
    public float TraceSpeed => _traceSpeed;
    public float TraceDistance => _traceDistance;
    public bool IsAction { get { return _isAction; }  set { _isAction = value; } }
    public bool IsDie => _isDie;

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
        _state.AddState(EnemyState.Idle, new PaladinIdle(this));
        _state.AddState(EnemyState.Move, new PaladinMove(this));
        _state.AddState(EnemyState.Trace, new PaladinTrace(this));
        _state.AddState(EnemyState.Attack, new PaladinAttack(this));
    }

    private IEnumerator Die()
    {
        _isDie = true;

        _animator.SetTrigger(DieParam);

        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        yield return new WaitForSeconds(2.0f);

        gameObject.SetActive(false);
    }
}

public abstract class PaladinState : EnemyBase
{
    protected Paladin _paladin;
    private static readonly int MoveParam = Animator.StringToHash("Move");

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

        _paladin.Anim.SetFloat(MoveParam, speed);
    }
}

public class PaladinIdle : PaladinState
{
    public PaladinIdle(Paladin paladin) : base(paladin) { }

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
            _paladin.StartCoroutine(ChangeDelay(other));
        }
    }
}

public class PaladinMove : PaladinState
{
    public PaladinMove(Paladin paladin) : base(paladin) { }
    
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
            _paladin.StartCoroutine(ChangeDelay(other));
        }
    }
}

public class PaladinTrace : PaladinState
{
    public PaladinTrace(Paladin paladin) : base(paladin) { }

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

public class PaladinAttack : PaladinState
{
    public PaladinAttack(Paladin paladin) : base(paladin) { }

    private Vector3 _target;
    private static readonly int AttackParam = Animator.StringToHash("Attack");

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
        _paladin.Anim.SetTrigger(AttackParam);
    }
}