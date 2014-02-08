using UnityEngine;
using System.Collections;

public class GalaxyMap : MonoBehaviour {

    public struct GalaxySectorData {

        /// <summary>[0,1] base density of the sector. Approximately corresponds to dust, yellowish color</summary>
        public float BaseDensity;

        /// <summary>[0,1] density from arm bands. Approximately corresponds to stars, bluish color</summary>
        public float BandingDensity;

        public Color GetSectorColor() {
            return new Color(BaseDensity * 0.7f, BaseDensity * 0.7f, BaseDensity * 0.6f);
        }
    }

    /// <summary>Number of sectors on either side of the 0-axis</summary>
    public int m_sectorResolution = 25;

    /// <summary>How big each sector is visually</summary>
    public float m_sectorDensity = 0.25f;

    private GalaxySectorData[,] m_sectors;

    private Transform m_transform;
    private Object m_prefab;

    public AnimationCurve m_baseDensityFalloff;

    void Start() {
        float fDistance;
        float fBaseDensity;

        m_transform = transform;

        Random.seed = 1;

        // Seed sector pass
        m_sectors = new GalaxySectorData[m_sectorResolution * 2 + 1, m_sectorResolution * 2 + 1];

        for (int i = -m_sectorResolution; i <= m_sectorResolution; i++) {
            for (int j = -m_sectorResolution; j <= m_sectorResolution; j++) {
                fDistance = new Vector2(i, j).sqrMagnitude;
                fBaseDensity = m_baseDensityFalloff.Evaluate(fDistance / 625);

                m_sectors[i + m_sectorResolution, j + m_sectorResolution] = new GalaxySectorData();
                m_sectors[i + m_sectorResolution, j + m_sectorResolution].BaseDensity = fBaseDensity;
            }
        }

        StartCoroutine(DrawChunk(-m_sectorResolution));

        // Spawn pass
        /*for (int i = -m_sectorResolution; i <= m_sectorResolution; i++) {
            for (int j = -m_sectorResolution; j <= m_sectorResolution; j++) {
                spawn = Instantiate(prefab) as GameObject;
                spawn.name = "Sector " + i + ", " + j;

                spawnTrans = spawn.transform;
                spawnTrans.parent = m_transform;
                spawnTrans.localScale = new Vector3(m_sectorResolution, m_sectorResolution, 1);
                spawnTrans.localPosition = new Vector3(i * m_sectorResolution, j * m_sectorResolution, 0);

                material = spawnTrans.GetComponentInChildren<MeshRenderer>().material;
                material.color = m_sectors[i, j].GetSectorColor();
            }
        }*/
    }

    /// <summary>
    /// Draw a chunk of the map
    /// </summary>
    /// <param name="iRow"></param>
    /// <returns></returns>
    private IEnumerator DrawChunk(int iRow) {
        if (m_prefab == null) {
            m_prefab = Resources.Load("Galaxy Sector");
        }

        GameObject spawn;
        Transform spawnTrans;
        Material material;

        for (int iCol = -m_sectorResolution; iCol <= m_sectorResolution; iCol++) {
            spawn = Instantiate(m_prefab) as GameObject;
            spawn.name = "Sector " + iCol + ", " + iRow;

            spawnTrans = spawn.transform;
            spawnTrans.parent = m_transform;
            spawnTrans.localScale = new Vector3(m_sectorDensity, m_sectorDensity, 1);
            spawnTrans.localPosition = new Vector3(iCol * m_sectorDensity, iRow * m_sectorDensity, 0);

            material = spawnTrans.GetComponentInChildren<MeshRenderer>().material;
            material.color = m_sectors[iCol + m_sectorResolution, iRow + m_sectorResolution].GetSectorColor();
        }

        yield return new WaitForSeconds(0.1f);

        if (iRow < m_sectors.GetLength(0) - 1 /*m_sectorResolution * 2 - 2*/) {
            StartCoroutine(DrawChunk(iRow + 1));
        }
    }
}
