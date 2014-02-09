using UnityEngine;
using System.Collections;

public class PlanetInfo : SystemBody {
    private static Object m_prefab;

    public PlanetInfo(int iSeed)
        : base(iSeed) {

    }

    /// <summary>
    /// Create a GameObject according to the body's type
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public override SolarSystemBody Create(Transform parent) {
        Debug.Log("Create Planet");

        Planet planet;

        if (m_prefab == null) {
            m_prefab = Resources.Load("Planet");
        }

        planet = (GameObject.Instantiate(m_prefab) as GameObject).GetComponent<Planet>();
        planet.transform.parent = parent;
        planet.Setup(this);

        return planet;
    }
}

public class Planet : SolarSystemBody {
    private float m_fBaseSize;

    public virtual void Setup(SystemBody planet) {
        Debug.Log("Planet >> Setup >> " + planet.MajorAxis + ", " + planet.MinorAxis);
        base.Setup(planet);
        m_material.color = new Color(0.5f, 0.3f, 0.1f);

        m_fBaseSize = 0.3f;

        // Place somewhere on the orbit
        float fAngle = Random.value * Mathf.PI * 2;
        m_transform.localPosition = new Vector3(planet.MajorAxis * Mathf.Cos(fAngle), planet.MinorAxis * Mathf.Sin(fAngle), 0);
        m_transform.localScale = new Vector3(m_fBaseSize, m_fBaseSize, 1);
    }

    protected override void OnViewChange(Vector3 pos, Rect view) {
        base.OnViewChange(pos, view);

        float fWidth = ((190 - pos.z) / 190) * 1.75f + m_fBaseSize;
        fWidth = Mathf.Clamp(fWidth, m_fBaseSize, 3f);
        m_transform.localScale = new Vector3(fWidth, fWidth, 1);
    }
}
