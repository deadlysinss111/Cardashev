using UnityEngine;

public class MapResourceLoader : MonoBehaviour
{
    // ICONS
    [SerializeField] private Mesh _bossIcon;
    public Mesh BOSS_ICON { get => _bossIcon; }

    [SerializeField] private Mesh _combatIcon;
    public Mesh COMBAT_ICON { get => _combatIcon; }

    [SerializeField] private Mesh _eliteIcon;
    public Mesh ELITE_ICON { get => _eliteIcon; }

    [SerializeField] private Mesh _eventIcon;
    public Mesh EVENT_ICON { get => _eventIcon; }

    [SerializeField] private Mesh _shopIcon;
    public Mesh SHOP_ICON { get => _shopIcon; }

    [SerializeField] private Mesh _restIcon;
    public Mesh REST_ICON { get => _restIcon; }

    /*void Start()
    {
        // ICONS
        Texture2D tex = (Texture2D)Resources.Load("Icons/boss_icon");

        _bossIcon = Sprite.Create(tex, new Rect(), new Vector2());

        _combatIcon = (Sprite)Resources.Load("Icons/combat_icon");
        _eliteIcon = (Sprite)Resources.Load("Icons/elite_icon");
        _eventIcon = (Sprite)Resources.Load("Icons/event_icon");
        _shopIcon = (Sprite)Resources.Load("Icons/shop_icon");

        print(_bossIcon);
        print(_combatIcon);
        print(_eliteIcon);
        print(_eventIcon);
        print(_shopIcon);
    }*/
}
