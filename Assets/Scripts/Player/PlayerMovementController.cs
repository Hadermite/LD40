using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour {

    private const float MovementSpeed = 5f;
    private const float RunMultiplier = 2f;
    
    private Rigidbody2D body;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        Vector3 movement = new Vector3 {
            x = Input.GetAxisRaw("Horizontal"),
            y = Input.GetAxisRaw("Vertical")
        };

        if (movement.magnitude != 0) {
            float multiplier = 1;
            if (Input.GetKey(KeyCode.LeftShift)) {
                multiplier = RunMultiplier;
            }
            movement = movement.normalized * MovementSpeed * multiplier * Time.deltaTime;
            body.MovePosition(transform.position + movement);
        }
    }
}
