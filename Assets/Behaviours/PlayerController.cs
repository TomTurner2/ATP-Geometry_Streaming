using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float move_speed = 1;
    [SerializeField] float rotation_speed = 3;
    [SerializeField] float sprint_multiplier = 2;
    [SerializeField] float jump_force = 500;
    [SerializeField] Rigidbody rigidbody = null;
    [SerializeField] Camera camera_target = null;

    private Vector2 rotation;
    private bool jumping = false;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        DetectFlying();
        DetectJump();
    }


    void FixedUpdate ()
    {
        Jump();
        Move();
    }


    void LateUpdate()
    {
        CameraRotation();
    }


    private void Move()
    {
        if (rigidbody == null)
            return;

        Vector3 position = CalculateMovePosition();
        rigidbody.MovePosition(position);
    }


    private void Jump()
    {
        if (rigidbody == null || !jumping)
            return;

        rigidbody.AddForce(new Vector3(0, jump_force * rigidbody.mass, 0), ForceMode.Impulse);
        jumping = false;
    }


    private void CameraRotation()
    {
        if (camera_target == null || Cursor.lockState == CursorLockMode.None)
            return;

        float x_rotation = rotation.x + Input.GetAxis("Mouse X") * rotation_speed;
        float y_rotation = rotation.y + Input.GetAxis("Mouse Y") * rotation_speed;
        rotation = new Vector2(x_rotation, y_rotation);//rotation based on mouse

        camera_target.transform.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up);
        camera_target.transform.localRotation *= Quaternion.AngleAxis(rotation.y, Vector3.left); 
    }


    private Vector3 CalculateMovePosition()
    {
        float speed = move_speed;

        if (Input.GetKey(KeyCode.LeftShift))
            speed *= sprint_multiplier;

        float x_speed = speed * Input.GetAxis("Vertical") * Time.deltaTime;
        float z_speed = speed * Input.GetAxis("Horizontal") * Time.deltaTime;

        Vector3 position = rigidbody.position;
        position += camera_target.transform.forward * x_speed;
        position += camera_target.transform.right * z_speed;

        if (rigidbody.useGravity)
            position = new Vector3(position.x, rigidbody.position.y, position.z);//prevent flying

        return position;
    }


    private void DetectFlying()
    {
        if (!Input.GetKeyDown(KeyCode.F))
            return;

        rigidbody.useGravity = !rigidbody.useGravity;
    }


    private void DetectJump()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        jumping = true;
    }
}
