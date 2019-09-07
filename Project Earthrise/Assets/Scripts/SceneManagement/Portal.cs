using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement {
  public class Portal : MonoBehaviour {

    [SerializeField] int sceneToLoad = -1;
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destination;
    [SerializeField] float fadeOutTime = 0.5f;
    [SerializeField] float fadeWaitTime = 0.75f;
    [SerializeField] float fadeInTime = 0.5f;

    enum DestinationIdentifier {
      A, B, C, D, E
    }

    private void OnTriggerEnter(Collider other) {
      if (other.CompareTag("Player")) {
        StartCoroutine(Transition());
      }
    }

    private IEnumerator Transition() {
      if (sceneToLoad < 0) {
        Debug.LogError("Scene to load not set");
        yield break;
      }

      DontDestroyOnLoad(gameObject);

      Fader fader = FindObjectOfType<Fader>();
      SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
      PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
      playerController.enabled = false;
      yield return fader.FadeOut(fadeOutTime);
      savingWrapper.Save();

      yield return SceneManager.LoadSceneAsync(sceneToLoad);
      PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
      newPlayerController.enabled = false;

      savingWrapper.Load();

      Portal otherPortal = GetOtherPortal();
      UpdatePlayer(otherPortal);

      savingWrapper.Save();

      yield return new WaitForSeconds(fadeWaitTime);
      fader.FadeIn(fadeInTime);

      newPlayerController.enabled = true;
      Destroy(gameObject);
    }

    private Portal GetOtherPortal() {
      foreach (Portal portal in FindObjectsOfType<Portal>()) {
        if (portal == this) continue;
        if (portal.destination != this.destination) continue;

        return portal;
      }
      return null;
    }

    private void UpdatePlayer(Portal otherPortal) {
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      player.GetComponent<NavMeshAgent>().enabled = false;
      player.transform.position = otherPortal.spawnPoint.position;
      player.transform.rotation = otherPortal.spawnPoint.rotation;
      player.GetComponent<NavMeshAgent>().enabled = true;
    }
  }
}