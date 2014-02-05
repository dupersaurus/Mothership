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
        planet.Setup(this);

        return planet;
    }
}

public class Planet : SolarSystemBody {
    public virtual void Setup(SystemBody planet) {
        Debug.Log("Planet >> Setup >> " + planet.MajorAxis + ", " + planet.MinorAxis);
        base.Setup(planet);
        m_material.color = new Color(0, 0.3f, 0.7f);

        // Place somewhere on the orbit
        float fAngle = Random.value * Mathf.PI * 2;
        m_transform.position = new Vector3(planet.MajorAxis * Mathf.Cos(fAngle), planet.MinorAxis * Mathf.Sin(fAngle), 0);
        m_transform.localScale = new Vector3(0.3f, 0.3f, 1);
    }
}
