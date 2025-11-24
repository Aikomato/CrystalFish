using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Wire : MonoBehaviour
{
    [Header("Wire Setup")]
    public Transform wireStart;           // The anchor point (previously parent position)
    public SpriteRenderer wireEnd;        // The stretched sprite
    public GameObject lightOn;            // Light object to activate once connected

    [Header("Scene Progression")]
    public string nextSceneName = "";     // Set this to any scene name
    public static int completedWires = 0; // Tracks all wired connections
    public static int totalWires = 4;     // How many wires must be lit

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, .2f);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                UpdateWire(hit.transform.position);

                // Wires match when parents have same name
                if (hit.transform.parent != null &&
                    transform.parent != null &&
                    hit.transform.parent.name == transform.parent.name)
                {
                    hit.GetComponent<Wire>()?.CompleteConnection();
                    CompleteConnection();
                }

                return;
            }
        }

        UpdateWire(mousePos);
    }

    private void OnMouseUp()
    {
        // Snap back to original position without flipping
        UpdateWire(startPosition);
    }

    public void CompleteConnection()
    {
        if (!lightOn.activeSelf)
        {
            lightOn.SetActive(true);
            completedWires++;

            // If all wires done, load next scene
            if (completedWires >= totalWires && nextSceneName != "")
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }

        Destroy(this); // disable dragging
    }

    private void UpdateWire(Vector3 endPoint)
    {
        transform.position = endPoint;

        // Direction vector
        Vector3 direction = endPoint - wireStart.position;

        // Rotation without flipping:
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Adjust wire length without flipping sprite
        float dist = Vector2.Distance(wireStart.position, endPoint);
        wireEnd.size = new Vector2(dist, wireEnd.size.y);
    }
}
