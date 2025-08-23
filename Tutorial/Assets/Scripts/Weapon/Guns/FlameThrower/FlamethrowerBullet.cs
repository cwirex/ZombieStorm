using Assets.Scripts.Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

/// <summary>
/// Class of a continuous Flames (Activated by Flamethrower)
/// </summary>
public class FlamethrowerBullet : MonoBehaviour
{
    [SerializeField] LayerMask obstaclesLayer;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    private Transform playerTF;
    private WeaponStats stats;
    private float nextTick = 0f;
    [SerializeField]  private float knockbackR = 1f;

    private void Start() {
        playerTF = FindObjectOfType<Player>()?.transform;
    }

    private void Update() {
        if (Time.time > nextTick) {
            nextTick = Time.time + 1 / stats.FireRate;
            enemiesInRange.RemoveAll(item => item == null); // remove dead enemies
            foreach (Enemy enemy in enemiesInRange) {
                // check if there is no wall between Enemy and Player
                if (!Physics.Raycast(enemy.transform.position, playerTF.position, Vector3.Distance(enemy.transform.position, playerTF.position) + 0.1f, obstaclesLayer)) {
                    enemy.TakeDamage(stats.Damage / stats.FireRate, transform.forward / knockbackR);
                }
            }
        }
    }

    internal void SetStats(WeaponStats weaponStats) {
        stats = weaponStats;
    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out Enemy enemy)) {
            if (!enemiesInRange.Contains(enemy)) {
                enemiesInRange.Add(enemy);
            }
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.TryGetComponent(out Enemy enemy)) {
            if (enemiesInRange.Contains(enemy)) {
                enemiesInRange.Remove(enemy);
            }
        }
    }
}
