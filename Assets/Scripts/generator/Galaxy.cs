using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Galaxy : MonoBehaviour {
    private static Galaxy m_instance;
    
    private Transform m_transform;

    private List<SectorGenerator> m_sectors;
    private List<GameObject> m_interestMarkers;

    private Rect m_lastView;

    private Star m_showSystemOf;

    void Awake() {
        m_instance = this;
    }

    void OnDestroy() {
        m_instance = null;
    }

	// Use this for initialization
	void Start () {
        SectorGenerator sector;
        Object prefab = Resources.Load("Sector");

        m_transform = transform;
        m_sectors = new List<SectorGenerator>();
        m_interestMarkers = new List<GameObject>();

        /*for (int i = -1; i < 2; i++) {
            for (int j = -1; j < 2; j++) {
                sector = (Instantiate(prefab) as GameObject).GetComponent<SectorGenerator>();
                sector.transform.parent = m_transform;
                sector.Setup(i, j, 1);
                m_sectors.Add(sector);
            }
        }*/
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
                interests.AddRange(m_sectors[i].SearchInterests(Vector3.zero, 144, 1));
            }
        }

        // Create the markers
        /*GameObject marker;
        Object prefab = Resources.Load("Interest Marker");

        for (int i = 0; i < interests.Count; i++) {
            marker = Instantiate(prefab) as GameObject;
            marker.transform.parent = m_transform;
            marker.transform.localPosition = interests[i].LocalPosition;
        }*/

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

        Vector3 target = star.Position;
        target.z = -1;

        SectorCamera.MoveCameraTo(target, 2, BringInSolarSystem, SectorCamera.Instance.m_exitSectorToSystem);

        /*TweenPosition tween = TweenPosition.Begin(gameObject, 2, target);
        tween.eventReceiver = gameObject;
        tween.callWhenFinished = "BringInSolarSystem";*/
    }

    private void BringInSolarSystem() {
        Vector3 pos = m_showSystemOf.Position;
        SolarSystem solarSystem = (Instantiate(Resources.Load("Solar System")) as GameObject).GetComponent<SolarSystem>();
        solarSystem.transform.position = new Vector3(pos.x, pos.y, 200);
        solarSystem.Setup(m_showSystemOf);

        pos.z = 140;
        SectorCamera.MoveCameraTo(pos, 3, null, SectorCamera.Instance.m_enterSystemFromSector);

        NGUITools.SetActive(gameObject, false);
    }
}
