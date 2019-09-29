using UnityEngine;

namespace RPG.Stats {
  public class ExperienceBar : MonoBehaviour {

    BaseStats baseStats;

    private void Awake() {
      baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
    }

    private void Update() {
      transform.localScale = new Vector3(baseStats.GetExperienceFractionToNextLevel(), 1, 1);
    }
  }
}