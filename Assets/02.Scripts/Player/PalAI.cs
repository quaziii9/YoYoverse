//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//using UnityEngine.Playables;

//public enum PalStates
//{
//    Decimal
//    Idle = 0,
//    Move = 1,
//    Action = 2
//}

//public class PalAI : MonoBehaviour
//{
//    public Pal pal;
//    public int id;

//    public PalStates palState;
//    public GameObject target;
//    public Building targetBulding;

//    public bool thisPalCanSleep = true;
//    public bool thisPalCanbuild = true;
//    public bool thisPalCanFire = false;
//    public bool thisPalCanSeed = false;
//    public bool thisPalCanWater = false;
//    public bool thisPalCanHarvest = false;
//    public bool thisPalCanCutting = false;
//    public bool thisPalCanMining = false;
//    public bool thisPalCanProduce = false;

//    Coroutine coroutine;
//    Coroutine playing;

//    public bool thisPalPlaying = false;
//    bool start = false;

//    #region Astar Pathfinding
//    Astar astar;
//    private List<Node> path;
//    public int pathInitIndex, pathFinalIndex;
//    public float targetx;
//    public float targety;
//    #endregion

//    private void OnEnable()
//    {
//        if (start)
//            StartCoroutine(Search());
//    }

//    private void Awake()
//    {
//        astar = GetComponent<Astar>();
//    }
//    private void OnDisable()
//    {
//        StopAllCoroutines();
//    }
//    void Start()
//    {
//        start = true;
//        transform.GetChild(0).gameObject.SetActive(false);

//        pal = PalDatabase.Instance.GetPal(id);
//        StartCoroutine(Search());
//    }

//    void OnGo()
//    {
//        astar.SetDestination(target); // 타켓 목적지 설정
//        path = astar.AStar(); // 길찾기 연산        
//        if (path.Count == 0)
//        {
//            return;
//        }
//        palState = PalStates.Move;
//        pathInitIndex = 0; // 초기화
//        pathFinalIndex = path.Count;
//        if (thisPalPlaying) { thisPalPlaying = false; StopCoroutine(playing); }
//    }

//    Start is called before the first frame update
//    IEnumerator Search()
//    {
//        while (true)
//        {
//            if (palState == PalStates.Idle)
//            {
//                if (thisPalCanSleep && PalManager.Instance.sleeping.Count > 0)
//                {
//                    int count = PalManager.Instance.sleeping.Count;
//                    for (int i = 0; i < count; i++)
//                    {
//                        targetBulding = PalManager.Instance.sleeping[i];
//                        if (targetBulding.workingPal == null)
//                        {
//                            targetBulding.workingPal = this;
//                            target = targetBulding.gameObject;
//                            OnGo();
//                            break;
//                        }
//                    }
//                }

//                if (thisPalCanFire && PalManager.Instance.cooking.Count > 0)
//                {
//                    int count = PalManager.Instance.cooking.Count;
//                    for (int i = 0; i < count; i++)
//                    {
//                        targetBulding = PalManager.Instance.cooking[i];
//                        if (targetBulding.workingPal == null)
//                        {
//                            targetBulding.workingPal = this;
//                            target = targetBulding.gameObject;
//                            OnGo();
//                            break;
//                        }
//                    }
//                }

//                if (thisPalCanSeed && PalManager.Instance.seeding.Count > 0)
//                {
//                    int count = PalManager.Instance.seeding.Count;
//                    for (int i = 0; i < count; i++)
//                    {
//                        targetBulding = PalManager.Instance.seeding[i];
//                        if (targetBulding.workingPal == null)
//                        {
//                            targetBulding.workingPal = this;
//                            target = targetBulding.gameObject;
//                            OnGo();
//                            break;
//                        }
//                    }
//                }

//                if (thisPalCanWater && PalManager.Instance.watering.Count > 0)
//                {
//                    int count = PalManager.Instance.watering.Count;
//                    for (int i = 0; i < count; i++)
//                    {
//                        targetBulding = PalManager.Instance.watering[i];
//                        if (targetBulding.workingPal == null)
//                        {
//                            targetBulding.workingPal = this;
//                            target = targetBulding.gameObject;
//                            OnGo();
//                            break;
//                        }
//                    }
//                }

//                if (thisPalCanHarvest && PalManager.Instance.harvesting.Count > 0)
//                {
//                    int count = PalManager.Instance.harvesting.Count;
//                    for (int i = 0; i < count; i++)
//                    {
//                        targetBulding = PalManager.Instance.harvesting[i];
//                        if (targetBulding.workingPal == null)
//                        {
//                            targetBulding.workingPal = this;
//                            target = targetBulding.gameObject;
//                            OnGo();
//                            break;
//                        }
//                    }
//                }
//                if (thisPalCanProduce && PalManager.Instance.producing.Count > 0)
//                {
//                    int count = PalManager.Instance.producing.Count;
//                    for (int i = 0; i < count; i++)
//                    {
//                        targetBulding = PalManager.Instance.producing[i];
//                        if (targetBulding.workingPal == null)
//                        {
//                            targetBulding.workingPal = this;
//                            target = targetBulding.gameObject;
//                            OnGo();
//                            break;
//                        }
//                    }
//                }

//                if (thisPalCanMining && PalManager.Instance.mining.Count > 0)
//                {
//                    int count = PalManager.Instance.mining.Count;
//                    for (int i = 0; i < count; i++)
//                    {
//                        targetBulding = PalManager.Instance.mining[i];
//                        if (targetBulding.workingPal == null)
//                        {
//                            targetBulding.workingPal = this;
//                            target = targetBulding.gameObject;
//                            OnGo();
//                            break;
//                        }
//                    }
//                }

//                if (thisPalCanCutting && PalManager.Instance.cutting.Count > 0)
//                {
//                    int count = PalManager.Instance.cutting.Count;
//                    for (int i = 0; i < count; i++)
//                    {
//                        targetBulding = PalManager.Instance.cutting[i];
//                        if (targetBulding.workingPal == null)
//                        {
//                            targetBulding.workingPal = this;
//                            target = targetBulding.gameObject;
//                            OnGo();
//                            break;
//                        }
//                    }
//                }

//                if (thisPalCanbuild == true && PalManager.Instance.buildings.Count > 0 && palState == PalStates.Idle)
//                {
//                    targetBulding = PalManager.Instance.buildings[0];
//                    target = targetBulding.gameObject;
//                    if (thisPalPlaying)
//                    {
//                        StopCoroutine(playing);
//                        thisPalPlaying = false;
//                    }
//                    OnGo();
//                }

//                if (palState == PalStates.Idle)
//                {
//                    if (Random.Range(0, 100) > 80)
//                    {
//                        if (thisPalPlaying) StopCoroutine(playing);
//                        float x = Random.Range(-1, 1);
//                        x = transform.position.x + x;
//                        if (x < PalBoxManager.Instance.palBoxBuilding[0].transform.position.x - 10) x = PalBoxManager.Instance.palBoxBuilding[0].transform.position.x - 10;
//                        else if (x > PalBoxManager.Instance.palBoxBuilding[0].transform.position.x + 10) x = PalBoxManager.Instance.palBoxBuilding[0].transform.position.x + 10;
//                        float y = Random.Range(-1, 1);
//                        y = transform.position.y + y;
//                        if (y < PalBoxManager.Instance.palBoxBuilding[0].transform.position.y - 10) y = PalBoxManager.Instance.palBoxBuilding[0].transform.position.y - 10;
//                        else if (y > PalBoxManager.Instance.palBoxBuilding[0].transform.position.y + 10) y = PalBoxManager.Instance.palBoxBuilding[0].transform.position.y + 10;
//                        Vector2 newPos = new Vector3(x, y);
//                        playing = StartCoroutine(PlayAlone(newPos));
//                    }

//                }

//            }
//            else if (palState == PalStates.Move)
//            {
//                if (targetBulding.buildingStatement == BuildingStatement.Built) //가기 전에 건물 완공되면 업무취소
//                {
//                    palState = PalStates.Idle;
//                    target = null;
//                }

//            }
//            yield return new WaitForSeconds(1f);
//        }

//    }

//    IEnumerator PlayAlone(Vector2 pos)
//    {
//        while (true)
//        {
//            thisPalPlaying = true;
//            PalWorking(pos);
//            yield return new WaitForSeconds(0.02f);
//        }

//    }

//    private void FixedUpdate()
//    {
//        switch (palState)
//        {
//            case PalStates.Idle:
//                break;
//            case PalStates.Move:
//                PalMove();
//                break;
//            case PalStates.Action:
//                PalWork();
//                break;
//        }
//    }


//    void PalWork()
//    {
//        switch (targetBulding.buildingStatement)
//        {
//            case BuildingStatement.Built:
//                palState = PalStates.Idle;
//                break;
//            case BuildingStatement.isBuilding:
//                targetBulding.Build(0.1f);
//                if (!targetBulding.gameObject.activeSelf)
//                {
//                    palState = PalStates.Idle;
//                }
//                break;
//            case BuildingStatement.Working:
//                targetBulding.Work(0.01f);
//                break;
//            case BuildingStatement.Done:
//                palState = PalStates.Idle;
//                break;
//        }
//    }

//    public void PalWorking(Vector2 pos)
//    {
//        transform.position = Vector3.MoveTowards(transform.position, pos, 0.03f);
//    }

//    public void PalMove() // Astar pathFinding
//    {
//        targetx = path[pathInitIndex].x;
//        targety = path[pathInitIndex].y;

//        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetx, targety), 0.03f);
//        if (Mathf.Abs(transform.position.x - targetx) < 0.1f && Mathf.Abs(transform.position.y - targety) < 0.1f)
//        {
//            astar.SetGizmoIndex(pathInitIndex);
//            pathInitIndex++;
//            if (pathInitIndex == pathFinalIndex)
//            {
//                palState = PalStates.Idle;
//            }
//        }
//    }

//    private void OnCollisionEnter2D(Collision2D collision)
//    {

//        if (collision.gameObject.CompareTag("Furniture") || collision.gameObject.CompareTag("PalBed"))
//        {
//            int index = collision.gameObject.GetComponent<Building>().index;
//            if (targetBulding.index == index)
//            {
//                palState = PalStates.Action;
//            }
//            else
//            {
//                OnGo();
//            }
//        }
//        else
//        {
//            return;
//        }
//    }
//}