using UnityEngine;

namespace RPG.UI.DamageText {
  public class DamageTextSpawner : MonoBehaviour {

    [SerializeField] DamageText damageTextPrefab;

    public void Spawn(float damageAmount) {
      DamageText instance = Instantiate(damageTextPrefab, this.transform);
      instance.SetValue(damageAmount);
    }
  }
}