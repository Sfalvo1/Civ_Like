using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseController : MonoBehaviour
{
    public float scrollSensitivity;

    bool isDraggingCamera = false;
    Vector3 lastMousePosition;

    Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        // Raycast click-to-drag is a slower way

        Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        float rayLength = (mouseRay.origin.y / mouseRay.direction.y);
        Vector3 hitPos = mouseRay.origin - (mouseRay.direction * rayLength);

        MouseDrag(ref mouseRay, ref rayLength, ref hitPos);

        // Zoom to scrollwheel
        float scrollAmount = -Input.GetAxis("Mouse ScrollWheel");
        float minHeight = 2;
        float maxHeight = 20;

        HandleZoom(hitPos, scrollAmount, minHeight, maxHeight);

        AdjustCameraAngle(maxHeight);

    }

    private void HandleZoom(Vector3 hitPos, float scrollAmount, float minHeight, float maxHeight)
    {
        if (Mathf.Abs(scrollAmount) > 0.01f)
        {
            // Moves camera towards hitPos
            Vector3 dir = camera.transform.position - hitPos;

            Vector3 p = camera.transform.position;

            // Stop zooming out at a certain dist
            if (scrollAmount > 0 || p.y < (maxHeight + 1 - 0.1f))
            {
                camera.transform.Translate(dir * scrollAmount, Space.World);
            }

            p = camera.transform.position;

            if (p.y < minHeight)
            {
                p.y = minHeight;
            }
            if (p.y > maxHeight)
            {
                p.y = maxHeight;
            }

            camera.transform.position = p;

            // Change camera angle
            float lowZoom = minHeight + 3;
            float highZoom = maxHeight - 3;

            //if(p.y < lowZoom)
            //{
            //    camera.transform.rotation = Quaternion.Euler(
            //        Mathf.Lerp(30, 60, ((p.y - minHeight) / (lowZoom - minHeight))),
            //        camera.transform.rotation.eulerAngles.y,
            //        camera.transform.rotation.eulerAngles.z
            //        );
            //}
            //else if (p.y > highZoom)
            //{
            //    camera.transform.rotation = Quaternion.Euler(
            //        Mathf.Lerp(60, 90, ((p.y - highZoom) / (maxHeight - highZoom))),
            //        camera.transform.rotation.eulerAngles.y,
            //        camera.transform.rotation.eulerAngles.z
            //        );
            //}
            //else
            //{
            //    camera.transform.rotation = Quaternion.Euler(
            //        60,
            //        camera.transform.rotation.eulerAngles.y,
            //        camera.transform.rotation.eulerAngles.z
            //        );
            //}

            // AdjustCameraYAxis();
        }
    }

    private void MouseDrag(ref Ray mouseRay, ref float rayLength, ref Vector3 hitPos)
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDraggingCamera = true;

            lastMousePosition = hitPos;
        }

        else if (Input.GetMouseButtonUp(0))
        {
            isDraggingCamera = false;
        }

        if (isDraggingCamera)
        {
            Vector3 diff = lastMousePosition - hitPos;
            camera.transform.Translate(diff, Space.World);

            // If ray is not redone, then the camera will jitter
            mouseRay = camera.ScreenPointToRay(Input.mousePosition);

            // What is point at which mouse ray intersects y=0
            rayLength = (mouseRay.origin.y / mouseRay.direction.y);
            lastMousePosition = hitPos = mouseRay.origin - (mouseRay.direction * rayLength);
        }
    }

    private void AdjustCameraAngle(float maxHeight)
    {
        camera.transform.rotation = Quaternion.Euler(
            Mathf.Lerp(35, 90, camera.transform.position.y / (maxHeight / 1.5f)),
            camera.transform.rotation.eulerAngles.y,
            camera.transform.rotation.eulerAngles.z
        );
    }

    private void AdjustCameraYAxis()
    {
        float scrollNum = -Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity + Time.deltaTime;
        Vector3 scrollDiff = new Vector3(0, scrollNum, 0);
        camera.transform.position += scrollDiff;
    }
}
