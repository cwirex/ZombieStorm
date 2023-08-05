using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Unity.VisualScripting;

public class AIMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTransform;
    private float nextUpdate = 0f;
    [SerializeField] float updateFrequency = 10f;
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start() {
        playerTransform = FindObjectOfType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.isActiveAndEnabled) {
            if(Time.time > nextUpdate) {
                agent.destination = playerTransform.position;

                float distanceFromPlayer = Vector3.Distance(transform.position, playerTransform.position);
                nextUpdate = Time.time + distanceFromPlayer / updateFrequency;
                agent.stoppingDistance = Mathf.Clamp((distanceFromPlayer-2) * 0.1f -1, 0f, 5f);
            }
        }

        
    }

}
