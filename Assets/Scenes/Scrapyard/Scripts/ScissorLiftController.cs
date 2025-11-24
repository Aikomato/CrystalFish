using UnityEngine;
using System.Collections;

public class ScissorLiftController : MonoBehaviour
{
    [Header("References")]
    public Transform scissorScaleObject;    // mesh2_005
    public Transform platformObject;        // mesh4_001

    [Header("Button Settings")]
    public Transform button;                // The cube
    public MeshRenderer buttonRenderer;     // Renderer of the cube
    public Material blueMat;
    public Material redMat;

    [Header("Player Detection (Distance Based)")]
    public Transform player;              // drag your Player here
    public float activationRange = 0.8f;  // distance required to activate

    [Header("Lift Transform Values")]
    public Vector3 startScale = new Vector3(6062.718f, 6062.718f, 613.8636f);
    public Vector3 endScale = new Vector3(6062.718f, 6062.718f, 5637.539f);

    public Vector3 startPos = new Vector3(586.01f, 16.13f, 366.27f);
    public Vector3 endPos = new Vector3(586.01f, 21.74f, 366.27f);

    [Header("Timing")]
    public float delayBeforeLift = 5f;
    public float liftDuration = 3f;
    public float liftStayTime = 7f;
    public float buttonDownOffset = -0.15f;

    private bool activated = false;
    private float originalButtonY;

    void Start()
    {
        // Save original button position
        originalButtonY = button.localPosition.y;

        // Initialize lift
        scissorScaleObject.localScale = startScale;
        platformObject.localPosition = startPos;

        buttonRenderer.material = blueMat;
    }

    void Update()
    {
        if (activated) return;

        float dist = Vector3.Distance(player.position, button.position);

        if (dist < activationRange)
        {
            Debug.Log("Player is within activation range — starting lift sequence.");
            activated = true;
            StartCoroutine(Sequence());
        }
    }

    IEnumerator Sequence()
    {
        // BUTTON PRESS VISUALS
        Debug.Log("Button pressed → turning red & sinking");
        buttonRenderer.material = redMat;

        button.localPosition = new Vector3(
            button.localPosition.x,
            buttonDownOffset,
            button.localPosition.z
        );

        // WAIT BEFORE LIFT MOVES
        yield return new WaitForSeconds(delayBeforeLift);

        // LIFT GOES UP
        Debug.Log("Lift moving UP...");
        float t = 0;
        while (t < liftDuration)
        {
            t += Time.deltaTime;
            float lerp = t / liftDuration;

            scissorScaleObject.localScale = Vector3.Lerp(startScale, endScale, lerp);
            platformObject.localPosition = Vector3.Lerp(startPos, endPos, lerp);

            yield return null;
        }

        // STAY UP
        Debug.Log("Lift is UP. Waiting...");
        yield return new WaitForSeconds(liftStayTime);

        // LIFT GOES DOWN
        Debug.Log("Lift moving DOWN...");
        t = 0;
        while (t < liftDuration)
        {
            t += Time.deltaTime;
            float lerp = t / liftDuration;

            scissorScaleObject.localScale = Vector3.Lerp(endScale, startScale, lerp);
            platformObject.localPosition = Vector3.Lerp(endPos, startPos, lerp);

            yield return null;
        }

        // RESET BUTTON
        Debug.Log("Resetting button...");
        button.localPosition = new Vector3(
            button.localPosition.x,
            originalButtonY,
            button.localPosition.z
        );
        buttonRenderer.material = blueMat;

        activated = false;
        Debug.Log("Lift reset complete. Ready again.");
    }
}
