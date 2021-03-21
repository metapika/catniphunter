using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints = new List<Transform>();
    public GameObject cargoPrefab;
    public float timeBetweenPlatforms = .5f;
    void Start()
    {
        SpawnACargo(spawnPoints[GetRandomNumber()]);
    }
    private void SpawnACargo(Transform point) 
    {
        Instantiate(cargoPrefab, point.position, point.rotation);

        StartCoroutine(SpawnNext());
    }
    private IEnumerator SpawnNext()
    {
        yield return new WaitForSeconds(timeBetweenPlatforms);

        SpawnACargo(spawnPoints[GetRandomNumber()]);
    }
    private int GetRandomNumber() 
    {
        return Random.Range(0, 6);
    }
}
