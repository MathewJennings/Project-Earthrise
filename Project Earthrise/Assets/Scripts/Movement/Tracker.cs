using UnityEngine;

namespace RPG.Movement {
  public class Tracker : MonoBehaviour {

    [SerializeField]
    bool trackPlayer;
    
    [SerializeField]
    GameObject target;

    [SerializeField]
    Vector3 offsetVector;

    private void Awake() {
      if (trackPlayer) {
        target = GameObject.FindWithTag("Player");
      }
    }

    void Update() {
      transform.position = target.transform.position + offsetVector;
    }

    public void setTarget(GameObject target) {
      this.target = target;
    }
  }
}