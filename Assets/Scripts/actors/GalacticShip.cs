using UnityEngine;
using System.Collections;

public class GalacticShip : MonoBehaviour {

    /// <summary>TODO for player only</summary>
    private static GalacticShip m_instance;

    public static GalacticShip Instance {
        get { return m_instance; }
    }

    private Transform m_transform;

    /// <summary>Icon to display when camera is at a distance</summary>
    public GameObject m_distanceIcon;

    public GameObject m_beacon;
    private bool m_bFlashBeacon = true;

    public OrbitRenderer m_fuelRangeIndicator;

    private ShipInfo m_info;

	/// <summary>
	/// Distance, in light years, FTL travel with current fuel load. This is the fuel load, divided by the ship's usage when running at FTL, multiplied by speed.
	/// </summary>
	/// <value>The FTL range.</value>
	public float FTLRange {
		get { return m_info.FuelLoad / FTLFuelUseage * FTLSpeed; }
	}

	/// <summary>
	/// Time that the ship can stay in continuous FTL, in game seconds
	/// </summary>
	/// <value>The FTL time.</value>
	private float FTLTime {
        get { return m_info.FTLTime; }
	}

	/// <summary>
	/// FTL speed, ly/sec (game)
	/// </summary>
	/// <value>The FTL speed.</value>
	private float FTLSpeed {
        get { return m_info.FTLSpeed; }
	}

    /// <summary>
    /// Fuel usage while in FTL, units/sec
    /// </summary>
    private float FTLFuelUseage {
        get { return m_info.FTLFuelUseage; }
    }

    /// <summary>The ship's current destination</summary>
    private Star m_currentDestination;

    /// <summary>Whether the ship is in warp</summary>
    private bool m_bInJump = false;

    /// <summary>Distance, in ly, remaining in the current jump</summary>
    private float m_fDistanceRemaining;

    void Awake() {
        m_instance = this;
        m_info = new ShipInfo();
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

    /// <summary>
    /// On TimeManager update
    /// </summary>
    /// <param name="fDelta"></param>
	private void OnWorldUpdate(float fDelta) {
        if (!m_bInJump) {
            return;
        }

        float fStep = FTLSpeed * SectorGenerator.UNIT_PER_LY * fDelta;
        m_info.UseFuel(FTLFuelUseage * fDelta);

        Vector3 heading = m_currentDestination.transform.position - m_transform.position;
        m_fDistanceRemaining = heading.magnitude / SectorGenerator.UNIT_PER_LY;

        if (fStep * fStep < heading.sqrMagnitude) {
            heading.Normalize();
            heading *= fStep;
            m_transform.rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, heading), Vector3.forward);
        } 
        
        // Finish the jump
        else {
            m_fDistanceRemaining = 0;
            _ExitFTL();
        }

        m_transform.position += heading;
        DrawFuelRange();
	}

    /// <summary>
    /// Time remaining, in game seconds, in the current FTL jump
    /// </summary>
    public static double JumpTimeRemaining {
        get {
            if (!m_instance.m_bInJump) {
                return 0;
            } else {
                return m_instance.m_fDistanceRemaining / m_instance.FTLSpeed;
            }
        }
    }

    public static Star JumpDestination {
        get { return m_instance.m_currentDestination; }
    }

    /// <summary>
    /// On camera view change
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rect"></param>
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

        float fLineWidth = (pos.z / -10) * 0.02f;
        m_fuelRangeIndicator.SetLineWidth(fLineWidth, fLineWidth);
    }

    public static void SetEnabled(bool bEnable) {
        m_instance.gameObject.SetActive(bEnable);
    }

    /// <summary>
    /// Moves the ship to a location
    /// </summary>
    /// <param name="pos"></param>
    public static void MoveTo(Vector3 pos) {
        m_instance.m_transform.position = pos;
        m_instance.DrawFuelRange();
    }

    /// <summary>
    /// Enter FTL to a specific star system
    /// </summary>
    /// <param name="target"></param>
    public static void EnterFTL(Star target) {
        m_instance._EnterFTL(target);
    }

    private void _EnterFTL(Star target) {
        m_currentDestination = target;
        m_bInJump = true;
        //m_fuelRangeIndicator.gameObject.SetActive(false);
        TimeManager.TimeScale = 100000;
        Mothership.UI.UI.SetJumpDestination();
    }

    /// <summary>
    /// Exit the current jump
    /// </summary>
    private void _ExitFTL() {
        m_bInJump = false;
        Galaxy.ShowSolarSystem(m_info, m_currentDestination);
        TimeManager.Pause();
    }

    /// <summary>
    /// Draw the ship's fuel range
    /// </summary>
    private void DrawFuelRange() {
        m_fuelRangeIndicator.gameObject.SetActive(true);
        m_fuelRangeIndicator.DrawCircle(Vector3.zero, FTLRange * SectorGenerator.UNIT_PER_LY, 0.02f);
    }

    /// <summary>
    /// Returns parameters about the distance from the ship to a target
    /// </summary>
    /// <param name="target">The target to calculate</param>
    /// <param name="fDistance">Distance to the target, in ly</param>
    /// <param name="fTime">Time to the target at current jump speed, in seconds</param>
    public static void GetParamsTo(Star target, out float fDistance, out double fTime) {

        // Distance
        Vector3 heading = target.transform.position - m_instance.m_transform.position;
        fDistance = heading.magnitude / SectorGenerator.UNIT_PER_LY;

        // Time
        fTime = fDistance / m_instance.FTLSpeed;
    }
}
