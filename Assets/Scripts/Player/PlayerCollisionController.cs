using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerCollisionController : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.name == "Player House") {

        }
    }
}
