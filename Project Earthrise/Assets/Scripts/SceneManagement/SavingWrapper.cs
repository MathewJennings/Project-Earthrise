using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement {
  public class SavingWrapper : MonoBehaviour {

    [SerializeField] float timeToFadeInInitialScene = 0.25f;

    const string defaultSaveFile = "save";

    private void Awake() {
      StartCoroutine(LoadLastScene());
    }

    private void Update() {
      if (Input.GetKeyDown(KeyCode.S)) {
        Save();
      }
      if (Input.GetKeyDown(KeyCode.L)) {
        Load();
      }
      if (Input.GetKeyDown(KeyCode.D)) {
        Delete();
      }
    }

    private IEnumerator LoadLastScene() {
      yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
      Fader fader = FindObjectOfType<Fader>();
      fader.FadeOutImmediate();
      yield return fader.FadeIn(timeToFadeInInitialScene);
    }

    public void Save() {
      GetComponent<SavingSystem>().Save(defaultSaveFile);
    }

    public void Load() {
      GetComponent<SavingSystem>().Load(defaultSaveFile);
    }

    public void Delete() {
      GetComponent<SavingSystem>().Delete(defaultSaveFile);
    }
  }
}