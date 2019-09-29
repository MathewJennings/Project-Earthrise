using RPG.SceneManagement;
using UnityEngine;

namespace RPG.UI {
  public class DeleteSaveButton : MonoBehaviour {

    private SavingWrapper savingWrapper;

    private void Start() {
      savingWrapper = GameObject.FindObjectOfType<SavingWrapper>();
    }

    public void DeleteSave() {
      savingWrapper.Delete();
    }
  }
}