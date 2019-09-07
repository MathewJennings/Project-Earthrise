using UnityEngine;

namespace RPG.Control {
  public class PatrolPath : MonoBehaviour {

    const float waypointGizmoRadius = 0.25f;

    public Vector3 GetWaypoint(int i) {
      return transform.GetChild(i).position;
    }
    
    public int GetNextWaypointIndex(int i) {
      if (i + 1 == transform.childCount) {
        return 0;
      }
      return i + 1;
    }

    private void OnDrawGizmos() {
      for (int i = 0; i < transform.childCount; i++) {
        Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
        Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(GetNextWaypointIndex(i)));
      }
    }
  }
}