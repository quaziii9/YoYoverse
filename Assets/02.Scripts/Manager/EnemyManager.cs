using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [PropertySpace(5f, 0f)]public GameObject meleeEnemyPrefab; // 근거리 적 오브젝트 프리팹
    public GameObject sniperEnemyLeftPrefab; // 왼쪽 회전 저격수 적 오브젝트 프리팹
    [PropertySpace(0f, 10f)] public GameObject sniperEnemyRightPrefab; // 오른쪽 회전 저격수 적 오브젝트 프리팹

    public List<Transform> meleeSpawnPoints = new List<Transform>(); // 근거리 적 스폰 포인트 리스트
    public List<Transform> leftSniperSpawnPoints = new List<Transform>(); // 왼쪽 회전 저격수 적 스폰 포인트 리스트
    public List<Transform> rightSniperSpawnPoints = new List<Transform>(); // 오른쪽 회전 저격수 적 스폰 포인트 리스트

    private List<GameObject> _meleeEnemies = new List<GameObject>(); // 근거리 적 오브젝트 리스트
    private List<GameObject> _sniperEnemies = new List<GameObject>(); // 저격수 적 오브젝트 리스트

    protected override void Awake()
    {
        base.Awake();
        
        EventManager<PlayerEvents>.StartListening(PlayerEvents.PlayerSpawn, SpawnEnemies);
    }

    private void OnDestroy()
    {
        EventManager<PlayerEvents>.StopListening(PlayerEvents.PlayerSpawn, SpawnEnemies);
    }

    // 적 오브젝트 생성
    private void SpawnEnemies()
    {
        foreach (var spawnPoint in meleeSpawnPoints)
        {
            GameObject enemy = Instantiate(meleeEnemyPrefab, spawnPoint.position, Quaternion.identity);
            _meleeEnemies.Add(enemy);
        }

        foreach (var spawnPoint in leftSniperSpawnPoints)
        {
            GameObject enemy = Instantiate(sniperEnemyLeftPrefab, spawnPoint.position, Quaternion.Euler(new Vector3 (0,180,0)));
            _sniperEnemies.Add(enemy);
        }
        
        foreach (var spawnPoint in rightSniperSpawnPoints)
        {
            GameObject enemy = Instantiate(sniperEnemyRightPrefab, spawnPoint.position, Quaternion.Euler(new Vector3(0, 180, 0)));
            _sniperEnemies.Add(enemy);
        }
    }

    // 적 오브젝트 초기화
    public void ResetEnemies()
    {
        DebugLogger.Log("적 오브젝트 초기화");
        // 기존 근거리 적 오브젝트 삭제
        foreach (var enemy in _meleeEnemies)
        {
            Destroy(enemy);
        }
        _meleeEnemies.Clear();

        // 기존 저격수 적 오브젝트 삭제
        foreach (var enemy in _sniperEnemies)
        {
            Destroy(enemy);
        }
        _sniperEnemies.Clear();

        // 초기 위치에 적 오브젝트 재생성
        // SpawnEnemies();
    }
}