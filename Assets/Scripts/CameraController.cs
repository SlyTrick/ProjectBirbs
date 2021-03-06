﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float offset;

    private void Update()
    {
        if(playerTransform != null)
        {
            //transform.position = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z - offset);
            transform.position = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        }
    }
}
