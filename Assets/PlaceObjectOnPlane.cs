using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObjectOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The prefab to instantiate on a detected AR plane.")]
    private GameObject m_ObjectPrefab;

    public GameObject objectPrefab
    {
        get => m_ObjectPrefab;
        set => m_ObjectPrefab = value;
    }

    private ARRaycastManager m_RaycastManager;
    private GameObject m_SpawnedObject;
    private static readonly List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        Vector2 screenPosition;
        if (!TryGetInputPosition(out screenPosition))
        {
            return;
        }

        if (m_RaycastManager != null && m_RaycastManager.Raycast(screenPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = s_Hits[0].pose;

            if (m_SpawnedObject == null)
            {
                if (m_ObjectPrefab != null)
                {
                    m_SpawnedObject = Instantiate(m_ObjectPrefab, hitPose.position, hitPose.rotation);
                }
            }
            else
            {
                m_SpawnedObject.transform.position = hitPose.position;
                m_SpawnedObject.transform.rotation = hitPose.rotation;
            }
        }
    }

    private bool TryGetInputPosition(out Vector2 screenPosition)
    {
        // Check for touch input
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                screenPosition = touch.position.ReadValue();
                return true;
            }
        }

        // Check for mouse input (Editor / Standalone)
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                screenPosition = Mouse.current.position.ReadValue();
                return true;
            }
        }

        screenPosition = default;
        return false;
    }
}
