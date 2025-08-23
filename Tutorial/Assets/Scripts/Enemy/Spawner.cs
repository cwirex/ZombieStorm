using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private int spawnCount = 3;
    [SerializeField] private float spawnInterval = 0.6f;
    [SerializeField] GameObject pfEnemy;
    

    private int spawnedEnemies = 0;

    private void Start() {
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine() {
        // Calculate the initial delay based on the index of this spawner
        float initialDelay = spawnInterval * transform.GetSiblingIndex();

        // Wait for the initial delay before starting the spawning
        yield return new WaitForSeconds(initialDelay);

        while (spawnedEnemies < spawnCount) {
            Instantiate(pfEnemy, transform.position, Quaternion.identity);
            spawnedEnemies++;

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
