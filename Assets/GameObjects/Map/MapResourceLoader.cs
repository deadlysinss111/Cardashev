using UnityEngine;

public class MapResourceLoader : MonoBehaviour
{
    // ICONS' Mesh
    [Header("Icon Mesh")]
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

    // ICONS' Sprite
    [Header("Icon Sprite")]
    [SerializeField] private Sprite _bossIconSprite;
    public Sprite BOSS_ICON_SPRITE { get => _bossIconSprite; }

    [SerializeField] private Sprite _combatIconSprite;
    public Sprite COMBAT_ICON_SPRITE { get => _combatIconSprite; }

    [SerializeField] private Sprite _eliteIconSprite;
    public Sprite ELITE_ICON_SPRITE { get => _eliteIconSprite; }

    [SerializeField] private Sprite _eventIconSprite;
    public Sprite EVENT_ICON_SPRITE { get => _eventIconSprite; }

    [SerializeField] private Sprite _shopIconSprite;
    public Sprite SHOP_ICON_SPRITE { get => _shopIconSprite; }

    [SerializeField] private Sprite _restIconSprite;
    public Sprite REST_ICON_SPRITE { get => _restIconSprite; }


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
