using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {
  public class CinematicTrigger : MonoBehaviour, ISaveable {

    bool hasBeenTriggered = false;

    public object CaptureState() {
      return hasBeenTriggered;
    }

    public void RestoreState(object state) {
      hasBeenTriggered = (bool)state;
    }

    private void OnTriggerEnter(Collider other) {
      if (!hasBeenTriggered && other.CompareTag("Player")) {
        GetComponent<PlayableDirector>().Play();
        hasBeenTriggered = true;
      }
    }
  }
}