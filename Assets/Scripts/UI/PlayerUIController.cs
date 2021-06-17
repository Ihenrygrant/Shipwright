using UnityEngine;

public class PlayerUIController : MonoBehaviour {

    public GameObject playerHUD;
    public GameObject CreativeMenu;
    public GameObject EscapeMenu;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayerStateUpdateNotification()
    {
        switch (PlayerInfo.PlayerState)
        {
            case PlayerStates.Flying:
                playerHUD.SetActive(true);
                CreativeMenu.SetActive(false);
                EscapeMenu.SetActive(false);
                break;
            case PlayerStates.Driving:
                playerHUD.SetActive(false);
                CreativeMenu.SetActive(false);
                EscapeMenu.SetActive(false);
                break;
            case PlayerStates.InventoryMenu:
                playerHUD.SetActive(false);
                CreativeMenu.SetActive(true);
                EscapeMenu.SetActive(false);
                break;
            case PlayerStates.EscapeMenu:
                playerHUD.SetActive(false);
                CreativeMenu.SetActive(false);
                EscapeMenu.SetActive(true);
                break;
        }
    }

    public void UpdatePlayerState(PlayerStates playerStates)
    {
        PlayerInfo.SetPlayerState(playerStates);
    }
}
