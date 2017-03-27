﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class GameManager : PunBehaviour {

    public static GameManager gm;

    public GameObject Player;

    public Camera MainCamera;

    public bool CanServerControl = false;
    public bool CanClientControl = false;

    public bool AmIServer = false;
    public bool AmIClient = false;

    public GameObject BlackScreen;

    public Transform BlackScreenInvisPos;
    public Transform BlackScreenVisPos;

    private PhotonView pv;

    private float timer = 0.0f;

    private void Awake()
    {
        if ( gm == null)
        {
            gm = this;
            DontDestroyOnLoad(gm.transform);
        }else if ( gm != this)
        {
            Destroy(gm);
        }
    }

    private void Start()
    {
        BlackScreen.GetComponent<Transform>().position = BlackScreenInvisPos.position;
        pv = Player.GetComponent<PhotonView>();
        CanServerControl = true;
        UpdateOwner();
    }

    private void Update()
    {
        if ( !CheckIf2Players())
        {
            return;
        }

        // If we do have two players in.
        timer += Time.deltaTime;
        if ( timer > 15.0f)
        {
            timer = 0.0f;
            CanClientControl = !CanClientControl;
            CanServerControl = !CanServerControl;
            UpdateOwner();
        }

        if ( pv.isMine)
        {
            BlackScreen.GetComponent<Transform>().position = BlackScreenVisPos.position;
            int whose = pv.ownerId;
            if (whose == 1)
            {
                // You are server.
                AmIServer = true;
            }
            else if (whose == 2)
            {
                // You are the Client.
                AmIClient = true;
            }
        }
        else
        {
            BlackScreen.GetComponent<Transform>().position = BlackScreenInvisPos.position;
        }

    }

    void UpdateOwner()
    {
        if ( !pv.isMine)
        {
            int whose = pv.ownerId;

            //if ( (AmIServer && CanServerControl) || (AmIClient && CanClientControl) )
            //{
            //    pv.RequestOwnership();
            //}

            if (whose == 2 && CanServerControl)
            {
                pv.RequestOwnership();
                // Instead of using Main Camera, use a black object to appear before the game screen.
                //MainCamera.enabled = false;
            }
            else if (whose == 1 && CanServerControl)
            {
                // This is not yours (You are client) and you cannot control this.
                //MainCamera.enabled = true;
                return;
            }
            else if (whose == 2 && CanClientControl)
            {
                // This is not yours ( You are Server ) and you cannot control this.
                //MainCamera.enabled = true;
                return;
            }
            else if (whose == 1 && CanClientControl)
            {
                // This is not your ( You are the Client ). But you should be able to control. So Claim it
                pv.RequestOwnership();
                //MainCamera.enabled = false;
            }
            
        }
        else
        {
            
        }
        // If this is yours. Dont do anything. Wait for others to claim it? But this is a singleton script? Lets see if this actually works.
    }

    bool CheckIf2Players()
    {
        int count = PhotonNetwork.playerList.Length;
        if ( count == 2)
        {
            return true;
        }else
        {
            return false;
        }
    }

    void updateBools()
    {
        if (pv.ownerId == 1)
        {
            CanServerControl = true;
        }
        else if (pv.ownerId == 2)
        {
            CanClientControl = true;
        }
    }

}