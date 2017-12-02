using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour {

    private const float MaxSpeed = 10f;
    private const float RunMultiplier = 2f;
    
    private Rigidbody2D body;

    private void Start() {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        Vector3 movement = new Vector3 {
            x = Input.GetAxisRaw("Horizontal"),
            y = Input.GetAxisRaw("Vertical")
        };

        if (movement.magnitude != 0) {
            float multiplier = 1;
            if (Input.GetKey(KeyCode.LeftShift)) {
                multiplier = RunMultiplier;
            }
            movement = movement.normalized * MaxSpeed * Time.deltaTime * multiplier;
            body.MovePosition(transform.position + movement);
        }
    }
}
