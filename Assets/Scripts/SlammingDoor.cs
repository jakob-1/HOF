﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlammingDoor : MonoBehaviour {

    public bool on = false;
    public float slamSpeed = 10;
    public bool swung = false;
    public bool slammed = false;
    public AudioSource doorSwing;
    public AudioSource slamSound;
    public  float targetRotation = -90;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (on && transform.rotation.y > targetRotation)
        {
            if (!swung)
            {
                //doorSwing.Play();
                //Debug
                //Debug.Log("111111");
                swung = true;
                
            }
            var target = Quaternion.Euler(0, targetRotation, 0);
            transform.localRotation = 
                Quaternion.Slerp(transform.localRotation, target,
                    slamSpeed * Time.deltaTime);
            if (!slammed)
            {
                slamSound.Play();
                //Debug
                //Debug.Log("222222");
                slammed = true;
            }
        }
        
	}

    public void slam()
    {
        on = true;
    }
}