using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI : MonoBehaviour {

    public Camera m_worldCamera;
    public GameObject m_markerParent;

    private UICamera m_camera;
    private GameObject m_interestMarkerPrefab;

    private List<Star> m_lastScan;
    private List<GameObject> m_markers;

	// Use this for initialization
	void Start () {
        m_camera = GetComponentInChildren<UICamera>();
        m_interestMarkerPrefab = Resources.Load("UI/Interest Marker") as GameObject;

        m_markers = new List<GameObject>();
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

        UpdateView();
    }

    public void UpdateView() {
        Vector3 viewport;

        for (int i = 0; i < m_markers.Count; i++) {
            viewport = m_worldCamera.camera.WorldToViewportPoint(m_lastScan[i].transform.position);
            //Debug.Log("Viewport >> " + viewport);

            viewport.x *= m_camera.camera.pixelWidth;
            viewport.y *= m_camera.camera.pixelHeight;
            //viewport.y += m_camera.camera.pixelHeight;
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
}
