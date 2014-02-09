using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Mothership.UI {

    public class UI : MonoBehaviour {

        public enum Mode {
            Galaxy,
            Sector,
            SolarSystem,
            Transition
        }

        private static UI m_instance;

        public Camera m_worldCamera;
        public GameObject m_markerParent;

        public GameObject m_scanButton;
        public GameObject m_zoomOutButton;

        private UICamera m_camera;
        private GameObject m_interestMarkerPrefab;

        private List<Star> m_lastScan;
        private List<GameObject> m_markers;

        private Mode m_currentMode;

        void Awake() {
            m_instance = this;
        }

        // Use this for initialization
        void Start() {
            m_camera = GetComponentInChildren<UICamera>();
            m_interestMarkerPrefab = Resources.Load("UI/Interest Marker") as GameObject;

            m_markers = new List<GameObject>();

            SectorCamera.OnViewChange += UpdateView;
        }

        void OnDestroy() {
            SectorCamera.OnViewChange -= UpdateView;
        }

        private void ZoomOut() {
            if (m_currentMode == Mode.SolarSystem) {
                Galaxy.ExitSystem();
            } else if (m_currentMode == Mode.Sector) {
                Galaxy.ExitSector();
            }
        }

        private void InitiateScan() {
            ClearMarkers();

            m_lastScan = Galaxy.SearchInterests();
            GameObject marker;

            for (int i = 0; i < m_lastScan.Count; i++) {
                marker = NGUITools.AddChild(m_markerParent, m_interestMarkerPrefab);
                m_markers.Add(marker);

                marker.GetComponent<UIButtonMessage>().target = gameObject;
            }

            UpdateView(Vector3.zero, new Rect());
        }

        public void UpdateView(Vector3 pos, Rect view) {
            Vector3 viewport;

            for (int i = 0; i < m_markers.Count; i++) {
                viewport = m_worldCamera.camera.WorldToViewportPoint(m_lastScan[i].transform.position);
                //Debug.Log("Viewport >> " + viewport);

                viewport.x *= m_camera.camera.pixelWidth;
                viewport.y *= m_camera.camera.pixelHeight;
                viewport.z = 0;
                //Debug.Log("  Screen >> " + viewport + " (" + m_camera.camera.pixelWidth + ", " + m_camera.camera.pixelHeight + ")");

                m_markers[i].transform.localPosition = viewport;
            }
        }

        private void ClearMarkers() {
            for (int i = 0; i < m_markers.Count; i++) {
                Destroy(m_markers[i].gameObject);
            }

            m_markers.Clear();
        }

        private void OnInterestSelected(GameObject selected) {
            Galaxy.ShowSolarSystem(m_lastScan[m_markers.IndexOf(selected)]);
            ClearMarkers();
        }

        public static void SetMode(Mode mode) {
            m_instance._SetMode(mode);
        }

        private void _SetMode(Mode mode) {
            m_currentMode = mode;

            switch (mode) {
                case Mode.Galaxy:
                    NGUITools.SetActive(m_scanButton, false);
                    NGUITools.SetActive(m_zoomOutButton, false);
                    break;

                case Mode.Sector:
                    NGUITools.SetActive(m_scanButton, true);
                    NGUITools.SetActive(m_zoomOutButton, true);

                    m_zoomOutButton.GetComponentInChildren<UILabel>().text = "Galaxy Map";
                    break;

                case Mode.SolarSystem:
                    NGUITools.SetActive(m_scanButton, false);
                    NGUITools.SetActive(m_zoomOutButton, true);

                    m_zoomOutButton.GetComponentInChildren<UILabel>().text = "Region";
                    break;

                case Mode.Transition:
                    NGUITools.SetActive(m_scanButton, false);
                    NGUITools.SetActive(m_zoomOutButton, false);
                    break;
            }
        }
    }

}