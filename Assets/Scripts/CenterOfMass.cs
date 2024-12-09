using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CenterOfGravity : MonoBehaviour
{
    public Vector3 CenterOfMass;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + transform.rotation * CenterOfMass, 0.1f);
    }
}