﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Manager<UIManager> {
    private const string mainMenuScene = "MainMenu";

    /// <summary>
    /// The Inventory UI object in the current scene.
    /// </summary>
    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private DialogueManager dialogueManager;

    [SerializeField]
    private Text hoverText;

    [SerializeField]
    private Image blackout;

    [SerializeField]
    private Sprite interactIcon;

    [SerializeField]
    private AnimController genericAnimPrefab;
    public static AnimController GenericAnimPrefab { get { return instance.genericAnimPrefab; } }
    
    [SerializeField]
    private float fadeTime = 2f;
    public static float FadeTime { get { return instance.fadeTime; } }

    private System.DateTime fadeStart;
    private float currentFadeTime;
    private bool dlgActionFade;

    private static Sprite cursorIcon;

    private bool worldInputEnabled;

    /// <summary>
    /// Whether world input (clicking items in the world such
    /// as characters, object, and the background) is enabled.
    /// </summary>
    public static bool WorldInputEnabled {
        get { return instance.worldInputEnabled && AllInputEnabled; }
        private set { instance.worldInputEnabled = value; }
    }

    /// <summary>
    /// Whether player input of any kind is enabled.
    /// </summary>
    public static bool AllInputEnabled { get; private set; }

    void Start() {
        SingletonInit();

        AllInputEnabled = true;
        WorldInputEnabled = true;
        ClearHoverText();
        if (inventory != null) { inventory.Init(); }
        if (dialogueManager != null) { dialogueManager.Init(); }
    }

    void Update() {
        if (AllInputEnabled) {
            // Handle generic input
            if (inventory != null && Input.GetMouseButtonUp(1)) {
                if (Inventory.SelectedItem == null && !DialogueManager.isShown) {
                    Inventory.Show(!Inventory.isShown);
                }
                else {
                    Inventory.ClearSelection();
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape) && GameManager.GetCurrentScene() != mainMenuScene) {
            GameManager.LoadScene(mainMenuScene);
        }

        if (blackout != null && blackout.gameObject.activeInHierarchy &&
            (System.DateTime.Now - fadeStart).TotalSeconds >= currentFadeTime) {
            blackout.gameObject.SetActive(false);
            if (dlgActionFade) { Dialogue.Actions.CompletePendingAction(); }
        }
    }

    public static void SetCustomCursor(Sprite cursor) {
        cursorIcon = cursor;
        ShowCustomCursor(true);
    }

    public static void SetInteractionCursor(bool show) {
        if (show) {
            if (cursorIcon == null) { SetCustomCursor(instance.interactIcon); }
        }
        else if (cursorIcon == instance.interactIcon) { ClearCustomCursor(); }
    }

    public static void ClearCustomCursor() {
        cursorIcon = null;
        ShowCustomCursor(false);
    }
    
    public static void ShowCustomCursor(bool show) {
        if (show && cursorIcon != null) { Util.SetCursor(cursorIcon); }
        else { Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); }
    }

    public static void BlockWorldInput(bool block) {
        if (!block && !Inventory.isShown && !DialogueManager.isShown) {
            WorldInputEnabled = (true);
        }
        else { WorldInputEnabled = (false); }
    }

    public static void BlockAllInput(bool block) {
        AllInputEnabled = !block;
    }

    /// <summary>
    /// Load a different game scene. Use this for buttons (otherwise, use
    /// GameManager's static function).
    /// </summary>
    /// <param name="scene"></param>
    public void LoadScene(string scene) {
        GameManager.LoadScene(scene);
    }

    public void QuitGame() {
        GameManager.QuitGame();
    }

    public static void SetHoverText(string s) {
        if (instance.hoverText != null) { instance.hoverText.text = s; }
    }

    public static void ClearHoverText() {
        SetHoverText("");
    }

    public static void ShowHoverText(bool show) {
        instance.hoverText.gameObject.SetActive(show);
    }
    
    public static void FadeOut(bool isDialogueAction) { FadeOut(FadeTime, isDialogueAction); }
    public static void FadeOut(float duration = -1f, bool isDialogueAction = false) {
        SetAlpha(instance.blackout, 0.01f);
        Fade(255f, duration, isDialogueAction);
    }

    public static void FadeIn(bool isDialogueAction) { FadeIn(FadeTime, isDialogueAction); }
    public static void FadeIn(float duration = -1f, bool isDialogueAction = false) {
        SetAlpha(instance.blackout, 1f);
        Fade(0f, duration, isDialogueAction);
    }

    private static void SetAlpha(Image image, float alpha) {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private static void Fade(float targetAlpha, float duration, bool isDialogueAction = false) {
        instance.dlgActionFade = isDialogueAction;

        float d = (duration == -1f) ? FadeTime : duration;
        instance.currentFadeTime = duration;
        instance.fadeStart = System.DateTime.Now;

        instance.blackout.gameObject.SetActive(true);
        instance.blackout.CrossFadeAlpha(targetAlpha, d, false);
    }
}