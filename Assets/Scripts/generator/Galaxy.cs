using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mothership.UI;

public class Galaxy : MonoBehaviour {
    private static Galaxy m_instance;
    
    private Transform m_transform;

    private List<SectorGenerator> m_sectors;
    private List<GameObject> m_interestMarkers;

    private Rect m_lastView;

    private Star m_showSystemOf;
    private SolarSystem m_currentSystem;

    void Awake() {
        m_instance = this;
    }

    void OnDestroy() {
        m_instance = null;
    }

	// Use this for initialization
	void Start () {

        m_transform = transform;
        m_sectors = new List<SectorGenerator>();
        m_interestMarkers = new List<GameObject>();
	}

    /// <summary>
    /// Setup the sectors around a given sector coordinate
    /// </summary>
    /// <param name="iX"></param>
    /// <param name="iY"></param>
    /// <returns>Position to zoom the camera to for the given sectors</returns>
    public static Vector3 SetupSectors(int iX, int iY) {
        return m_instance._SetupSectors(iX, iY);
    }

    /// <summary>
    /// Setup the sectors around a given sector coordinate
    /// </summary>
    /// <param name="iX"></param>
    /// <param name="iY"></param>
    /// <returns>Position to zoom the camera to for the given sectors</returns>
    private Vector3 _SetupSectors(int iX, int iY) {
        for (int i = 0; i < m_sectors.Count; i++) {
            Destroy(m_sectors[i].gameObject);
        }

        m_sectors.Clear();
        
        SectorGenerator sector;
        Object prefab = Resources.Load("Sector");
        Vector3 pos = Vector3.zero;
        GalaxySectorData data;

        //for (int i = 0; i < 1; i++) {
        //    for (int j = 0; j < 1; j++) {
        for (int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {
                data = GalaxyMap.GetSectorData(iX + i, iY + j);

                if (data == null) {
                    continue;
                }

                sector = (Instantiate(prefab) as GameObject).GetComponent<SectorGenerator>();
                sector.transform.parent = m_transform;
                sector.Setup(data, 1);
                m_sectors.Add(sector);

                if (i == 0 && j == 0) {
                    pos = sector.transform.position;
                    pos.z = -10;
                }
            }
        }

        UI.SetMode(UI.Mode.Sector);

        // TODO real ship handling
        Ship.SetEnabled(true);
        Ship.MoveTo(new Vector3(pos.x, pos.y, -0.05f));

        return pos;
    }

    public void ViewChange(Rect view) {
        m_lastView = view;
    }

    public static List<Star> SearchInterests() {
        return m_instance._SearchInterests();
    }

    private List<Star> _SearchInterests() {
        List<Star> interests = new List<Star>();
        Rect view = SectorCamera.Instance.GetViewArea();

        for (int i = 0; i < m_sectors.Count; i++) {
            Debug.Log(m_sectors[i].name + " >> " + view + " vs " + m_sectors[i].Bounds);

            if (RectIntersectTest(view, m_sectors[i].Bounds)) {
                Debug.Log("^^^ INTERSECT");
                interests.AddRange(m_sectors[i].SearchInterests(view.center, 144, 1));
            }
        }

        Debug.Log("Interest Count >> " + interests.Count);
        return interests;
    }

    private bool RectIntersectTest(Rect a, Rect b) {
        return (Mathf.Abs(a.center.x - b.center.x) * 2 < (a.width + b.width)) && (Mathf.Abs(a.center.y - b.center.y) * 2 < (-a.height + -b.height));
    }

    public static void ShowSolarSystem(Star star) {
        m_instance._ShowSolarSystem(star);
    }

    private void _ShowSolarSystem(Star star) {
        m_showSystemOf = star;

        Vector3 pos = m_showSystemOf.Position;
        m_currentSystem = (Instantiate(Resources.Load("Solar System")) as GameObject).GetComponent<SolarSystem>();
        m_currentSystem.transform.position = new Vector3(pos.x, pos.y, 200);
        m_currentSystem.Setup(m_showSystemOf);

        Vector3 target = star.Position;
        target.z = -1;

        SectorCamera.MoveCameraTo(target, 2, BringInSolarSystem, SectorCamera.Instance.m_exitSectorToSystem);
        UI.SetMode(UI.Mode.Transition);

        /*TweenPosition tween = TweenPosition.Begin(gameObject, 2, target);
        tween.eventReceiver = gameObject;
        tween.callWhenFinished = "BringInSolarSystem";*/
    }

    private void BringInSolarSystem() {
        Vector3 pos = m_showSystemOf.Position;
        pos.z = 140;
        SectorCamera.MoveCameraTo(pos, 3, OnArrivedSolarSystem, SectorCamera.Instance.m_enterSystemFromSector);

        gameObject.SetActive(false);
    }

    private void OnArrivedSolarSystem() {
        UI.SetMode(UI.Mode.SolarSystem);
    }

    public static void ExitSystem() {
        m_instance._ExitSystem();
    }

    private void _ExitSystem() {
        UI.SetMode(UI.Mode.Transition);

        Vector3 pos = m_showSystemOf.Position;
        pos.z = -1;
        SectorCamera.MoveCameraTo(pos, 3, OnExitSolarSystem, SectorCamera.Instance.m_exitSystemToSector);
    }

    private void OnExitSolarSystem() {
        gameObject.SetActive(true);

        SectorCamera.ZoomCamera(-10, 2, OnArrivedExitSolarSystem, SectorCamera.Instance.m_enterSectorFromSystem);
    }

    private void OnArrivedExitSolarSystem() {
        Destroy(m_currentSystem.gameObject);
        UI.SetMode(UI.Mode.Sector);
    }
}
