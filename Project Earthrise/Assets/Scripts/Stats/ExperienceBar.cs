using UnityEngine;

namespace RPG.Stats {
  public class ExperienceBar : MonoBehaviour {

    [SerializeField] BaseStats baseStats;
    [SerializeField] RectTransform foreground;

    private void Update() {
      foreground.localScale = new Vector3(baseStats.GetExperienceFractionToNextLevel(), 1, 1);
    }
  }
}