using UnityEngine;

public class VersionOverlay : MonoBehaviour
{
    private static VersionOverlay instance;

    [SerializeField] bool onlyInDevelopmentBuild = false;
    [SerializeField] Vector2 margin = new Vector2(12, 12);
    [SerializeField] VersionOverlayPosition position = VersionOverlayPosition.TopLeft;
    [SerializeField] int fontSize = 16;
    [SerializeField] Color color = Color.white;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureExists()
    {
        if (instance != null) return;

        instance = Object.FindAnyObjectByType<VersionOverlay>();
        if (instance != null) return;

        GameObject go = new GameObject("VersionOverlay (Auto)");
        instance = go.AddComponent<VersionOverlay>();
        DontDestroyOnLoad(go);
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnGUI()
    {
        if (onlyInDevelopmentBuild && !Debug.isDebugBuild) return;

        string txt = $"v{Application.version}";

        var style = new GUIStyle(GUI.skin.label);
        style.fontSize = fontSize;
        style.normal.textColor = color;

        switch (position)
        {
            default:
            case VersionOverlayPosition.TopLeft:
                style.alignment = TextAnchor.UpperLeft;
                break;

            case VersionOverlayPosition.TopRight:
                style.alignment = TextAnchor.UpperRight;
                break;

            case VersionOverlayPosition.BottomLeft:
                style.alignment = TextAnchor.LowerLeft;
                break;

            case VersionOverlayPosition.BottomRight:
                style.alignment = TextAnchor.LowerRight;
                break;
        }
        Rect rect = GetRect(position, margin, 600, 40);
        GUI.Label(new Rect(rect), txt, style);
    }

    Rect GetRect(VersionOverlayPosition pos, Vector2 margin, float width, float height)
    {
        switch (pos)
        {
            case VersionOverlayPosition.TopRight:
                return new Rect(
                    Screen.width - width - margin.x,
                    margin.y,
                    width,
                    height
                );

            case VersionOverlayPosition.BottomLeft:
                return new Rect(
                    margin.x,
                    Screen.height - height - margin.y,
                    width,
                    height
                );

            case VersionOverlayPosition.BottomRight:
                return new Rect(
                    Screen.width - width - margin.x,
                    Screen.height - height - margin.y,
                    width,
                    height
                );

            default: // TopLeft
                return new Rect(
                    margin.x,
                    margin.y,
                    width,
                    height
                );
        }
    }
}

public enum VersionOverlayPosition
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}
