using UnityEngine;
using System.Collections;

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

    public void SetLineWidth(float fStart, float fEnd) {
        if (m_renderer == null) {
            m_renderer = GetComponent<LineRenderer>();
        }

        m_renderer.SetWidth(fStart, fEnd);
    }
}
