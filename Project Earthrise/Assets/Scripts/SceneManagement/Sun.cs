using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement {
  public class Sun : MonoBehaviour, ISaveable {

    // In seconds
    [SerializeField] float lengthOfDay = 300;
    [SerializeField] Color noonTint = new Color(128, 128, 128);
    [SerializeField] Color midnightTint = new Color(32, 32, 32);

    // private float midnight = 270f;
    // private float dawn/dusk = 0f/180f/360f;
    // private float noon = 90f;

    public object CaptureState() {
      return new SerializableVector3(transform.rotation.eulerAngles);
    }

    public void RestoreState(object state) {
      transform.rotation = Quaternion.Euler(((SerializableVector3) state).ToVector3());
      SetSkyTint();
    }

    void Update() {
      transform.Rotate(360 * Time.deltaTime / lengthOfDay, 0, 0, Space.World);
      SetSkyTint();
    }

    private void SetSkyTint() {
      float normalizedXRotation = (transform.eulerAngles.x + 90) % 360 / 180;
      // now midnight = 0 and noon = 1
      float skyTint = midnightTint.r + normalizedXRotation * (noonTint.r - midnightTint.r);
      RenderSettings.skybox.SetColor("_TintColor", new Color(skyTint, skyTint, skyTint));
    }
  }
}