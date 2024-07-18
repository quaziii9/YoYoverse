using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public GameObject meleeEnemyPrefab; // 근거리 적 오브젝트 프리팹
    public GameObject sniperEnemyPrefab; // 저격수 적 오브젝트 프리팹

    public List<Transform> meleeSpawnPoints = new List<Transform>(); // 근거리 적 스폰 포인트 리스트
    public List<Transform> sniperSpawnPoints = new List<Transform>(); // 저격수 적 스폰 포인트 리스트

    private List<GameObject> _meleeEnemies = new List<GameObject>(); // 근거리 적 오브젝트 리스트
    private List<GameObject> _sniperEnemies = new List<GameObject>(); // 저격수 적 오브젝트 리스트

    // 초기 적 오브젝트 생성
    private void Start()
    {
        SpawnEnemies();
    }

    // 적 오브젝트 생성
    private void SpawnEnemies()
    {
        foreach (var spawnPoint in meleeSpawnPoints)
        {
            GameObject enemy = Instantiate(meleeEnemyPrefab, spawnPoint.position, Quaternion.identity);
            _meleeEnemies.Add(enemy);
        }

        foreach (var spawnPoint in sniperSpawnPoints)
        {
            GameObject enemy = Instantiate(sniperEnemyPrefab, spawnPoint.position, Quaternion.identity);
            _sniperEnemies.Add(enemy);
        }
    }

    // 적 오브젝트 초기화
    public void ResetEnemies()
    {
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
        SpawnEnemies();
    }
}