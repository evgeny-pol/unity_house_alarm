using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Robber : Person
{
    [SerializeField, Min(0f)] private float _sneakingSpeed = 1.5f;
    [SerializeField, Min(0f)] private float _runningSpeed = 3f;
    [SerializeField, Min(0)] private float _rotationSpeed;
    [SerializeField, Min(0.01f)] private float _waypointApproachDistance = 0.01f;
    [SerializeField, Min(0f)] private float _checkSurroundingsTime = 5f;
    [SerializeField, Min(0f)] private float _waitOutTime = 5f;
    [SerializeField] private Transform _waypointsContainer;

    private Transform[] _waypoints;
    private float _moveSpeed;
    private float _waypointApproachDistanceSqr;
    private Coroutine _currentStateCoroutine;
    private int _currentWaypointIndex;
    private Vector3? _targetPosition;
    private Quaternion? _targetRotation;
    private Animator _animator;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _waypointApproachDistanceSqr = _waypointApproachDistance * _waypointApproachDistance;
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _waypoints = GameObjectUtils.GetChildren(_waypointsContainer);
        ChangeState(ExploreHouse());
    }

    private void FixedUpdate()
    {
        Vector3 newVelocity = _rigidbody.velocity;

        if (_targetPosition.HasValue)
        {
            VectorUtils.SetHorizontalComponent(ref newVelocity, transform.forward * _moveSpeed);
            Vector3 toTargetDirection = VectorUtils.HorizontalDirection(_rigidbody.position, _targetPosition.Value);
            Quaternion targetRotation = Quaternion.LookRotation(toTargetDirection);
            _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            newVelocity.x = newVelocity.z = 0;

            if (_targetRotation.HasValue)
            {
                Quaternion newRotation = Quaternion.RotateTowards(_rigidbody.rotation, _targetRotation.Value, _rotationSpeed * Time.fixedDeltaTime);
                _rigidbody.rotation = newRotation;

                if (newRotation == _targetRotation.Value)
                    _targetRotation = null;
            }
        }

        _rigidbody.velocity = newVelocity;
    }

    private void OnDrawGizmosSelected()
    {
        if (_waypointsContainer)
            Gizmos.DrawLineStrip(_waypointsContainer.Cast<Transform>().Select(t => t.position).ToArray(), false);
    }

    public override void OnDetected()
    {
        base.OnDetected();
        ChangeState(CheckSurroundings());
    }

    private void ChangeState(IEnumerator state)
    {
        if (_currentStateCoroutine != null)
            StopCoroutine(_currentStateCoroutine);

        _currentStateCoroutine = StartCoroutine(state);
    }

    private IEnumerator ExploreHouse()
    {
        _moveSpeed = _sneakingSpeed;
        yield return MoveToWaypoint(_waypoints.Length - 1);
        _currentStateCoroutine = null;
    }

    private IEnumerator MoveToWaypoint(int targetWaypointIndex)
    {
        _animator.SetBool(AnimatorParams.IsAlarmed, false);
        _animator.SetBool(AnimatorParams.IsMoving, true);

        while (enabled)
        {
            Transform currentWaypoint = _waypoints[_currentWaypointIndex];
            Vector3 currentWaypointPosition = currentWaypoint.position;
            Vector3 toCurrentWaypoint = VectorUtils.HorizontalDirection(_rigidbody.position, currentWaypointPosition);

            if (toCurrentWaypoint.sqrMagnitude <= _waypointApproachDistanceSqr)
            {
                if (_currentWaypointIndex == targetWaypointIndex)
                {
                    _targetPosition = null;
                    _targetRotation = Quaternion.LookRotation(currentWaypoint.forward);
                    _animator.SetBool(AnimatorParams.IsMoving, false);
                    yield break;
                }

                _currentWaypointIndex += (int)Mathf.Sign(targetWaypointIndex - _currentWaypointIndex);
                continue;
            }

            _targetPosition = currentWaypointPosition;
            yield return null;
        }
    }

    private IEnumerator CheckSurroundings()
    {
        _animator.SetBool(AnimatorParams.IsAlarmed, true);
        _animator.SetBool(AnimatorParams.IsMoving, false);
        _targetPosition = null;
        _targetRotation = null;
        yield return new WaitForSeconds(_checkSurroundingsTime);

        if (IsDetected)
            ChangeState(RunAway());
        else
            ChangeState(ExploreHouse());
    }

    private IEnumerator RunAway()
    {
        _animator.SetBool(AnimatorParams.IsAlarmed, false);
        _moveSpeed = _runningSpeed;
        _currentWaypointIndex = Mathf.Max(_currentWaypointIndex - 1, 0);
        yield return MoveToWaypoint(0);
        ChangeState(WaitOut());
    }

    private IEnumerator WaitOut()
    {
        yield return new WaitForSeconds(_waitOutTime);
        ChangeState(ExploreHouse());
    }
}
