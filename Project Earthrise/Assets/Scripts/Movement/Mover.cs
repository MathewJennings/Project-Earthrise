using System.Collections;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
  public class Mover : MonoBehaviour, IAction, ISaveable {

    [SerializeField] float maxSpeed = 6f;
    [Range(1, 2)]
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float maxRotationTime = 1f;
    [SerializeField] float energyToSprint = 0.1f;

    private NavMeshAgent navMeshAgent;
    private Health health;
    private Energy energy;

    private void Awake() {
      navMeshAgent = GetComponent<NavMeshAgent>();
      health = GetComponent<Health>();
      energy = GetComponent<Energy>();
    }

    private void Update() {
      navMeshAgent.enabled = !health.IsDead();
      UpdateAnimator();
    }

    public void StartMoveAction(Vector3 destination, float speedFraction) {
      GetComponent<ActionScheduler>().StartAction(this);
      MoveTo(destination, speedFraction);
    }

    public void MoveTo(Vector3 destination, float speedFraction) {
      navMeshAgent.destination = destination;
      navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
      navMeshAgent.isStopped = false;
    }

    public void MoveInDirection(Vector3 direction, float speedFraction) {
      GetComponent<ActionScheduler>().StartAction(this);
      if (speedFraction > 1) {
        if (energy.CanConsumeEnergy(energyToSprint)) {
          energy.ConsumeEnergy(energyToSprint);
        } else {
          speedFraction = 1;
        }
      }
      navMeshAgent.destination = transform.position + direction;
      navMeshAgent.speed = maxSpeed * Mathf.Max(0, speedFraction);
      navMeshAgent.isStopped = false;
    }

    public IEnumerator RotateAsynchronously(Vector3 newForward) {
      float elapsedRotationTime = 0f;
      while (!DoneRotating(newForward)) {
        elapsedRotationTime += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newForward), elapsedRotationTime);
        if (elapsedRotationTime / rotationSpeed > maxRotationTime) yield break;
        yield return new WaitForEndOfFrame();
      }
    }

    public void Cancel() {
      navMeshAgent.isStopped = true;
    }

    public object CaptureState() {
      return new SerializableVector3(transform.position);
    }

    public void RestoreState(object state) {
      GetComponent<ActionScheduler>().CancelCurrentAction();
      NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
      navMeshAgent.enabled = false;
      transform.position = ((SerializableVector3) state).ToVector3();
      navMeshAgent.enabled = true;
    }

    private void UpdateAnimator() {
      Vector3 velocity = navMeshAgent.velocity;
      Vector3 localVelocity = transform.InverseTransformDirection(velocity);
      float speed = localVelocity.z;
      GetComponent<Animator>().SetFloat("forwardSpeed", speed);
    }

    private bool DoneRotating(Vector3 newForward) {
      Vector3 normalizedPlayerForward = transform.forward.normalized;
      Ray playerRotationRay = new Ray(transform.position, normalizedPlayerForward);
      Vector3 normalizedCameraForward = new Vector3(newForward.x, 0, newForward.z).normalized;
      return Vector3.Distance(normalizedPlayerForward, normalizedCameraForward) < 0.01f;
    }
  }
}