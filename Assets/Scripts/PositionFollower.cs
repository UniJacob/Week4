using UnityEngine;

/**
 * This component follows the position of a given object, but not its rotation.
 * Especially useful for cameras.
 */ 
public class PositionFollower: MonoBehaviour{
    [SerializeField] GameObject objectToFollow;
    [SerializeField] Vector3 DistanceFromObject;
    private void Update()
    {
        Vector3 pos = objectToFollow.transform.position;
        pos += DistanceFromObject;
        transform.position = pos;
    }
}
