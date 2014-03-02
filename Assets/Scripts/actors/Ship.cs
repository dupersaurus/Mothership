using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {

    /// <summary>TODO for player only</summary>
    private static Ship m_instance;

    private Transform m_transform;

    /// <summary>Icon to display when camera is at a distance</summary>
    public GameObject m_distanceIcon;

    public GameObject m_beacon;
    private bool m_bFlashBeacon = true;

	/// <summary>
	/// Distance, in light years, FTL travel with current fuel load
	/// </summary>
	/// <value>The FTL range.</value>
	public float FTLRange {
		get { return 0; }
	}

	/// <summary>
	/// Time that the ship can stay in continuous FTL, in game seconds
	/// </summary>
	/// <value>The FTL time.</value>
	public float FTLTime {
		get { return 0; }
	}

	/// <summary>
	/// FTL speed, ly/sec (game)
	/// </summary>
	/// <value>The FTP speed.</value>
	public float FTPSpeed {
		get { return 0; }
	}

    void Awake() {
        m_instance = this;
    }

    // Use this for initialization
    void Start() {
        m_transform = transform;
        SectorCamera.OnViewChange += OnViewChange;
		TimeManager.OnWorldUpdate += OnWorldUpdate;

        OnViewChange(SectorCamera.Instance.transform.position, new Rect());
        gameObject.SetActive(false);
    }

    void OnDestroy() {
        SectorCamera.OnViewChange -= OnViewChange;
		TimeManager.OnWorldUpdate -= OnWorldUpdate;
    }

	private void OnWorldUpdate(float fDelta) {

	}

    private void OnViewChange(Vector3 pos, Rect rect) {
            
        if (m_transform.position.z - pos.z >= 5) {
            if (!m_bFlashBeacon) {
                NGUITools.SetActive(m_beacon, true);
                m_bFlashBeacon = true;
            }
        } else {
            if (m_bFlashBeacon) {
                NGUITools.SetActive(m_beacon, false);
                m_bFlashBeacon = false;
            }
        }

    }

    public static void SetEnabled(bool bEnable) {
        m_instance.gameObject.SetActive(bEnable);
    }

    public static void MoveTo(Vector3 pos) {
        m_instance.m_transform.position = pos;
    }
}
