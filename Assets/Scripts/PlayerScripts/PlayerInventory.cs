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
    
    // Maximum storage capacities
    public const int MAX_MEDKITS = 5;
    public const int MAX_TNT = 100;

    private Player player;
    private List<Item> items = new List<Item>();
    private UIController uiController;


    private void Start() {
        player = GetComponent<Player>();
        uiController = FindObjectOfType<UIController>();
        InitializeInventory();
    }

    public void InitializeInventory() {
        int nMedkits = 1; // Starting amount - max storage is 5
        int nTnt = 3;     // Starting amount - max storage is 100  
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
            HealthController healthController = player.GetComponent<HealthController>();
            if (healthController != null && healthController.HealByPercentage(0.5f)) { // Heal 50% of max health
                medkit.Reduce();
                uiController.SetMedsCounter(medkit.Amount);

                if (medkit.IsEmpty()) items.Remove(medkit);
                Debug.Log("Used medkit - healed 50% of max health!");
            } else {
                Debug.Log("Already at full health!");
            }
        } else {
            Debug.Log("No medkits in inventory!");
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
    
    /// <summary>
    /// Gets the current amount of medkits in inventory
    /// </summary>
    public int GetMedkitCount() {
        if (TryGetItem(out Medkit medkit)) {
            return medkit.Amount;
        }
        return 0;
    }
    
    /// <summary>
    /// Gets the current amount of TNT in inventory  
    /// </summary>
    public int GetTNTCount() {
        if (TryGetItem(out TNT tnt)) {
            return tnt.Amount;
        }
        return 0;
    }
    
    /// <summary>
    /// Checks if player can store more medkits
    /// </summary>
    public bool CanAddMedkits(int amount = 1) {
        return GetMedkitCount() + amount <= MAX_MEDKITS;
    }
    
    /// <summary>
    /// Checks if player can store more TNT
    /// </summary>
    public bool CanAddTNT(int amount = 1) {
        return GetTNTCount() + amount <= MAX_TNT;
    }
    
    /// <summary>
    /// Adds medkits to inventory if there's space
    /// </summary>
    public bool TryAddMedkits(int amount) {
        if (!CanAddMedkits(amount)) {
            Debug.Log($"Cannot add {amount} medkits - would exceed max capacity of {MAX_MEDKITS}");
            return false;
        }
        
        if (TryGetItem(out Medkit medkit)) {
            medkit.AddAmount(amount);
            uiController.SetMedsCounter(medkit.Amount);
        } else {
            AddItem(new Medkit(200f, amount));
        }
        return true;
    }
    
    /// <summary>
    /// Adds TNT to inventory if there's space
    /// </summary>
    public bool TryAddTNT(int amount) {
        if (!CanAddTNT(amount)) {
            Debug.Log($"Cannot add {amount} TNT - would exceed max capacity of {MAX_TNT}");
            return false;
        }
        
        if (TryGetItem(out TNT tnt)) {
            tnt.AddAmount(amount);
            uiController.SetTntsCounter(tnt.Amount);
        } else {
            AddItem(new TNT(amount));
        }
        return true;
    }
}
