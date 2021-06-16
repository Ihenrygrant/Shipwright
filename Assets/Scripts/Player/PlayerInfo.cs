using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates
{
    Flying,
    Driving,
    InventoryMenu,
    EscapeMenu
}

public static class PlayerInfo {
    public static ushort health;
    public static int blockSelection;

    public static PlayerStates PlayerState;

    public static void PlayerInit()
    {
        SetPlayerState(PlayerStates.Flying);
    }

    public static void SetPlayerState(PlayerStates state)
    {
        switch (state)
        {
            case PlayerStates.Flying:
                //Cursor.visible = false;
                break;
            case PlayerStates.Driving:
                //Cursor.visible = false;
                break;
            case PlayerStates.InventoryMenu:
                //Cursor.visible = true;
                break;
            case PlayerStates.EscapeMenu:
                //Cursor.visible = true;
                break;
        }

        PlayerState = state;

    }


}
