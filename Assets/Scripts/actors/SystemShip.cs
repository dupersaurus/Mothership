using UnityEngine;
using System.Collections;

public class SystemShip : MonoBehaviour {
    private Transform m_transform;

    private OrbitRenderer m_orbitRenderer;
    private OrbitParams m_orbit;

    private ShipInfo m_info;

    /// <summary>
    /// Setup the ship
    /// </summary>
    /// <param name="info">Info from GalacticShip</param>
    /// <param name="fOrbitRadius">Distance to being orbit at, in AU</param>
    public void Setup(ShipInfo info, float fOrbitRadius) {
        m_transform = transform;
        m_info = info;
        m_orbit = new OrbitParams(fOrbitRadius, 1, 0);

        m_orbitRenderer = OrbitRenderer.Spawn(m_transform.parent);
        m_orbitRenderer.name = "Player Orbit";
        m_orbitRenderer.transform.localPosition = Vector3.zero;
        m_orbitRenderer.DrawOrbit(m_orbit, 0.05f);
    }
}
