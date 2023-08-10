using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player {
    public abstract class Item {
        public int Amount { get; set; }
        public virtual void AddAmount(int amount) {
            Amount += amount;
        }

        public virtual void Reduce(int amount = 1) {
            Amount -= amount;
            if(Amount < 0) {
                throw new System.ArgumentOutOfRangeException("Amount of Item " + this + " is < 0.");
            }
        }

        public virtual bool IsEmpty() {
            return Amount == 0;
        }

        public virtual bool IsNotEmpty() {
            return Amount != 0;
        }
    }

    public class Medkit : Item {
        public float healing;

        public Medkit(float healing, int amount = 0) {
            this.healing = healing;
            Amount = amount;
        }
    }

    public class TNT : Item {
        public TNT(int amount = 0) {
            Amount = amount;
        }
    }

}