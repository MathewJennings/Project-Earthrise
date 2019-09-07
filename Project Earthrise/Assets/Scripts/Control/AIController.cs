using GameDevTV.Utils;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Control {
  public class AIController : MonoBehaviour {
    [SerializeField] float chaseDistance = 5f;
    [SerializeField] float suspicionTime = 3f;
    [Range(0, 1)]
    [SerializeField] float patrolSpeedFraction = 0.35f;
    [SerializeField] PatrolPath patrolPath;
    [SerializeField] float waypointTolerance = 1f;
    [SerializeField] float waypointDwellTime = 3f;

    private ActionScheduler actionScheduler;
    private Mover mover;
    private Fighter fighter;
    private Health health;
    private GameObject player;

    private LazyValue<Vector3> guardPosition;
    private float timeSinceLastSawPlayer = Mathf.Infinity;
    private int currentWaypointIndex;
    private float timeSinceArrivedAtWaypoint = Mathf.Infinity;

    private void Awake() {
      actionScheduler = GetComponent<ActionScheduler>();
      mover = GetComponent<Mover>();
      fighter = GetComponent<Fighter>();
      health = GetComponent<Health>();
      player = GameObject.FindWithTag("Player");
      guardPosition = new LazyValue<Vector3>(GetGuardPosition);
    }

    private Vector3 GetGuardPosition() {
      return transform.position;
    }

    private void Start() {
      guardPosition.ForceInit();
    }

    private void Update() {
      if (health.IsDead()) return;

      if (InAttackRangeOfPlayer() && fighter.CanAttack(player)) {
        AttackBehavior();
      } else if (timeSinceLastSawPlayer <= suspicionTime) {
        SuspicionBehavior();
      } else {
        PatrolBehavior();
      }

      UpdateTimers();
    }

    private bool InAttackRangeOfPlayer() {
      return Vector3.Distance(transform.position, player.transform.position) <= chaseDistance;
    }

    private void AttackBehavior() {
      timeSinceLastSawPlayer = 0;
      fighter.Attack(player);
    }

    private void SuspicionBehavior() {
      actionScheduler.CancelCurrentAction();
    }

    private void PatrolBehavior() {
      Vector3 nextPosition = guardPosition.value;
      if (patrolPath != null) {
        if (AtWaypoint()) {
          timeSinceArrivedAtWaypoint = 0;
          CycleWaypoint();
        }
        nextPosition = GetCurrentWaypoint();
      }
      if (timeSinceArrivedAtWaypoint >= waypointDwellTime) {
        mover.StartMoveAction(nextPosition, patrolSpeedFraction);
      }
    }

    private bool AtWaypoint() {
      float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
      return distanceToWaypoint <= waypointTolerance;
    }

    private void CycleWaypoint() {
      currentWaypointIndex = patrolPath.GetNextWaypointIndex(currentWaypointIndex);
    }

    private Vector3 GetCurrentWaypoint() {
      return patrolPath.GetWaypoint(currentWaypointIndex);
    }

    private void UpdateTimers() {
      timeSinceLastSawPlayer += Time.deltaTime;
      timeSinceArrivedAtWaypoint += Time.deltaTime;
    }

    private void OnDrawGizmosSelected() {
      Gizmos.color = Color.blue;
      Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
  }
}