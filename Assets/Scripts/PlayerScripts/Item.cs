using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player {
    public abstract class Item {
        public string itemName;
        //other common values
    }

    public class Medkit : Item {
        public float healingAmount;

        public Medkit(float healing) {
            healingAmount = healing;
            itemName = "Apteczka";
        }
    }

    public class TNT : Item {
        public TNT() {
            itemName = "TNT";
        }
    }

}