﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SlimUI.ModernMenu
{
public class UIMenuManager : MonoBehaviour
    {
        private Animator CameraObject;

        // campaign button sub menu
        [Header("MENUS")] [Tooltip("The Menu for when the MAIN menu buttons")]
        public GameObject mainMenu;

        [Tooltip("THe first list of buttons")] public GameObject firstMenu;

        [Tooltip("The Menu for when the PLAY button is clicked")]
        public GameObject signInMenu;

        [Tooltip("Optional 4th Menu")] public GameObject createAccountMenu;

        [Tooltip("The Menu for when the EXIT button is clicked")]
        public GameObject exitMenu;

        public enum Theme
            {
                custom1,
                custom2,
                custom3
            };

        [Header("THEME SETTINGS")] public Theme theme;
        private int themeIndex;
        public ThemedUIData themeController;

        [Header("PANELS")] [Tooltip("The UI Panel parenting all sub menus")]
        public GameObject mainCanvas;

        public GameObject PanelCreateRoom;
        public GameObject PanelRoomList;
        public GameObject PanelJoinedRoom;

        [Tooltip("The UI Panel that holds the KEY BINDINGS window tab")]
        public GameObject PanelKeyBindings;

        [Tooltip("The UI Sub-Panel under KEY BINDINGS for MOVEMENT")]
        public GameObject PanelMovement;

        [Tooltip("The UI Sub-Panel under KEY BINDINGS for COMBAT")]
        public GameObject PanelCombat;

        [Tooltip("The UI Sub-Panel under KEY BINDINGS for GENERAL")]
        public GameObject PanelGeneral;


        // highlights in settings screen
        [Header("SETTINGS SCREEN")] [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
        public GameObject lineGame;

        [Tooltip("Highlight Image for when VIDEO Tab is selected in Settings")]
        public GameObject lineVideo;

        [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
        public GameObject lineControls;

        [Tooltip("Highlight Image for when KEY BINDINGS Tab is selected in Settings")]
        public GameObject lineKeyBindings;

        [Tooltip("Highlight Image for when MOVEMENT Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineMovement;

        [Tooltip("Highlight Image for when COMBAT Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineCombat;

        [Tooltip("Highlight Image for when GENERAL Sub-Tab is selected in KEY BINDINGS")]
        public GameObject lineGeneral;

        [Header("LOADING SCREEN")] [Tooltip("If this is true, the loaded scene won't load until receiving user input")]
        public bool waitForInput = true;

        public GameObject loadingMenu;

        [Tooltip("The loading bar Slider UI element in the Loading Screen")]
        public Slider loadingBar;

        public TMP_Text loadPromptText;
        public KeyCode userPromptKey;

        [Header("SFX")] [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
        public AudioSource hoverSound;

        [Tooltip("The GameObject holding the Audio Source component for the AUDIO SLIDER")]
        public AudioSource sliderSound;

        [Tooltip(
            "The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen")]
        public AudioSource swooshSound;

        [Header("Extra")] public GameObject goBackButton;
        public GameObject LobbyButtons;

        void Start()
            {
                CameraObject = transform.GetComponent<Animator>();

                signInMenu.SetActive(false);
                exitMenu.SetActive(false);
                if (createAccountMenu) createAccountMenu.SetActive(false);
                firstMenu.SetActive(true);
                mainMenu.SetActive(true);

                SetThemeColors();
            }

        void SetThemeColors()
            {
                switch (theme)
                    {
                        case Theme.custom1:
                            themeController.currentColor = themeController.custom1.graphic1;
                            themeController.textColor = themeController.custom1.text1;
                            themeIndex = 0;
                            break;
                        case Theme.custom2:
                            themeController.currentColor = themeController.custom2.graphic2;
                            themeController.textColor = themeController.custom2.text2;
                            themeIndex = 1;
                            break;
                        case Theme.custom3:
                            themeController.currentColor = themeController.custom3.graphic3;
                            themeController.textColor = themeController.custom3.text3;
                            themeIndex = 2;
                            break;
                        default:
                            Debug.Log("Invalid theme selected.");
                            break;
                    }
            }

        public void PlayCampaign()
            {
                exitMenu.SetActive(false);
                if (createAccountMenu) createAccountMenu.SetActive(false);
                // signInMenu.SetActive(true);
                Position2();
                ReturnMenu();
            }


        public void PlayCampaignMobile()
            {
                exitMenu.SetActive(false);
                if (createAccountMenu) createAccountMenu.SetActive(false);
                signInMenu.SetActive(true);
                mainMenu.SetActive(false);
            }

        public void PlayNewAccount()
            {
                exitMenu.SetActive(false);
                if (createAccountMenu) createAccountMenu.SetActive(true);
                signInMenu.SetActive(false);
            }

        public void ReturnMenu()
            {
                signInMenu.SetActive(false);
                if (createAccountMenu) createAccountMenu.SetActive(false);
                exitMenu.SetActive(false);
                mainMenu.SetActive(true);
            }

        public void LoadScene(string scene)
            {
                if (scene != "")
                    {
                        StartCoroutine(LoadAsynchronously(scene));
                    }
            }

        public void DisablePlayCampaign()
            {
                signInMenu.SetActive(false);
            }

        public void Position2()
            {
                DisablePlayCampaign();
                CameraObject.SetFloat("Animate", 1);
            }

        public void Position1()
            {
                CameraObject.SetFloat("Animate", 0);
            }

        void DisablePanels()
            {
                PanelRoomList.SetActive(false);
                PanelJoinedRoom.SetActive(false);
                PanelCreateRoom.SetActive(false);
                PanelKeyBindings.SetActive(false);

                lineGame.SetActive(false);
                lineControls.SetActive(false);
                lineVideo.SetActive(false);
                lineKeyBindings.SetActive(false);

                PanelMovement.SetActive(false);
                lineMovement.SetActive(false);
                PanelCombat.SetActive(false);
                lineCombat.SetActive(false);
                PanelGeneral.SetActive(false);
                lineGeneral.SetActive(false);
            }

        public void ShowPanelCreateNewRoom()
            {
                DisablePanels();
                PanelCreateRoom.SetActive(true);
                lineGame.SetActive(true);
            }

        public void DisablePanelCreateNewRoom()
            {
                DisablePanels();
            }

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


        public void ShowPanelRoomList()
            {
                DisablePanels();
                PanelRoomList.SetActive(true);
                lineControls.SetActive(true);
            }

        public void KeyBindingsPanel()
            {
                DisablePanels();
                MovementPanel();
                PanelKeyBindings.SetActive(true);
                lineKeyBindings.SetActive(true);
            }

        public void MovementPanel()
            {
                DisablePanels();
                PanelKeyBindings.SetActive(true);
                PanelMovement.SetActive(true);
                lineMovement.SetActive(true);
            }

        public void CombatPanel()
            {
                DisablePanels();
                PanelKeyBindings.SetActive(true);
                PanelCombat.SetActive(true);
                lineCombat.SetActive(true);
            }

        public void GeneralPanel()
            {
                DisablePanels();
                PanelKeyBindings.SetActive(true);
                PanelGeneral.SetActive(true);
                lineGeneral.SetActive(true);
            }

        public void PlayHover()
            {
                hoverSound.Play();
            }

        public void PlaySFXHover()
            {
                sliderSound.Play();
            }

        public void PlaySwoosh()
            {
                swooshSound.Play();
            }

        // Are You Sure - Quit Panel Pop Up
        public void AreYouSure()
            {
                exitMenu.SetActive(true);
                if (createAccountMenu) createAccountMenu.SetActive(false);
                DisablePlayCampaign();
            }

        public void AreYouSureMobile()
            {
                exitMenu.SetActive(true);
                if (createAccountMenu) createAccountMenu.SetActive(false);
                mainMenu.SetActive(false);
                DisablePlayCampaign();
            }

        public void ExtrasMenu()
            {
                signInMenu.SetActive(false);
                if (createAccountMenu) createAccountMenu.SetActive(true);
                exitMenu.SetActive(false);
            }

        public void QuitGame()
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
            }

        // Load Bar synching animation
        IEnumerator LoadAsynchronously(string sceneName)
            {
                // scene name is just the name of the current scene being loaded
                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
                operation.allowSceneActivation = false;
                mainCanvas.SetActive(false);
                loadingMenu.SetActive(true);

                while (!operation.isDone)
                    {
                        float progress = Mathf.Clamp01(operation.progress / .95f);
                        loadingBar.value = progress;

                        if (operation.progress >= 0.9f && waitForInput)
                            {
                                loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() +
                                                      " to continue";
                                loadingBar.value = 1;

                                if (Input.GetKeyDown(userPromptKey))
                                    {
                                        operation.allowSceneActivation = true;
                                    }
                            }
                        else if (operation.progress >= 0.9f && !waitForInput)
                            {
                                operation.allowSceneActivation = true;
                            }

                        yield return null;
                    }
            }
    }
}