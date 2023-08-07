using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    [SerializeField] private GameObject pfTnt;
    [SerializeField] private LayerMask physicalObjects;
    private Player player;
    internal List<Item> items = new List<Item>();

    private void Start() {
        player = GetComponent<Player>();
        InitializeInventory();
    }

    public void InitializeInventory() {
        for(int i = 0; i < 2; i++) {
            items.Add(new Medkit(200f));
        }
        for(int i = 0; i < 10; i++){
            items.Add(new TNT());
        }
    }

    /// <summary>
    /// Dodawanie przedmiotu do ekwipunku gracza
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Item item) {
        items.Add(item);
    }

    /// <summary>
    /// Usuwanie przedmiotu z ekwipunku gracza
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(Item item) {
        items.Remove(item);
    }

    public void UseMedkit() {
        Medkit medkit = items.Find(item => item is Medkit) as Medkit;
        if (medkit != null) {
            player.Heal(medkit.healingAmount);
            RemoveItem(medkit);
        } else {
            Debug.Log("No medkids in inventory!");
        }
    }

    public void UseTNT() {
        TNT tnt = items.Find(item => item is TNT) as TNT;
        if (tnt != null) {
            float requiredSpace = 0.6f;
            Collider[] colliders = Physics.OverlapSphere(player.transform.position, requiredSpace, physicalObjects);
            if (colliders.Length == 0) {
                // No colliders found, so we can spawn the TNT
                SpawnTNT(player.transform.position);
                RemoveItem(tnt);
            } else {
                Debug.Log("Cannot place TNT. There are physical objects nearby.");
            }
        } else {
            Debug.Log("No TNTs in inventory!");
        }
    }

    private void SpawnTNT(Vector3 position) {
        Instantiate(pfTnt, position, Quaternion.identity);
    }
}
