﻿using UnityEngine;

[RequireComponent(typeof(OrbitingCameraInput))]
public class OrbitingCamera : MonoBehaviour
{
    public Transform target;

    public float distance = 5f;
    public float sensitivity = 100f;
    private float yRot = 0f;
    private float xRot = 20f;

    private OrbitingCameraInput input;

    private void Start()
    {
        input = GetComponent<OrbitingCameraInput>();

#if UNITY_EDITOR
        // Somehow after updating to 2019.3, mouse axes sensitivity decreased, but only in the editor.
        sensitivity *= 10f;
#elif UNITY_WEBGL
            // To prevent the mouse axes not being detected when the cursor leaves the game window.
            Cursor.lockState = CursorLockMode.Locked;
#endif
    }

    private void LateUpdate()
    {
        var inputState = this.input.GetCameraInput();

        yRot += inputState.x * sensitivity * Time.deltaTime;
        xRot -= inputState.y * sensitivity * Time.deltaTime;
        xRot = Mathf.Clamp(xRot, 0f, 75f);

        Quaternion worldRotation = transform.parent != null ? transform.parent.rotation : Quaternion.FromToRotation(Vector3.up, target.up);
        Quaternion cameraRotation = worldRotation * Quaternion.Euler(xRot, yRot, 0f);
        Vector3 targetToCamera = cameraRotation * new Vector3(0f, 0f, -distance);

        transform.SetPositionAndRotation(target.position + targetToCamera, cameraRotation);
    }
}
