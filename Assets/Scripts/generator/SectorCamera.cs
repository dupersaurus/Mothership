using UnityEngine;
using System.Collections;

public class SectorCamera : MonoBehaviour {

    private static SectorCamera m_instance;

    public static SectorCamera Instance {
        get { return m_instance; }
    }

    private Camera m_camera;
    private Transform m_transform;

    /// <summary>Rectangle of the area currently on screen</summary>
    public Rect m_viewArea;

    public float m_panRate;
    public float m_zoomRate;
    public float m_closestCamZ;
	public float m_farthestCamZ;

    void Awake() {
        m_instance = this;
        m_camera = camera;
        m_transform = transform;
        m_viewArea = CalculateViewArea();
    }

    void OnDrawGizmos() {
        if (m_transform == null) {
            m_transform = transform;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(m_viewArea.center, new Vector3(m_viewArea.width, m_viewArea.height, 0.2f));
    }

    void Update() {
        Rect newView;
        bool bUpdateView = false;
        float fPanDelta = m_panRate * Time.deltaTime;
        Vector3 camPos = m_transform.position;

        // X pan
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            bUpdateView = true;
            camPos.x -= fPanDelta;
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            bUpdateView = true;
            camPos.x += fPanDelta;
        }

        // Y pan
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            bUpdateView = true;
            camPos.y += fPanDelta;
        } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            bUpdateView = true;
            camPos.y -= fPanDelta;
        }

        // Z pan
        float fScroll = Input.GetAxis("Mouse ScrollWheel");
		float fDiff = m_zoomRate;

        if (fScroll != 0 && (camPos.z < m_closestCamZ || fScroll < 0)) {
            /*camPos.z += fScroll * m_zoomRate * Time.deltaTime;

            if (fScroll > 0 && camPos.z >= m_closestCamZ) {
                camPos.z = m_closestCamZ;
            }*/

			// Approach but don't pass closest
			if (fScroll < 0) {
				/*fDiff = (camPos.z - m_closestCamZ) / 2;

				if (fDiff > m_zoomRate) {
					fDiff = m_zoomRate;
				}*/

				camPos.z += fScroll * fDiff * Time.deltaTime;

				if (camPos.z > m_closestCamZ) {
					camPos.z = m_closestCamZ;
				}
			}

			// Approach but don't pass farthest
			else {
				/*fDiff = (m_farthestCamZ - camPos.z) / 2;
				
				if (fDiff > m_zoomRate) {
					fDiff = m_zoomRate;
				}*/
				
				camPos.z += fScroll * fDiff * Time.deltaTime;
				
				if (camPos.z < m_farthestCamZ) {
					camPos.z = m_farthestCamZ;
				}
			}

            bUpdateView = true;
        }

        if (bUpdateView) {
            m_transform.position = camPos;
            newView = CalculateViewArea();

            // TODO send the view delta to the renderer

            m_viewArea = newView;
        }
    }

    /// <summary>
    /// Calculates the view area given the camera's current settings
    /// </summary>
    private Rect CalculateViewArea() {
        float fFOV = m_camera.fieldOfView * 0.5f;
        float fOppositeAngle = (90 - fFOV);   // 180 - 90 - FOV
        Vector3 camPos = m_transform.position;
        float fHeight;
        float fWidth;

        if (!m_camera.orthographic) {
            fHeight = ((Mathf.Abs(camPos.z) * Mathf.Sin(fFOV * Mathf.Deg2Rad)) / Mathf.Sin(fOppositeAngle * Mathf.Deg2Rad)) * 2;
        } else {
            fHeight = m_camera.orthographicSize * 2;
        }

        fWidth = fHeight * m_camera.aspect;

        return new Rect(camPos.x - fWidth * 0.5f, camPos.y + fHeight * 0.5f, fWidth, -fHeight);
    }

    public Rect GetViewArea() {
        return m_viewArea;
    }
}
