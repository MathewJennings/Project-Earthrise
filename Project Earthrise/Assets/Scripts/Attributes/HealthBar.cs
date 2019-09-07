using UnityEngine;

namespace RPG.Attributes {
  public class HealthBar : MonoBehaviour {

    [SerializeField] Health healthComponent;
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform foreground;

    private void Update() {
      float healthFraction = healthComponent.GetFraction();
      if (Mathf.Approximately(healthFraction, 0) || Mathf.Approximately(healthFraction, 1)) {
        canvas.enabled = false;
      } else {
        canvas.enabled = true;
        foreground.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
      }
    }
  }
}