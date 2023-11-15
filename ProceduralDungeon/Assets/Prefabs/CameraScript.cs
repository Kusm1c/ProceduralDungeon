using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    public GameObject player;
    [SerializeField] private float lerpingSpeed = 10f;

    // Update is called once per frame
    private void LateUpdate()
    {
        if (player == null) return;
        Vector3 desiredPosition = player.transform.position + offset;
        Vector3 lerpedPosition = Vector3.Lerp(transform.position, desiredPosition, lerpingSpeed * Time.deltaTime);
        transform.position = lerpedPosition;
        var transformRotation = transform.rotation;
        transformRotation.x = LookAtPlayer();
        transform.rotation = transformRotation;
    }

    private float LookAtPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        return rotation.x;
    }
}
