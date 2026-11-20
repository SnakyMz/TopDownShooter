using UnityEngine;

public static class PlayerControl
{
    public static Controls Controls { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Controls = new Controls();
        Controls.Player.Enable();
    }
}
