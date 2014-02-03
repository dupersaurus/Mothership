using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SectorGenerator : MonoBehaviour {

    private Transform m_transform;

    private GameObject m_starPrefab;

    public int m_testSeed;
    public int m_testX;
    public int m_testY;

    public Vector2 m_sectorSize;
    public float m_sampleIncrement = 0.1f;

    private List<Star> m_stars;

    void Start() {
        Setup(m_testX, m_testY, m_testSeed);
    }

    /// <summary>
    /// Construct the sector
    /// </summary>
    /// <param name="area">Rectangle defining the sector's bounds for perlin generation</param>
    /// <param name="iSeed">The sector's seed. Everything random in the sector starts with this.</param>
    public void Setup(int iSectorX, int iSectorY, int iSeed) {
        m_starPrefab = Resources.Load("Star") as GameObject;
        m_stars = new List<Star>();
        m_transform = transform;

        Random.seed = iSeed;
        float fValue;
        GameObject star;

        // Center of the sector in real space
        float fXZero = m_sectorSize.x * iSectorX;
        float fYZero = m_sectorSize.y * iSectorY;
        m_transform.position = new Vector3(fXZero, fYZero);

        Rect area = new Rect(fXZero - m_sectorSize.x * 0.5f, fYZero + m_sectorSize.y * 0.5f, m_sectorSize.x, -m_sectorSize.y);

        float xLeft = Mathf.Floor(area.xMin);
        float xRight = Mathf.Ceil(area.xMax);
        float yTop = Mathf.Ceil(area.yMin);
        float yBottom = Mathf.Floor(area.yMax);

        for (float fSampleX = xLeft; fSampleX <= xRight; fSampleX += m_sampleIncrement) {
            for (float fSampleY = yTop; fSampleY >= yBottom; fSampleY -= m_sampleIncrement) {
                fValue = Mathf.PerlinNoise(fSampleX, fSampleY) * Random.value;

                if (Random.value * fValue > 0.45f) {
                    star = Instantiate(m_starPrefab) as GameObject;
                    star.transform.parent = transform;
                    star.transform.localPosition = new Vector3(fSampleX + MakeNoise() - fXZero, fSampleY + MakeNoise() - fYZero);
                    m_stars.Add(star.GetComponent<Star>());
                }
            }
        }

        for (int i = 0; i < m_stars.Count; i++) {
            m_stars[i].Setup(Random.Range(0, int.MaxValue));
        }
    }

    private float MakeNoise() {
        return m_sampleIncrement * Random.value - m_sampleIncrement * 0.5f;
    }
}
