using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
  public class Mover : MonoBehaviour, IAction, ISaveable {

    [SerializeField] float maxSpeed = 6f;

    private NavMeshAgent navMeshAgent;
    private Health health;

    private void Awake() {
      navMeshAgent = GetComponent<NavMeshAgent>();
      health = GetComponent<Health>();
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
      navMeshAgent.destination = transform.position + direction;
      navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
      navMeshAgent.isStopped = false;
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
  }
}