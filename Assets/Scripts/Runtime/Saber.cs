using UnityEngine;

public class Saber : MonoBehaviour
{
    [SerializeField] private LayerMask _sliceableMask;

    private void OnTriggerEnter(Collider other)
    {
        if((1 << other.gameObject.layer & _sliceableMask) == 0)
        {
            return;
        }

        // TODO : Do Slice
    }
}
