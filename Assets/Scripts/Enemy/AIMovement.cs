using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTransform;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start() {
        playerTransform = FindObjectOfType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransform != null && agent.isActiveAndEnabled) {
            agent.destination = playerTransform.position;
        }
    }

}
