using EzySlice;
using UnityEngine;

public class Saber : MonoBehaviour
{
    [SerializeField] private LayerMask _sliceableMask;
    [SerializeField] private Transform _plane;
    [SerializeField] private Vector3 _prevPos;
    [SerializeField] Material _hullMaterial;
    [SerializeField] NoteManager _noteManager;

    private void Awake()
    {
        _prevPos = transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 move = transform.position - _prevPos;

        if (move.sqrMagnitude > 0.005f)
        {
            _plane.right = move.normalized;
        }

        _prevPos = transform.position;
    }

    private void Slice(GameObject target)
    {
        Vector3 sliceNormal = _plane.up;
        Vector3 slicePoint = _plane.position;
        SlicedHull hull = target.Slice(slicePoint, sliceNormal, _hullMaterial);

        if (hull != null)
        {
            if (_noteManager.TryHit(target.transform))
            {
                GameObject upper = hull.CreateUpperHull(target);
                HullBehaviour upperHullBehaviour = upper.AddComponent<HullBehaviour>();
                upperHullBehaviour.velocity = -sliceNormal + Vector3.forward;
                upperHullBehaviour.normal = -sliceNormal;

                GameObject lower = hull.CreateLowerHull(target);
                HullBehaviour lowerHullBehaviour = lower.AddComponent<HullBehaviour>();
                lowerHullBehaviour.velocity = sliceNormal + Vector3.forward;
                lowerHullBehaviour.normal = sliceNormal;
            
                Destroy(target);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & _sliceableMask) == 0)
        {
            return;
        }

        Slice(other.gameObject);
    }
}
