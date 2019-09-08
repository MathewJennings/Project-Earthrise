using System;
using RPG.Attributes;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control {
  public class PlayerController : MonoBehaviour {

    [SerializeField] CursorMapping[] cursorMappings = null;
    [SerializeField] float maxNavMeshProjectionDistance = 1f;
    [SerializeField] float maxNavPathLength = 40f;

    [System.Serializable]
    struct CursorMapping {
      public CursorType cursorType;
      public Texture2D texture;
      public Vector2 hotspot;
    }

    private Health playerHealth;

    private void Awake() {
      playerHealth = GetComponent<Health>();
    }

    void Update() {
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

      // Point and click
      if (InteractWithUI()) return;
      if (playerHealth.IsDead()) {
        SetCursor(CursorType.None);
        return;
      }
      if (InteractWithComponent()) return;
      if (InteractWithMovement()) return;
      SetCursor(CursorType.None);
    }

    private bool InteractWithUI() {
      if (EventSystem.current.IsPointerOverGameObject()) {
        SetCursor(CursorType.UI);
        return true;
      }
      return false;
    }

    private bool InteractWithComponent() {
      RaycastHit[] hits = RaycastAllSorted();
      foreach (RaycastHit hit in hits) {
        IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
        foreach (IRaycastable raycastable in raycastables) {
          if (raycastable.HandleRaycast(this)) {
            SetCursor(raycastable.GetCursorType());
            return true;
          }
        }
      }
      return false;
    }

    private RaycastHit[] RaycastAllSorted() {
      RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
      float[] distances = new float[hits.Length];
      for (int i = 0; i < hits.Length; i++) {
        distances[i] = hits[i].distance;
      }
      Array.Sort(distances, hits);
      return hits;
    }

    private bool InteractWithMovement() {
      Vector3 target;
      bool hasHit = RaycastNavMesh(out target);
      if (hasHit) {
        if (Input.GetMouseButton(0)) {
          GetComponent<Mover>().StartMoveAction(target, 1f);
        }
        SetCursor(CursorType.Movement);
        return true;
      }
      return false;
    }

    private bool RaycastNavMesh(out Vector3 target) {
      target = new Vector3();

      RaycastHit raycastHit;
      bool hasHit = Physics.Raycast(GetMouseRay(), out raycastHit);
      if (!hasHit) return false;

      NavMeshHit navMeshHit;
      bool hasCastToNavMesh = NavMesh.SamplePosition(
        raycastHit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
      if (!hasCastToNavMesh) return false;

      target = navMeshHit.position;

      NavMeshPath path = new NavMeshPath();
      bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
      if (!hasPath) return false;
      if (path.status != NavMeshPathStatus.PathComplete) return false;
      if (GetPathLength(path) > maxNavPathLength) return false;

      return true;
    }

    private float GetPathLength(NavMeshPath path) {
      float total = 0f;
      if (path.corners.Length < 2) return total;
      for (int i = 0; i < path.corners.Length - 1; i++) {
        total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
      }
      return total;
    }

    private static Ray GetMouseRay() {
      return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private void SetCursor(CursorType type) {
      CursorMapping mapping = GetCursorMapping(type);
      Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
    }

    private CursorMapping GetCursorMapping(CursorType type) {
      foreach (CursorMapping mapping in cursorMappings) {
        if (mapping.cursorType == type) {
          return mapping;
        }
      }
      return cursorMappings[0];
    }
  }
}