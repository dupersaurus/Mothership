using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SectorGenerator : MonoBehaviour {

	public static float LY_PER_UNIT = -1;

    private Transform m_transform;

    private GameObject m_starPrefab;

    public int m_testSeed;
    public int m_testX;
    public int m_testY;

    public Vector2 m_sectorSize;
    public float m_sampleIncrement = 0.1f;

    public GameObject m_background;

    private List<Star> m_stars;

    /// <summary>The bounds of the sector, in galaxy space</summary>
    private Rect m_bounds;

    /// <summary>The bounds of the sector, in galaxy space</summary>
    public Rect Bounds {
        get { return m_bounds; }
    }

    void Start() {
		if (LY_PER_UNIT == -1) {
			LY_PER_UNIT = 2000f / m_sectorSize.x;
		}

        //Setup(m_testX, m_testY, m_testSeed);
    }

    /// <summary>
    /// Construct the sector
    /// </summary>
    /// <param name="area">Rectangle defining the sector's bounds for perlin generation</param>
    /// <param name="iSeed">The sector's seed. Everything random in the sector starts with this.</param>
    public void Setup(GalaxySectorData data, int iSeed) {

		int iSectorX = data.X;
		int iSectorY = data.Y;
        name = "Sector (" + iSectorX + ", " + iSectorY + ")";

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
        m_bounds = new Rect(fXZero - m_sectorSize.x * 0.5f, fYZero + m_sectorSize.y * 0.5f, m_sectorSize.x, -m_sectorSize.y);

		Rect area = m_bounds; //new Rect(fXZero - m_sectorSize.x * 0.5f, fYZero + m_sectorSize.y * 0.5f, m_sectorSize.x, -m_sectorSize.y);

        float xLeft = Mathf.Floor(area.xMin);
        float xRight = Mathf.Ceil(area.xMax);
        float yTop = Mathf.Ceil(area.yMin);
        float yBottom = Mathf.Floor(area.yMax);
        float fDensity = data.GetSectorDensity();
        float fTempDensity;
		float fAverage = 0;
        float fAverageDensity = 0;
		int iTicks = 0;

        for (float fSampleX = xLeft; fSampleX <= xRight; fSampleX += m_sampleIncrement) {
            for (float fSampleY = yTop; fSampleY >= yBottom; fSampleY -= m_sampleIncrement) {
                
                // Draw from perlin, then flatten range
                //fValue = Mathf.PerlinNoise(fSampleX, fSampleY);// *Random.value; //*data.GetSectorDensity();
                //fValue = fValue * 0.8f + 0.1f;
                fValue = Random.value;

                fTempDensity = fDensity * Random.value * 0.25f;// *Random.value;
				fAverage += fValue;
                fAverageDensity += fTempDensity;
				iTicks++;

                if (fValue < fTempDensity) {
                    star = Instantiate(m_starPrefab) as GameObject;
                    star.transform.parent = transform;
                    star.transform.localPosition = new Vector3(fSampleX + MakeNoise() - fXZero, fSampleY + MakeNoise() - fYZero);
                    m_stars.Add(star.GetComponent<Star>());
                }

                //background.SetPixel(iTexX, iTexY, Color.magenta);
            }
        }

        fAverage = fAverage / (float)iTicks;
        fAverageDensity = fAverageDensity / (float)iTicks;

        // Draw background?
        Texture2D background = new Texture2D(Mathf.CeilToInt(100 * (m_sectorSize.x / m_sectorSize.y)), 100, TextureFormat.ARGB32, false);
        int iTexX, iTexY;
        Color[] colors = new Color[16];
        Color temp = data.GetSectorColor();

        for (int i = 0; i < background.width; i++) {
            for (int j = 0; j < background.height; j++) {
                fValue = Mathf.PerlinNoise(((float)i / (float)background.width) * area.width + xLeft, ((float)j / (float)background.height) * area.height + yTop);

                /*for (int iColor = 0; iColor < 16; iColor++) {
                    colors[iColor] = new Color(0, 0, 0.25f * fValue, 1);
                }*/

                iTexX = i;
                iTexY = j;
                background.SetPixel(i, j, temp * fValue * fDensity);
                //background.SetPixels(iTexX, iTexY, Mathf.Min(4, background.width - iTexX), Mathf.Min(4, background.height - iTexY), colors);
            }
        }

        background.Apply();

        if (m_background != null) {
            MeshRenderer renderer = m_background.GetComponent<MeshRenderer>();

            if (renderer != null) {
                renderer.material.mainTexture = background;
                //renderer.material.SetTexture("Base (RGB)", background);
            }
        }

        // Setup stars
        for (int i = 0; i < m_stars.Count; i++) {
            m_stars[i].Setup(Random.Range(0, int.MaxValue));
        }
    }

    private float MakeNoise() {
        return m_sampleIncrement * Random.value - m_sampleIncrement * 0.5f;
    }

    /// <summary>
    /// Returns a list of interesting objects in the sector that match a given scan radius
    /// </summary>
    /// <param name="from">The position, in world space, the request is from</param>
    /// <param name="fDistanceSqr">Max search distance, as distance squared</param>
    /// <param name="fStrength">Strength of the search signal</param>
    /// <returns></returns>
    public List<Star> SearchInterests(Vector3 from, float fDistanceSqr, float fStrength) {
        List<Star> matches = new List<Star>();
		float fDist;

        for (int i = 0; i < m_stars.Count; i++) {
			fDist = (m_stars[i].Position - from).sqrMagnitude;

            if (m_stars[i].IsInteresting && fDist <= fDistanceSqr) {
                matches.Add(m_stars[i]);
            }
        }

        return matches;
    }
}
