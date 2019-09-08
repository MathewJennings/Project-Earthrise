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
      // Tech Debt. Why is this null when first starting the game?
      BaseStats parentBaseStats = GetComponentInParent<BaseStats>();
      if (parentBaseStats != null) {
        parentBaseStats.onLevelUp -= SpawnLevelUpText;
      }
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