using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Position")]
    public float amount;
    public float maxAmount;
    public float smoothAmount;

    [Header("Rotational")]
    public float rotationAmount = 8;
    public float maxRotationAmount = 5;
    public float smoothRotation = 12;
    [Space]
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;

    private Vector3 initialPosition;
    private Quaternion initialRotation;


    private float InputX;
    private float InputY;

    
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateSway();

        MoveSway();
        tiltSway();
    }
    public void CalculateSway()
    {
        InputX = -Input.GetAxis("Mouse X");
        InputY = -Input.GetAxis("Mouse Y");
    }
    public void MoveSway()
    {
        float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(InputY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }

    private void tiltSway()
    {
        float tiltY = Mathf.Clamp(InputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
        float tiltX = Mathf.Clamp(InputY * rotationAmount, -maxRotationAmount, maxRotationAmount);

        Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? -tiltX : 0f, rotationY ? tiltY: 0f, rotationZ ? tiltY: 0));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothAmount);
    }
}
