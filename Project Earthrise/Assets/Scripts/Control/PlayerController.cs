using RPG.Attributes;
using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control {
  public class PlayerController : MonoBehaviour {

    [SerializeField] float maxNavMeshProjectionDistance = 1f;
    [SerializeField] float maxNavPathLength = 40f;

    private Health playerHealth;

    private void Awake() {
      playerHealth = GetComponent<Health>();
      Cursor.visible = false;
    }

    void Update() {
      InteractWithMovement();

      if (Input.GetKeyDown(KeyCode.E)) {
        GetComponent<Fighter>().Attack();
      }
    }

    private void InteractWithMovement() {
      Vector3 movementTarget = Vector3.zero;
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
      if (movementTarget != Vector3.zero) {
        GetComponent<Mover>().MoveInDirection(movementTarget, 1f);
      }
    }
  }
}