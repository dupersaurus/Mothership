using UnityEngine;
using System.Collections;

public class SolarSystemBody : MonoBehaviour {

    protected Transform m_transform;
    protected Material m_material;

    protected OrbitRenderer m_orbitRing;
    protected bool m_bShowOrbitRing = true;

    protected void OnDestroy() {
        SectorCamera.OnViewChange -= OnViewChange;
    }

    public virtual void Setup(SystemBody body) {
        m_transform = transform;
        m_material = GetComponent<MeshRenderer>().material;

        if (m_bShowOrbitRing) {
            Object prefab = Resources.Load("Orbit Renderer");
            m_orbitRing = (Instantiate(prefab) as GameObject).GetComponent<OrbitRenderer>();
            m_orbitRing.transform.parent = m_transform.parent;
            m_orbitRing.DrawCircle(Vector3.zero, body.MajorAxis, 0.03f);
        } 
        
        SectorCamera.OnViewChange += OnViewChange;
    }

    protected virtual void OnViewChange(Vector3 pos, Rect view) {
        if (m_bShowOrbitRing) {
            float fWidth = ((190 - pos.z) / 190) * 0.25f + 0.05f;

            fWidth = Mathf.Clamp(fWidth, 0.03f, 1);
            m_orbitRing.SetLineWidth(fWidth, fWidth);
        }
    }
}
