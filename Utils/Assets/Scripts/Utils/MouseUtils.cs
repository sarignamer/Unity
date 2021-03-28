using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class MouseUtils
    {
        public static Vector3 GetMousePosition(Camera? camera = null)
        {
            Camera cam = (camera == null) ? Camera.main : camera;

            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -cam.transform.position.z;
            Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(mousePosition);
            mouseWorldPosition.z = 0;

            return mouseWorldPosition;
        }

        public static Vector3 GetMousePosition3D(Camera? camera = null, LayerMask? layerMask = null)
        {
            Camera cam = (camera == null) ? Camera.main : camera;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (layerMask == null)
            {
                if (Physics.Raycast(ray, out RaycastHit raycastHit))
                {
                    return raycastHit.point;
                }
            }
            else
            {
                if (Physics.Raycast(ray, out RaycastHit raycastHit, cam.farClipPlane, (LayerMask)layerMask))
                {
                    return raycastHit.point;
                }
            }

            return Vector3.zero;
        }
    }
}
