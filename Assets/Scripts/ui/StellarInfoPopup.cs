using UnityEngine;
using System.Collections;
using System;

namespace Mothership.UI {
    public class StellarInfoPopup : MonoBehaviour {
        public UILabel m_idLabel;
        public UILabel m_typeLabel;
        public UILabel m_distanceLabel;
        public UILabel m_timeLabel;
        public UILabel m_jumpExpectionLabel;
        public UIButton m_jumpButton;

        private UI m_ui;
        private Star m_target;

        public Star Target {
            get { return m_target; }
        }

        public void Setup(Vector3 pos, UI ui, Star target) {
            pos.z = -1;
            transform.position = pos;
            m_ui = ui;
            m_target = target;

            m_idLabel.text = target.Data.Identification;
            m_idLabel.color = target.GetColor();
            m_typeLabel.text = target.Data.Type;

            float fDistance = 0;
            double fTime = 0;
            GalacticShip.GetParamsTo(target, out fDistance, out fTime);

            m_distanceLabel.text = Math.Round(fDistance, 2) + " ly";
            m_timeLabel.text = UI.ConvertToTimeString(fTime, true);

            if (fDistance > GalacticShip.Instance.FTLRange) {
                m_jumpButton.collider.enabled = false;
                m_jumpButton.UpdateColor(false, true);
                m_jumpExpectionLabel.text = "Not enough fuel";
            } else {
                m_jumpButton.collider.enabled = true;
                m_jumpButton.UpdateColor(true, true);
                m_jumpExpectionLabel.text = string.Empty;
            }
        }

        private void OnSelectJump() {
            m_ui.InitiateJump(m_target);
        }

        public void OnSelectClose() {
            Destroy(gameObject);
        }
    }
}
