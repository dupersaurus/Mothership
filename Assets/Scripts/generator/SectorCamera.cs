using UnityEngine;
using System.Collections;

public class SectorCamera : MonoBehaviour {
    public delegate void cbOnCameraMoveFinished();
    private cbOnCameraMoveFinished OnCameraMoveFinished;

    public delegate void cbOnViewChange(Vector3 pos, Rect view);
    public static event cbOnViewChange OnViewChange;

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

    public AnimationCurve m_enterSectorFromSystem;
    public AnimationCurve m_exitSectorToSystem;
    public AnimationCurve m_enterSystemFromSector;
    public AnimationCurve m_exitSystemToSector;

    /// <summary>If the camera is automatically moving to someplace</summary>
    private bool m_bIsTweening = false;

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
        bool bUpdateView = false;
        Rect newView;
        Vector3 camPos = m_transform.position;

        if (m_bIsTweening) {
            bUpdateView = true;
        } else {

            float fPanDelta = m_panRate * Time.deltaTime;

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

            if (fScroll != 0) {

                // Approach but don't pass closest
                if (fScroll < 0) {
                    /*fDiff = (camPos.z - m_closestCamZ) / 2;

                    if (fDiff > m_zoomRate) {
                        fDiff = m_zoomRate;
                    }*/
                }

                // Approach but don't pass farthest
                else {
                    /*fDiff = (m_farthestCamZ - camPos.z) / 2;
				
                    if (fDiff > m_zoomRate) {
                        fDiff = m_zoomRate;
                    }*/
                }

                camPos.z += fScroll * fDiff * Time.deltaTime;

                /*if (camPos.z > m_closestCamZ) {
                    camPos.z = m_closestCamZ;
                }*/

                /*if (camPos.z < m_farthestCamZ) {
                    camPos.z = m_farthestCamZ;
                }*/

                bUpdateView = true;
            }
        }

        if (bUpdateView) {
            m_transform.position = camPos;
            newView = CalculateViewArea();

            if (OnViewChange != null) {
                OnViewChange(camPos, newView);
            }

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

    public static void MoveCameraTo(Vector3 pos, float fTime, cbOnCameraMoveFinished callback, AnimationCurve curve = null) {
        m_instance._MoveCameraTo(pos, fTime, callback, curve);
    }

    private void _MoveCameraTo(Vector3 pos, float fTime, cbOnCameraMoveFinished callback, AnimationCurve curve = null) {
        m_bIsTweening = true;

        TweenPosition tween = TweenPosition.Begin(gameObject, fTime, pos);
        tween.eventReceiver = gameObject;
        tween.callWhenFinished = "TweenFinished";

        if (curve != null) {
            tween.animationCurve = curve;
        } else {
            tween.method = UITweener.Method.EaseInOut;
        }

        OnCameraMoveFinished = callback;
    }

    private void TweenFinished() {
        m_bIsTweening = false;

        if (OnCameraMoveFinished != null) {
            OnCameraMoveFinished();
            OnCameraMoveFinished = null;
        }
    }
}