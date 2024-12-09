using UnityEngine;


/**
 * This component lets the player drag its object while clicking the left mouse button,
 * and drop it by releasing the mouse.
 */
[RequireComponent(typeof(Rigidbody))]
public class DragAndLaunch : MonoBehaviour
{

    [Header("These fields are for display only")]
    [SerializeField] private Vector3 positionMinusMouse;
    [SerializeField] private float screenZCoordinate;

    Rigidbody rb;
    [SerializeField] float forceSize = 10;
    [HideInInspector] public bool WasReleased = false;
    float StartingZ;
    float LowestY;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        LowestY = GetLowestY();
        StartingZ = transform.position.z;
    }

    private float GetLowestY()
    {
        float ans = -Mathf.Infinity;
        Vector3 direction = Vector3.down;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit))
        {
            Vector3 hitPoint = hit.point;
            Debug.Log("Ray Hit Point " + hitPoint);
            ans = hitPoint.y + GetComponent<SphereCollider>().radius;
        }
        return ans;
    }

    // This function is called when the player clicks the collider of this object.
    void OnMouseDown()
    {
        if (WasReleased) return;  // Do not allow the player to drag the object if it was released once.
        screenZCoordinate = Camera.main.WorldToScreenPoint(transform.position).z;
        positionMinusMouse = transform.position - MousePositionOnWorld();
    }

    // This function is called when the player drags the mouse.
    void OnMouseDrag()
    {
        if (WasReleased) return;  // Do not allow the player to drag the object if it was released once.
        Vector3 NewPos = positionMinusMouse + MousePositionOnWorld();
        NewPos.y = Mathf.Max(LowestY, NewPos.y);
        NewPos.z = StartingZ;
        transform.position = NewPos;
    }

    // This function is called when the player releases the mouse button.
    void OnMouseUp()
    {
        if (WasReleased) return;
        WasReleased = true;
        rb.isKinematic = false;
        rb.AddForce(new Vector3(0, 0, forceSize), ForceMode.Impulse);

    }

    private Vector3 MousePositionOnWorld()
    {
        Vector3 mouseOnScreen = Input.mousePosition;    // Screen coordinates of mouse (x,y)
        mouseOnScreen.z = screenZCoordinate;            // z coordinate of game object on screen
        Vector3 mouseOnWorld = Camera.main.ScreenToWorldPoint(mouseOnScreen);
        return mouseOnWorld;
    }

    public void Restart()
    {
        rb.isKinematic = true;
        WasReleased = false;
    }
}

