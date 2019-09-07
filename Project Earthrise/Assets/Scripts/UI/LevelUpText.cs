using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI {
  public class LevelUpText : MonoBehaviour {

    [SerializeField] Text levelUpText;

    public void SetValue(int level) {
      levelUpText.text = String.Format("Level {0}", level);
    }

    public void DestroyText() {
      Destroy(this.gameObject);
    }
  }
}