using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour, IWeapon
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 20f;
    [SerializeField] float bulletLifetime = 5f;

    private GameObject thread;

    private void Start() {
        thread = GameObject.FindGameObjectWithTag("Thread");
    }

    public void Shoot() {
        if (thread != null) {
            Vector3 threadPosition = thread.transform.position;
            GameObject bullet = Instantiate(bulletPrefab, threadPosition, transform.rotation);
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = transform.forward * bulletSpeed;

            Destroy(bullet, bulletLifetime);
        }
    }
}
