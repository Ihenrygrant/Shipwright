using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlls : MonoBehaviour {

    public PlayerUIController PlayerUI;
    public CameraController cameraController;
    public BuildingController buildingController;

    // Use this for initialization
    void Start () {
        PlayerInfo.PlayerInit();
    }
	
	// Update is called once per frame
	void Update () {

        MenuControls();

        switch (PlayerInfo.PlayerState)
        {
            case PlayerStates.Flying:
                FlightControls();
                BuildingControls();
                break;
            
        }
    }

    void MenuControls()
    {
        if (Input.GetKey(KeyCode.I))
        {
            UpdatePlayerState((int)PlayerStates.InventoryMenu);
            return;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            UpdatePlayerState((int)PlayerStates.EscapeMenu);
            return;
        }
    }

    public void UpdatePlayerState(int playerStates)
    {
        PlayerInfo.SetPlayerState((PlayerStates)playerStates);
        PlayerUI.PlayerStateUpdateNotification();
    }

    void BuildingControls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            buildingController.RemoveBlock();
        }

        if (Input.GetMouseButtonDown(1))
        {
            buildingController.PlaceBlock();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            buildingController.RemoveBlock();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            buildingController.PlaceBlock();
        }

    }

    void FlightControls()
    {

        Vector3 movement = new Vector3(0,0,0);

        //movement
        if (Input.GetKey(KeyCode.Q))
        {
            cameraController.RollPlayer(Vector3.forward);
        }

        if (Input.GetKey(KeyCode.E))
        {

            cameraController.RollPlayer(-Vector3.forward);
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement += transform.right;
            //cameraController.SetVelocity(transform.right);
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement += -transform.right;
            //cameraController.SetVelocity(-transform.right);
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement += -transform.forward;
            //cameraController.SetVelocity(-transform.forward);
        }
        if (Input.GetKey(KeyCode.W))
        {
            movement += transform.forward;
            //cameraController.SetVelocity(transform.forward);
        }
        movement *= 5;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            movement *= 5;
        }

        cameraController.SetVelocity(movement);
    }

}
