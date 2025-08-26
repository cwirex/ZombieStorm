using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockbackable {
    Rigidbody rb { get; set; }

    public void ApplyKnockbackForce(Vector3 direction, float force);
}