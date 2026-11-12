using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class JumpTracker : MonoBehaviour
{
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private Vector3 jumpPoint;
    [SerializeField] private Vector3 jumpStartPosition;
    [SerializeField] private Vector3 jumpEndPosition;
    [SerializeField] private float jumpDistance;
    [SerializeField] private float markerSize = 0.25f;

    private CharacterController characterController;
    private Vector3 currentPosition;
    private float jumpHeight;
    private bool isTracking;
    private GameObject startMarker;
    private GameObject endMarker;

    public Vector3 JumpPoint => jumpPoint;
    public Vector3 JumpStartPosition => jumpStartPosition;
    public Vector3 JumpEndPosition => jumpEndPosition;
    public float JumpDistance => jumpDistance;
    public float JumpHeight => jumpHeight;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (distanceText == null)
        {
            Debug.LogError("Assign a TextMeshPro text component to display the jump distance and player position.", this);
            return;
        }

        TrackPlayerPosition();
    }

    public void BeginJump()
    {
        if (!characterController.isGrounded)
        {
            return;
        }

        ClearMarkers();
        jumpPoint = transform.position;
        RecordJumpPosition(true);
        jumpEndPosition = Vector3.zero;
        jumpDistance = 0f;
        jumpHeight = 0f;
        isTracking = true;
        startMarker = CreateMarker(jumpPoint);
        TrackPlayerPosition();
    }

    public void RecordJumpPosition(bool beforeJump)
    {
        if (beforeJump)
        {
            jumpStartPosition = transform.position;
            return;
        }

        jumpEndPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (isTracking)
        {
            Vector3 position = transform.position;
            jumpDistance = Vector2.Distance(
                new Vector2(jumpPoint.x, jumpPoint.z),
                new Vector2(position.x, position.z));
            jumpHeight = Mathf.Max(0f, position.y - jumpStartPosition.y);

            if (characterController.isGrounded)
            {
                isTracking = false;
                RecordJumpPosition(false);
                endMarker = CreateMarker(position);
            }
        }

        TrackPlayerPosition();
    }

    private void TrackPlayerPosition()
    {
        currentPosition = transform.position;
        UpdateDistanceText();
    }

    private GameObject CreateMarker(Vector3 position)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = "Jump Marker";
        marker.transform.position = position;
        marker.transform.localScale = Vector3.one * markerSize;
        Destroy(marker.GetComponent<Collider>());

        Renderer markerRenderer = marker.GetComponent<Renderer>();
        MaterialPropertyBlock properties = new MaterialPropertyBlock();
        markerRenderer.GetPropertyBlock(properties);
        properties.SetColor("_Color", Color.red);
        properties.SetColor("_BaseColor", Color.red);
        markerRenderer.SetPropertyBlock(properties);

        return marker;
    }

    private void ClearMarkers()
    {
        if (startMarker != null)
        {
            Destroy(startMarker);
        }

        if (endMarker != null)
        {
            Destroy(endMarker);
        }
    }

    private void UpdateDistanceText()
    {
        if (distanceText != null)
        {
            distanceText.text =
                $"Current: {currentPosition}\nDistance: {jumpDistance:F2} m\nHeight: {jumpHeight:F2} m\nStart: {jumpStartPosition}\nEnd: {jumpEndPosition}";
        }
    }
}
