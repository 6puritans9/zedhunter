using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleManager : MonoBehaviour
    {
        public GameObject LobbyButtons;
        public GameObject goBackButton;
    public void ToggleLobbyButtons()
        {
            if (LobbyButtons.activeSelf)
                {
                    LobbyButtons.SetActive(false);
                    goBackButton.SetActive(true);
                }
            else
                {
                    LobbyButtons.SetActive(true);
                    goBackButton.SetActive(false);
                }
        }
}
