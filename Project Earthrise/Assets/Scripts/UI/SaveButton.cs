using RPG.SceneManagement;
using UnityEngine;

namespace RPG.UI {
  public class SaveButton : MonoBehaviour {

    private SavingWrapper savingWrapper;

    private void Start() {
      savingWrapper = GameObject.FindObjectOfType<SavingWrapper>();
    }

    public void Save() {
      savingWrapper.Save();
    }
  }
}