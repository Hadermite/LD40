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
            Vector3 newPosition = transform.position + movement;
            newPosition.x = Mathf.Clamp(newPosition.x, -4.7f, 10);
            newPosition.y = Mathf.Clamp(newPosition.y, -3.4f, 10);
            body.MovePosition(newPosition);
        }
    }
}
