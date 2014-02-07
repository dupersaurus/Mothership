using UnityEngine;
using System.Collections;

public class SolarSystemBody : MonoBehaviour {

    protected Transform m_transform;
    protected Material m_material;

    protected OrbitRenderer m_orbitRing;
    protected bool m_bShowOrbitRing = true;

    public virtual void Setup(SystemBody body) {
        m_transform = transform;
        m_material = GetComponent<MeshRenderer>().material;

        if (m_bShowOrbitRing) {
            Object prefab = Resources.Load("Orbit Renderer");
            m_orbitRing = (Instantiate(prefab) as GameObject).GetComponent<OrbitRenderer>();
            m_orbitRing.transform.parent = m_transform.parent;
            m_orbitRing.DrawCircle(Vector3.zero, body.MajorAxis, 0.03f);
        }
    }
}
