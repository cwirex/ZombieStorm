using Assets.Scripts.Player;
using Assets.Scripts.PlayerScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds Player Items and controlls their usage.
/// </summary>
public class PlayerInventory : MonoBehaviour {

    [SerializeField] private GameObject pfTnt;
    [SerializeField] private LayerMask physicalObjects;

    private Player player;
    private List<Item> items = new List<Item>();
    private UIController uiController;


    private void Start() {
        player = GetComponent<Player>();
        uiController = FindObjectOfType<UIController>();
        InitializeInventory();
    }

    public void InitializeInventory() {
        int nMedkits = 3;
        int nTnt = 15;
        AddItem(new Medkit(200f, nMedkits));
        AddItem(new TNT(nTnt));
    }

    public void AddItem(Item item) {
        items.Add(item);
        uiController.UpdateItemCounter(item);
    }

    private bool TryGetItem<T>(out T item) where T : Item {
        item = items.Find(i => i is T) as T;
        return item != null;
    }

    public void UseItem<T>() where T : Item {
        if(typeof(T) == typeof(TNT)) {
            UseTNT();
        } else if(typeof(T) == typeof(Medkit)) {
            UseMedkit();
        }
    }

    private void UseMedkit() {
        if (TryGetItem(out Medkit medkit)) {
            if (player.Heal(medkit.healing)) {
                medkit.Reduce();
                uiController.SetMedsCounter(medkit.Amount);

                if (medkit.IsEmpty()) items.Remove(medkit);
            }
        } else {
            Debug.Log("No medkids in inventory!");
        }
    }

    private void UseTNT() {
        if(TryGetItem(out TNT tnt)) {
            float requiredSpace = 0.6f;
            Collider[] colliders = Physics.OverlapSphere(player.transform.position, requiredSpace, physicalObjects);
            if (colliders.Length == 0) {
                // No colliders found, so we can spawn the TNT
                SpawnTNT(player.transform.position);
                tnt.Reduce();
                uiController.SetTntsCounter(tnt.Amount);

                if (tnt.IsEmpty()) items.Remove(tnt);
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
