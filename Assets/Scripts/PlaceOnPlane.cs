using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class PlaceOnPlane : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The main camera in use.")]
        Camera m_Camera;

        [SerializeField]
        [Tooltip("Displays this cursor to indicate object placement.")]
        GameObject m_Cursor;

        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject m_PlacedPrefab;

        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        public GameObject placedPrefab
        {
            get { return m_PlacedPrefab; }
            set { m_PlacedPrefab = value; }
        }

        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedObject { get; private set; }

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
        }

        void Update()
        {
            TryGetSnapPoint();
        }

        void TryGetSnapPoint()
        {
            RaycastHit hit;

            if (Physics.Raycast(m_Camera.ScreenPointToRay(transform.position), out hit))
            {
                if (hit.collider.gameObject.CompareTag("SnapPoint"))
                {
                    hit.collider.gameObject.GetComponent<MeshRenderer>().enabled = true;

                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        Instantiate(m_PlacedPrefab, hit.collider.transform);
                    }
                }
                else
                {
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        if (m_RaycastManager.Raycast(m_Camera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f)), s_Hits, TrackableType.PlaneWithinPolygon))
                        {
                            // Raycast hits are sorted by distance, so the first one
                            // will be the closest hit.
                            var hitPose = s_Hits[0].pose;

                            Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                        }
                    }
                }
            }
        }

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        ARRaycastManager m_RaycastManager;
        GameObject blockHolder;
    }
}
