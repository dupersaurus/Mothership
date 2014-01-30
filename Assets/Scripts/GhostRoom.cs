using UnityEngine;
using System.Collections;

public class GhostRoom : Room {

    private bool m_bIsValid = false;

    public bool IsValid {
        get { return m_bIsValid; }
    }

    protected override Color GetRoomColor() {
        Color roomColor = base.GetRoomColor();
        roomColor.a = 0.5f;

        return roomColor;
    }
}
