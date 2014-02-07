using UnityEngine;
using System.Collections;

namespace Mothership.Actors {

    public class Ship : MonoBehaviour {
        private Transform m_transform;

        /// <summary>Icon to display when camera is at a distance</summary>
        public GameObject m_distanceIcon;

        public GameObject m_beacon;
        private bool m_bFlashBeacon = true;

        // Use this for initialization
        void Start() {
            m_transform = transform;
            SectorCamera.OnViewChange += OnViewChange;

            OnViewChange(SectorCamera.Instance.transform.position, new Rect());
        }

        void OnDestroy() {
            SectorCamera.OnViewChange -= OnViewChange;
        }

        private void OnViewChange(Vector3 pos, Rect rect) {
            
            if (m_transform.position.z - pos.z >= 5) {
                if (!m_bFlashBeacon) {
                    NGUITools.SetActive(m_beacon, true);
                    m_bFlashBeacon = true;
                }
            } else {
                if (m_bFlashBeacon) {
                    NGUITools.SetActive(m_beacon, false);
                    m_bFlashBeacon = false;
                }
            }

        }
    }
}
