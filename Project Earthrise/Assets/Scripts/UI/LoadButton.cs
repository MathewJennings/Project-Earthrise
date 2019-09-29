using RPG.SceneManagement;
using UnityEngine;

namespace RPG.UI {
  public class LoadButton : MonoBehaviour {

    private SavingWrapper savingWrapper;

    private void Start() {
      savingWrapper = GameObject.FindObjectOfType<SavingWrapper>();
    }

    public void Load() {
      savingWrapper.Load();
    }
  }
}