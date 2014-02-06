using UnityEngine;
using System.Collections;

public abstract class SystemBody {
    public float MajorAxis;
    public float MinorAxis;
    protected int Seed;

    public SystemBody(int iSeed) {
        Seed = iSeed;
    }

    /// <summary>
    /// Create a GameObject according to the body's type
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public abstract SolarSystemBody Create(Transform parent);
}

public class SolarSystem : MonoBehaviour {
    private Transform m_transform;

    /// <summary>Unity units per AU in solar system space</summary>
    public const float UNIT_TO_AU = 5f;

    public void Setup(Star star) {
        m_transform = transform;
        SystemBody[] bodies = star.GetBodies();

        for (int i = 0; i < bodies.Length; i++) {
            bodies[i].Create(m_transform);
        }

        // Create the star
        GameObject starGO = Instantiate(Resources.Load("Star")) as GameObject;
        starGO.transform.parent = m_transform;
        starGO.GetComponent<Star>().Setup(star.Data);
        starGO.transform.localPosition = Vector3.zero;
        starGO.transform.localScale = new Vector3(star.Radius * UNIT_TO_AU * 100, star.Radius * UNIT_TO_AU * 100, 1);
    }
}
