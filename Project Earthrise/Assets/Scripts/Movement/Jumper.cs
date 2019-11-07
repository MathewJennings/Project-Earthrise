using System;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
  /**
   * The NavMeshAgent of the attached GameObject will be disabled when IsJumping() returns true
   */
  public class Jumper : MonoBehaviour, IAction {

    [SerializeField] float jumpForce = 700f;
    [SerializeField] float initialBoost = 0.3f;
    [SerializeField] float energyToJump = 10f;
    [SerializeField] float navMeshSamplingTolerance = 1f;

    private NavMeshAgent navMeshAgent;
    private Rigidbody rbody;
    private Energy energy;
    private bool isJumping;
    private Vector3 cachedPosition;

    private void Awake() {
      navMeshAgent = GetComponent<NavMeshAgent>();
      rbody = GetComponent<Rigidbody>();
      energy = GetComponent<Energy>();
    }

    private void Update() {
      if (!isJumping) return;

      if (transform.position == cachedPosition) {
        GetComponent<Animator>().SetTrigger("land");
      }

      if (NearGround()) {
        Land();
      }
      cachedPosition = transform.position;
    }

    public bool IsJumping() {
      return isJumping;
    }

    public void Jump() {
      if (isJumping) return;
      if (!energy.CanConsumeEnergy(energyToJump)) return;

      GetComponent<ActionScheduler>().StartAction(this);
      energy.ConsumeEnergy(energyToJump);
      navMeshAgent.enabled = false;
      rbody.isKinematic = false;
      rbody.useGravity = true;
      cachedPosition = transform.position;
      transform.Translate(0, initialBoost, 0);
      rbody.AddForce(0, jumpForce, 0, ForceMode.Impulse);
      isJumping = true;
      GetComponent<Animator>().SetTrigger("jump");
    }

    public void Cancel() {
      // Let the Jump finish naturally
    }

    public void MoveWhileJumping(Vector3 direction, float speed, float elapsedRotationTime) {
      transform.position += direction.normalized * Time.deltaTime * speed;
      if (Mathf.Approximately(elapsedRotationTime, 0f)) {
        StartCoroutine(GetComponent<Mover>().RotateAsynchronously(direction));
      }

      // If gliding up slopes, cancel the movement
      if (rbody.velocity.y < 0 && transform.position.y > cachedPosition.y) {
        print("CORRECTING");
        // transform.position -= direction.normalized * Time.deltaTime * speed;
      }
    }

    private bool NearGround() {
      Vector3 raycastPosition = RaycastNavMesh();
      if (raycastPosition.Equals(Vector3.zero)) return false;

      return Vector3.Distance(transform.position, raycastPosition) < 0.1f;
    }

    private Vector3 RaycastNavMesh() {
      RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down);
      if (hits.Length < 1) return Vector3.zero;

      float[] distances = new float[hits.Length];
      for (int i = 0; i < hits.Length; i++) {
        distances[i] = hits[i].distance;
      }
      Array.Sort(distances, hits);

      NavMeshHit navMeshHit;
      bool hasCastToNavMesh = NavMesh.SamplePosition(
        hits[hits.Length - 1].point, out navMeshHit, navMeshSamplingTolerance, NavMesh.AllAreas);
      if (!hasCastToNavMesh) return Vector3.zero;

      return navMeshHit.position;
    }

    private void Land() {
      navMeshAgent.enabled = true;
      rbody.isKinematic = true;
      rbody.useGravity = false;
      isJumping = false;
      GetComponent<Animator>().SetTrigger("land");
    }
  }
}