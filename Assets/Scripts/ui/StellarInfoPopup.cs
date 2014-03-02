using UnityEngine;
using System.Collections;

public class StellarInfoPopup : MonoBehaviour {
    public UILabel m_idLabel;
    public UILabel m_typeLabel;

    public void Setup(Star target) {
        m_idLabel.text = target.Data.Identification;
        m_typeLabel.text = target.Data.Type;
    }
}
