using UnityEngine;

public class MapResourceLoader : MonoBehaviour
{
    // ICONS
    [SerializeField] private Sprite _bossIcon;
    public Sprite BOSS_ICON { get => _bossIcon; }

    [SerializeField] private Sprite _combatIcon;
    public Sprite COMBAT_ICON { get => _combatIcon; }

    [SerializeField] private Sprite _eliteIcon;
    public Sprite ELITE_ICON { get => _eliteIcon; }

    [SerializeField] private Sprite _eventIcon;
    public Sprite EVENT_ICON { get => _eventIcon; }

    [SerializeField] private Sprite _shopIcon;
    public Sprite SHOP_ICON { get => _shopIcon; }

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
