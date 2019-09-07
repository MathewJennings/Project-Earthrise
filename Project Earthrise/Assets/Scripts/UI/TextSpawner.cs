using RPG.Stats;
using UnityEngine;

namespace RPG.UI {
  public class TextSpawner : MonoBehaviour {

    [SerializeField] DamageText damageTextPrefab;
    [SerializeField] LevelUpText levelUpTextPrefab;

    private void OnEnable() {
      GetComponentInParent<BaseStats>().onLevelUp += SpawnLevelUpText;
    }

    private void OnDisable() {
      GetComponentInParent<BaseStats>().onLevelUp += SpawnLevelUpText;
    }

    public void SpawnDamageText(float damageAmount) {
      DamageText instance = Instantiate(damageTextPrefab, this.transform);
      instance.SetValue(damageAmount);
    }

    public void SpawnLevelUpText(int newLevel) {
      LevelUpText instance = Instantiate(levelUpTextPrefab, this.transform);
      instance.SetValue(newLevel);
    }
  }
}