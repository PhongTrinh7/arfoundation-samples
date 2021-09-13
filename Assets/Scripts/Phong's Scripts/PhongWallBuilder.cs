using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public class PhongWallBuilder : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Highlightable layer.")]
        LayerMask layerMask;

        [SerializeField]
        [Tooltip("The main camera in use.")]
        Camera m_Camera;

        [SerializeField]
        [Tooltip("The cursor for your screen.")]
        Image cursor;

        [SerializeField]
        [Tooltip("The color of your cursor when hovering over a snap point")]
        Color cursorHighlightColor;

        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject[] placablePrefabs;

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();

            //An empty gameobject to organize hierarchy and hold all placed blocks.
            blockHolder = new GameObject("Blocks Holder");
        }

        private void Update()
        {
            CursorOverSnapPoint();
        }

        void CursorOverSnapPoint()
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, layerMask))
            {
                if (hit.collider.CompareTag("SnapPoint"))
                {
                    cursor.color = cursorHighlightColor;
                    return;
                }
            }
            cursor.color = Color.white;
        }

        bool CheckSnapPoint(int placableIndex)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, layerMask))
            {
                if (hit.collider.CompareTag("SnapPoint"))
                {
                    Instantiate(placablePrefabs[placableIndex], hit.transform.position, hit.transform.rotation, blockHolder.transform);
                    return true;
                }
            }

            return false;
        }

        void CheckPlanes(int placableIndex)
        {
            if (m_RaycastManager.Raycast(m_Camera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f)), s_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;

                //Instantiate slightly higher when placing on ground planes.
                Vector3 offsetFromGround = new Vector3(0, 0.05f, 0);

                Instantiate(placablePrefabs[placableIndex], hitPose.position + offsetFromGround, hitPose.rotation, blockHolder.transform);
            }
        }

        public void PlaceBlock(int placableIndex)
        {
            if (placableIndex > placablePrefabs.Length)
            {
                Debug.LogError("No corresponding placable at this index.");
                return;
            }

            if (!CheckSnapPoint(placableIndex))
            {
                CheckPlanes(placableIndex);
            }
        }

        public void ClearBlocks()
        {
            foreach (Transform block in blockHolder.transform)
            {
                Destroy(block.gameObject);
            }
        }

        public void SaveWall()
        {
            WallSave wallSave = new WallSave();

            foreach (Transform block in blockHolder.transform)
            {
                wallSave.blockPositions.Add(block.position);
            }

            string json = JsonUtility.ToJson(wallSave);
            Debug.Log(json);

            File.WriteAllText(Application.dataPath + "/Saves/wallsave.txt", json);
        }

        public void LoadWall()
        {
            //TODO
        }

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        ARRaycastManager m_RaycastManager;
        GameObject blockHolder;
    }

    public class WallSave
    {
        public List<Vector3> blockPositions = new List<Vector3>();
        //Save color.
        //etc.
    }
}

