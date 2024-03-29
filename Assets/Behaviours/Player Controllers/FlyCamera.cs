﻿

using UnityEngine;
using System.Collections;
using UnityEditor;

public class FlyCamera : MonoBehaviour
{
    [SerializeField] private float rotation_speed = 3f;
    [SerializeField] private float fly_speed = 2.5f;
    private Vector2 rotation;

    void Update()
    {
        rotation = new Vector2(rotation.x + Input.GetAxis("Mouse X") * rotation_speed,
            rotation.y + Input.GetAxis("Mouse Y") * rotation_speed);//rotation based on mouse

        transform.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotation.y, Vector3.left);

        transform.position += transform.forward * fly_speed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += transform.right * fly_speed * Input.GetAxis("Horizontal") * Time.deltaTime;
    }
}