using UnityEngine;

public static class PlayerKeybinds
{
    #region Keybinds

    [Header("Movement Keybinds")]
    // still using "horizontal" and "vertical" for WASD movement.
    public static readonly KeyCode JUMP_KEY = KeyCode.Space;
    public static readonly KeyCode SPRINT_KEY = KeyCode.LeftShift;
    public static readonly KeyCode CROUCH_KEY = KeyCode.LeftControl;

    [Header("Interaction Keybinds")]
    public static readonly KeyCode INTERACT_KEY = KeyCode.F;
    public static readonly KeyCode DROP_KEY = KeyCode.Q;
    public static readonly KeyCode PRIMARY_USE_KEY = KeyCode.Mouse0;
    public static readonly KeyCode SECONDARY_USE_KEY = KeyCode.Mouse1;

    [Header("Menu Keybinds")]
    public static readonly KeyCode TOOL_MENU_KEY = KeyCode.Tab;
    public static readonly KeyCode ESCAPE_KEY = KeyCode.Escape;

    #endregion
}
