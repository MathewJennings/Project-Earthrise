using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats {
  public class BaseStats : MonoBehaviour, ISaveable {
    [Range(1, 5)]
    [SerializeField] int startingLevel = 1;
    [SerializeField] CharacterClass characterClass;
    [SerializeField] Progression progression = null;
    [SerializeField] GameObject levelUpEffect = null;
    [SerializeField] bool shouldUseModifiers = false;

    public delegate void LevelUpAction(int newLevel);
    public event LevelUpAction onLevelUp;

    Experience experience;
    LazyValue<int> currentLevel;

    private void Awake() {
      experience = GetComponent<Experience>();
      currentLevel = new LazyValue<int>(CalculateLevel);
    }

    private void OnEnable() {
      if (experience != null) {
        experience.onExperienceGained += UpdateLevel;
      }
    }

    private void OnDisable() {
      if (experience != null) {
        experience.onExperienceGained -= UpdateLevel;
      }
    }

    private void Start() {
      currentLevel.ForceInit();
    }

    public object CaptureState() {
      return GetLevel();
    }

    public void RestoreState(object state) {
      currentLevel.value = (int)state;
    }

    public float GetStat(Stat stat) {
      return (GetBaseStat(stat) + GetAdditiveModifer(stat)) * (1 + GetPercentageModifier(stat) / 100);
    }

    public int GetLevel() {
      return currentLevel.value;
    }

    public float GetExperienceFractionToNextLevel() {
      float xpNeededForLastLevelUp;
      if (GetLevel() == 1) {
        xpNeededForLastLevelUp = 0;
      } else {
        xpNeededForLastLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, GetLevel() - 1);
      }
      float pointsBeyondCurrentLevel = experience.GetPoints() - xpNeededForLastLevelUp;
      float pointsToNextLevel = GetStat(Stat.ExperienceToLevelUp) - xpNeededForLastLevelUp;
      return pointsBeyondCurrentLevel / pointsToNextLevel;
    }

    private float GetBaseStat(Stat stat) {
      return progression.GetStat(stat, characterClass, GetLevel());
    }

    private float GetAdditiveModifer(Stat stat) {
      if (!shouldUseModifiers) return 0f;
      float total = 0f;
      foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
        foreach (float modifier in provider.GetAdditiveModifiers(stat)) {
          total += modifier;
        }
      }
      return total;
    }

    private float GetPercentageModifier(Stat stat) {
      if (!shouldUseModifiers) return 0f;
      float total = 0f;
      foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
        foreach (float modifier in provider.GetPercentageModifiers(stat)) {
          total += modifier;
        }
      }
      return total;
    }

    private int CalculateLevel() {
      if (experience == null) return startingLevel;
      float currentXP = experience.GetPoints();
      int penultimateLevel = progression.GetNumberOfStatLevels(Stat.ExperienceToLevelUp, characterClass);
      for (int level = 1; level <= penultimateLevel; level++) {
        float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
        if (xpToLevelUp > currentXP) {
          return level;
        }
      }
      return penultimateLevel + 1;
    }

    private void UpdateLevel() {
      int newLevel = CalculateLevel();
      if (newLevel > currentLevel.value) {
        currentLevel.value = newLevel;
        SpawnLevelUpEffect(newLevel);
        onLevelUp(newLevel);
      }
    }

    private void SpawnLevelUpEffect(int newLevel) {
      Instantiate(levelUpEffect);
    }
  }
}