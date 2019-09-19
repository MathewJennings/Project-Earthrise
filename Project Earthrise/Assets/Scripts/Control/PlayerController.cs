using RPG.Attributes;
using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control {
  public class PlayerController : MonoBehaviour {

    private Health playerHealth;

    private void Awake() {
      playerHealth = GetComponent<Health>();
      Cursor.visible = false;
    }

    void Update() {
      InteractWithMovement();

      if (Input.GetMouseButtonDown(0)) {
        GetComponent<Fighter>().Attack();
      }
    }

    private void InteractWithMovement() {
      Vector3 movementTarget = Vector3.zero;
      float speed = 1f;
      if (Input.GetKey(KeyCode.W)) {
        movementTarget += Camera.main.transform.forward;
      }
      if (Input.GetKey(KeyCode.A)) {
        movementTarget -= Camera.main.transform.right;
      }
      if (Input.GetKey(KeyCode.S)) {
        movementTarget -= Camera.main.transform.forward;
      }
      if (Input.GetKey(KeyCode.D)) {
        movementTarget += Camera.main.transform.right;
      }
      if (Input.GetKey(KeyCode.LeftControl)) {
        speed = 0.3f;
      }
      if (Input.GetKey(KeyCode.LeftShift)) {
        speed = 1.2f;
      }
      if (Input.GetKeyUp(KeyCode.LeftShift)) {
        GetComponent<Energy>().StopConsumingEnergy();
      }
      if (movementTarget != Vector3.zero) {
        GetComponent<Mover>().MoveInDirection(movementTarget, speed);
      }
    }
  }
}