using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void TakeDamage(float  damage);
    void TakeDamage(float damage, Vector3 direction);
    void Die();
}
