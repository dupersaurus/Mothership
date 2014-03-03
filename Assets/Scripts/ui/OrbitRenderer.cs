using UnityEngine;
using System.Collections;

public struct OrbitParams {
    /// <summary>The semi-major axis, in AU</summary>
    public float SemiMajorAxis;

    /// <summary>The eccentricity. Minor axis = major * sqrt(1 - e^2)</summary>
    public float Eccentricity;

    /// <summary>Rotation of the major axis around Vector3.right (1,0,0)</summary>
    public float Longitude;

    public OrbitParams(float fRadius, float fEccentricity, float fLongitude) {
        Eccentricity = fEccentricity;
        SemiMajorAxis = fRadius;
        Longitude = fLongitude;
    }
}

[RequireComponent(typeof(LineRenderer))]
public class OrbitRenderer : MonoBehaviour {
    private LineRenderer m_renderer;
    private Transform m_transform;

    /// <summary>
    /// Draw a circle
    /// </summary>
    /// <param name="center"></param>
    /// <param name="fRadius"></param>
    /// <param name="fLineWidth"></param>
    public void DrawCircle(Vector3 center, float fRadius, float fLineWidth) {
        if (m_renderer == null) {
            m_renderer = GetComponent<LineRenderer>();
        }

        if (m_transform == null) {
            m_transform = transform;
        }
        
        int iSteps = 120;
        float fIncrement = (Mathf.PI * 2) / (float)iSteps;

        m_transform.localPosition = center;

        m_renderer.SetVertexCount(iSteps + 1);
        m_renderer.SetWidth(fLineWidth, fLineWidth);

        for (int i = 0; i < iSteps; i++) {
            m_renderer.SetPosition(i, new Vector3(Mathf.Cos(fIncrement * (float)i) * fRadius, Mathf.Sin(fIncrement * (float)i) * fRadius, 0));
        }

        m_renderer.SetPosition(iSteps, new Vector3(fRadius, 0, 0));
    }

    /// <summary>
    /// Draws an orbit
    /// </summary>
    /// <param name="orbit"></param>
    /// <param name="fLineWidth"></param>
    public void DrawOrbit(OrbitParams orbit, float fLineWidth) {
        if (m_renderer == null) {
            m_renderer = GetComponent<LineRenderer>();
        }

        if (m_transform == null) {
            m_transform = transform;
        }

        int iSteps = 120;
        float fIncrement = (Mathf.PI * 2) / (float)iSteps;
        float fRadius = orbit.SemiMajorAxis * SolarSystem.UNIT_PER_AU;

        m_renderer.SetVertexCount(iSteps + 1);
        m_renderer.SetWidth(fLineWidth, fLineWidth);

        for (int i = 0; i < iSteps; i++) {
            m_renderer.SetPosition(i, new Vector3(Mathf.Cos(fIncrement * (float)i) * fRadius, Mathf.Sin(fIncrement * (float)i) * fRadius, 0));
        }

        m_renderer.SetPosition(iSteps, new Vector3(fRadius, 0, 0));
    }

    public void SetLineWidth(float fStart, float fEnd) {
        if (m_renderer == null) {
            m_renderer = GetComponent<LineRenderer>();
        }

        m_renderer.SetWidth(fStart, fEnd);
    }

    public void SetColors(Color start, Color end) {
        if (m_renderer == null) {
            m_renderer = GetComponent<LineRenderer>();
        }

        m_renderer.SetColors(start, end);
    }

    public static OrbitRenderer Spawn(Transform parent, string sPrefab = "Orbit Renderer") {
        Object prefab = Resources.Load(sPrefab);
        OrbitRenderer renderer = (Instantiate(prefab) as GameObject).GetComponent<OrbitRenderer>();
        renderer.transform.parent = parent;

        return renderer;
    }
}
