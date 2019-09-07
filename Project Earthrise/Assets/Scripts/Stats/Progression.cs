using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats {
  [CreateAssetMenu(fileName = "Progression", menuName = "Stats/Progression", order = 0)]
  public class Progression : ScriptableObject {

    [SerializeField] ProgressionCharacterClass[] characterClasses = null;

    Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

    public float GetStat(Stat stat, CharacterClass characterClass, int level) {
      BuildLookup();
      float[] levels = lookupTable[characterClass][stat];
      if (level > levels.Length) return -1f;
      return levels[level - 1];
    }

    public int GetNumberOfStatLevels(Stat stat, CharacterClass characterClass) {
      BuildLookup();
      return lookupTable[characterClass][stat].Length;
    }

    private void BuildLookup() {
      if (lookupTable != null) return;

      lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

      foreach (ProgressionCharacterClass progressionClass in characterClasses) {
        Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();
        lookupTable[progressionClass.characterClass] = statLookupTable;
        foreach (ProgressionStat progressionStat in progressionClass.stats) {
          statLookupTable[progressionStat.stat] = progressionStat.levels;
        }
      }
    }

    [System.Serializable]
    class ProgressionCharacterClass {
      public CharacterClass characterClass;
      public ProgressionStat[] stats;
    }

    [System.Serializable]
    class ProgressionStat {
      public Stat stat;
      public float[] levels;
    }
  }
}