using UnityEngine;

public class Bullet : MonoBehaviour {

    private const float Speed = 10f;

    private float lifeTimer = 3f;

    private void Update() {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0) {
            Destroy(gameObject);
            return;
        }

        Vector3 translation = transform.up * Speed * Time.deltaTime;
        transform.position += translation;
    }
}
