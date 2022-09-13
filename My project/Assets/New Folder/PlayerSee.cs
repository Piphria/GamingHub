using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerSee : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    [SerializeField] Transform cam;
    [SerializeField] Transform orientation;

    float mouseX;
    float mouseY;

    float multiplyer = 0.01f;

    float rotationX;
    float rotationY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MyInput();

        cam.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientation.transform.rotation = Quaternion.Euler(0, rotationY, 0);

    }

    void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        rotationY += mouseX * sensX * multiplyer;
        rotationX -= mouseY * sensY * multiplyer;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }
}
