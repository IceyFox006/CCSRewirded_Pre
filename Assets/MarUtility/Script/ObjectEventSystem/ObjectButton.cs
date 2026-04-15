using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class ObjectButton : MonoBehaviour
{
    [SerializeField]
        private bool _interactable = true;
    private bool isHovered = false;
    private bool isSelected = false;

    //Navigation
    [Header("Navigation")]
    [SerializeField]
        private OBNavigation _navigation;

    //Visual
    [Header("Visual")]
    [SerializeField]
        private OBVisualType _visual;
    private OBVisual curVisual;
    [SerializeField, ShowIf("_visual", OBVisualType.MATERIAL2D)]
        private OBVMaterial2D _materialVisual;

    //Event
    [Header("Event")]
    [SerializeField]
        private UnityEvent _onConfirm;
    [SerializeField]
        private UnityEvent _onSelect;
    [SerializeField]
        private UnityEvent _onDeselect;
    [SerializeField]
        private UnityEvent _onHoverEnter;
    [SerializeField]
        private UnityEvent _onHoverExit;

    #region GS
    public bool Interactable { get => _interactable; set => _interactable = value; }
    public OBNavigation Navigation { get => _navigation; set => _navigation = value; }
    public bool IsSelected { get => isSelected; set => isSelected = value; }

    #endregion
    #region Initialize
    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        InitializeVisual();
    }
    private void InitializeVisual()
    {
        switch (_visual)
        {
            case OBVisualType.COLOR: break;
            case OBVisualType.SPRITE: break;
            case OBVisualType.ANIMATION: break;
            case OBVisualType.MATERIAL2D: curVisual = _materialVisual; break;
        }
        curVisual.Initialize();
    }
    #endregion
    #region Event

    public void OnConfirm()
    {

    }
    public void OnSelect()
    {
        isSelected = true;

        curVisual.ApplySelect();
    }
    public void OnDeselect()
    {
        isSelected = false;

        if (isHovered)
            curVisual.ApplyHover();
        else
            curVisual.Reset();
    }
    public void OnHoverEnter()
    {
        isHovered = true;

        if (!isSelected)
            curVisual.ApplyHover();
    }
    public void OnHoverExit()
    {
        isHovered = false;

        if (!isSelected)
            curVisual.Reset();
    }
    #endregion
}
//=====================================================================================================================
[System.Serializable]
public class OBNavigation
{
    [SerializeField]
        private OBNavigationType _type;

    [SerializeField, AllowNesting, ShowIf("_type", OBNavigationType.EXPLICIT)]
        private ObjectButton _up;
    [SerializeField, AllowNesting, ShowIf("_type", OBNavigationType.EXPLICIT)]
        private ObjectButton _down;
    [SerializeField, AllowNesting, ShowIf("_type", OBNavigationType.EXPLICIT)]
        private ObjectButton _left;
    [SerializeField, AllowNesting, ShowIf("_type", OBNavigationType.EXPLICIT)]
        private ObjectButton _right;

    #region GS
    public ObjectButton Up { get => _up; set => _up = value; }
    public ObjectButton Down { get => _down; set => _down = value; }
    public ObjectButton Left { get => _left; set => _left = value; }
    public ObjectButton Right { get => _right; set => _right = value; }
    #endregion
}
public enum OBNavigationType
{
    EXPLICIT,
    //HORIZONTAL,
    //VERTICAL,
    //AUTOMATIC,
}
//=====================================================================================================================
[System.Serializable]
public class OBVisual
{
    public virtual void Initialize() { }
    public virtual void Reset() { }
    public virtual void ApplyHover() { }
    public virtual void ApplySelect() { }
}
public enum OBVisualType
{
    NONE,
    COLOR,
    SPRITE,
    ANIMATION,
    MATERIAL2D,
    MATERIAL3D,
}
//---------------------------------------------------------------------------------------------------------------------
[System.Serializable]
public class OBVMaterial2D : OBVisual
{
    [SerializeField]
        private SpriteRenderer _renderer;
    private Material defaultMaterial;

    [Header("Material")]
    [SerializeField, AllowNesting]
        private Material _hover;
    [SerializeField, AllowNesting]
        private Material _select;

    public override void Initialize()
    {
        defaultMaterial = _renderer.material;
    }
    public override void Reset()
    {
        _renderer.material = defaultMaterial;
    }

    public override void ApplyHover()
    {
        _renderer.material = _hover;
    }

    public override void ApplySelect()
    {
        _renderer.material = _select;
    }
}

