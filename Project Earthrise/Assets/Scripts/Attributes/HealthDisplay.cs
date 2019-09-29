using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes {
  public class HealthDisplay : MonoBehaviour {

    Health health;

    private void Awake() {
      health = GameObject.FindWithTag("Player").GetComponent<Health>();
    }

    private void Update() {
      transform.localScale = new Vector3(health.GetFraction(), 1, 1);
    }
  }
}