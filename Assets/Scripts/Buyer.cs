using UnityEngine;

public class Buyer : MonoBehaviour {

    public BuyerController controller;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.transform.tag == "BuyerBullet") return;
        if (collider.transform.tag == "PlayerBullet") {
            controller.DismissBuyer();
            Destroy(collider.gameObject);
        }
    }
}
