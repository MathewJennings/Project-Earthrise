using UnityEngine;

namespace RPG.UI {
  public class CompassMover : MonoBehaviour {

    [SerializeField] float numberOfPixelsNorthToNorth;

    private Vector3 north;
    private Vector3 startPosition;
    private float pixelsPerDegree;

    void Start() {
      north = Vector3.forward;
      startPosition = transform.position;
      pixelsPerDegree = numberOfPixelsNorthToNorth / 360f;
    }

    void Update() {
      Vector3 normalizedCameraForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
      Vector3 perpendicularVector = Vector3.Cross(north, normalizedCameraForward);
      float direction = Vector3.Dot(perpendicularVector, Vector3.up);
      transform.position = startPosition + (new Vector3(Vector3.Angle(normalizedCameraForward, north) * Mathf.Sign(direction) * pixelsPerDegree, 0, 0));
    }
  }
}