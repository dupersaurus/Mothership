using UnityEngine;
using System.Collections;

public class GhostRoom : Room {

    private bool m_bIsValid = true;

    public bool IsValid {
        get { return m_bIsValid; }
    }

    protected override Color GetRoomColor() {
        Color roomColor = base.GetRoomColor();
        roomColor.a = 0.5f;

        return roomColor;
    }

    /// <summary>
    /// Expands the tile bounds to a given world position, snapped to the grid
    /// </summary>
    /// <param name="pos">World position, snapped to the grid</param>
    public void ExpandTo(Vector2 pos) {

        // Mouse position relative to the origin
        Vector2 drag = pos - (Vector2)m_transform.position;

        // Rectange defined by the mouse and the origin, the new room size
        Rect newBounds = new Rect();

        if (drag.x < 0) {
            newBounds.xMin = drag.x;
            newBounds.xMax = 0;
        } else {
            newBounds.xMin = 0;
            newBounds.xMax = drag.x;
        }

        if (drag.y < 0) {
            newBounds.yMin = drag.y;
            newBounds.yMax = 0;
        } else {
            newBounds.yMin = 0;
            newBounds.yMax = drag.y;
        }

        // Scale single cell to fit the rectangle...
        SetCellSize(newBounds);

        Transform cell = m_cells[0].transform;
        RoomConflictType conflict = Ship.CheckPlacement(this, m_collisionBounds);

        if (conflict == RoomConflictType.Valid) {
            cell.GetComponent<MeshRenderer>().material.color = GetRoomColor();
            m_bIsValid = true;
        } else {
            cell.GetComponent<MeshRenderer>().material.color = Color.red;
            m_bIsValid = false;
        }

        /*BoxCollider box = gameObject.GetComponent<BoxCollider>();
        box.center = cell.localPosition;
        box.size = cell.localScale;*/

        // ...OR fill in and trim the existing tiles to fit the new rectangle

        /* // Possibly better way, if more complicated
        Vector2 newBounds = pos - (Vector2)m_transform.position;
        bool bTrim = false;

        // Expand right
        if (newBounds.x > 0) {
            if (newBounds.x > m_cellBounds.xMax) {
                for (int i = 0; i < m_cellBounds.height + 1; i++) {
                    CreateCell(new Vector3((m_cellBounds.xMax + 1) * TILE_SIZE, m_cellBounds.yMax - i * TILE_SIZE, 0));
                }

                m_cellBounds.xMax++;
            }
        } else if (newBounds.x < 0) {
            bTrim = true;
            m_cellBounds.xMax--;
        }

        // Expand down
        if (newBounds.y < 0) {
            if (newBounds.y < m_cellBounds.yMin) {
                for (int i = 0; i < m_cellBounds.width + 1; i++) {
                    CreateCell(new Vector3(m_cellBounds.xMin + i * TILE_SIZE, (m_cellBounds.yMin - 1) * TILE_SIZE, 0));
                }

                m_cellBounds.yMin--;
            }
        } else if (newBounds.y > 0) {
            bTrim = true;
            m_cellBounds.yMin++;
        }*/

        // Y
        /*if (diff.y > 0) {

        } else if (diff.y < 0) {
            bTrim = true;
        }

        // Bottom corner
        if (diff.x > 0 && diff.y > 0) {

        }*/
    }
}
