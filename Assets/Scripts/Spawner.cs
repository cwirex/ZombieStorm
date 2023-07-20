using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private int m_SpawnCount = 4;
    [SerializeField] GameObject pfEnemy;

    void Start()
    {
        for(int i = 0; i < m_SpawnCount; i++) {
            Instantiate(pfEnemy, transform);
        }
    }
}
