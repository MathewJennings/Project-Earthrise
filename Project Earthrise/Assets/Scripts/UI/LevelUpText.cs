using System;
using TMPro;
using UnityEngine;

namespace RPG.UI {
  public class LevelUpText : MonoBehaviour {

    [SerializeField] TextMeshProUGUI levelUpText;

    public void SetValue(int level) {
      levelUpText.text = String.Format("Level {0}", level);
    }

    public void DestroyText() {
      Destroy(this.gameObject);
    }
  }
}