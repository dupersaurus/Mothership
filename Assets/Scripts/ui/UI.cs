using UnityEngine;
using System;
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
        public GameObject m_popupParent;

        public GameObject m_scanButton;
        public GameObject m_zoomOutButton;

        public UILabel m_timeElapsedLabel;

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

        /// <summary>
        /// Keep up with the elapsed time
        /// </summary>
        void FixedUpdate() {
            TimeSpan span = TimeSpan.FromSeconds(TimeManager.ElapsedTime);
            string sText = string.Format("{0}y {1}d {2}h {3}m {4}s\nTime Scale {5}x", Mathf.FloorToInt((float)span.Days / 365f), (span.Days % 365).ToString("D3"), span.Hours.ToString("D2"), span.Minutes.ToString("D2"), span.Seconds.ToString("D2"), TimeManager.TimeScale);

            double fJumpTime = Ship.JumpTimeRemaining;

            if (fJumpTime != 0) {
                span = TimeSpan.FromSeconds(fJumpTime);
                sText = string.Format("{0}\nJump {1}y {2}d {3}h {4}m {5}s", sText, Mathf.FloorToInt((float)span.Days / 365f), (span.Days % 365).ToString("D3"), span.Hours.ToString("D2"), span.Minutes.ToString("D2"), span.Seconds.ToString("D2"));
            }

            m_timeElapsedLabel.text = sText;
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
            
            if (Ship.JumpDestination != null && (m_lastScan == null || m_lastScan.Count == 0)) {
                viewport = m_worldCamera.camera.WorldToViewportPoint(Ship.JumpDestination.transform.position);
                //Debug.Log("Viewport >> " + viewport);

                viewport.x *= m_camera.camera.pixelWidth;
                viewport.y *= m_camera.camera.pixelHeight;
                viewport.z = 0;
                //Debug.Log("  Screen >> " + viewport + " (" + m_camera.camera.pixelWidth + ", " + m_camera.camera.pixelHeight + ")");

                m_markers[0].transform.localPosition = viewport;
            } else {
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
        }

        private void ClearMarkers() {
            if (m_markers != null) {
                for (int i = 0; i < m_markers.Count; i++) {
                    Destroy(m_markers[i].gameObject);
                }

                m_markers.Clear();
            }

            if (m_lastScan != null) {
                m_lastScan.Clear();
            }
        }

        private void OnInterestSelected(GameObject selected) {
            //Galaxy.JumpToSolarSystem(m_lastScan[m_markers.IndexOf(selected)]);
            //ClearMarkers();

            GameObject popup = NGUITools.AddChild(m_popupParent, Resources.Load("UI/Stellar Info") as GameObject);
            popup.GetComponent<StellarInfoPopup>().Setup(m_lastScan[m_markers.IndexOf(selected)]);
        }

        public static void SetJumpDestination() {
            m_instance._SetJumpDestination();
        }

        private void _SetJumpDestination() {
            ClearMarkers();

            GameObject marker = NGUITools.AddChild(m_markerParent, m_interestMarkerPrefab);
            marker.GetComponentInChildren<UIWidget>().color = Color.cyan;
            m_markers.Add(marker);

            UpdateView(Vector3.zero, new Rect());
        }

        public static void SetMode(Mode mode) {
            m_instance._SetMode(mode);
        }

        private void _SetMode(Mode mode) {
            m_currentMode = mode;
            ClearMarkers();

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