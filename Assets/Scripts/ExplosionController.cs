using System.Collections;
using UnityEngine;

public class ExplosionController : MonoBehaviour {
    [SerializeField] private GameObject smallExplosionPrefab;
    [SerializeField] private GameObject mediumExplosionPrefab;
    [SerializeField] private GameObject largeExplosionPrefab;

    private void MakeExplosion(Vector3 position, GameObject explosionPrefab) {
        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);

        float effectDuration = 3f;
        Destroy(explosion, effectDuration);
    }

    public void MakeSmallExplosion(Vector3 position) {
        if (position.y < 0.1f) position.y = 1f;
        MakeExplosion(position, smallExplosionPrefab);
    }

    public void MakeMediumExplosion(Vector3 position) {
        if (position.y < 0.1f) position.y = 1f;
        MakeExplosion(position, mediumExplosionPrefab);
    }

    public void MakeLargeExplosion(Vector3 position) {
        if (position.y < 0.1f) position.y = 1.5f;
        MakeExplosion(position, largeExplosionPrefab);
    }
}
