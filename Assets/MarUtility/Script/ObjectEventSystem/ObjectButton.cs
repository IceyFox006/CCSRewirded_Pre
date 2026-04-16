using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.Events;

public class ObjectButton : MonoBehaviour
{
    [SerializeField]
        private bool _interactable = true;
    private bool isHovered = false;
    private bool isSelected = false;

    //Navigation
    [BoxGroup("Navigation")]
    [SerializeField]
        private OBNavigation _navigation;

    //Visual
    [SerializeField, BoxGroup("Visual")]
        private OBVisualType _visualType;
    private OBVisual curVisual;
    [SerializeField, BoxGroup("Visual"), ShowIf("_visualType", OBVisualType.MATERIAL2D)]
        private OBVMaterial2D _materialVisual;

    //Event
    [SerializeField, EnumFlags, BoxGroup("Event")]
        private OBEventType _eventTypes;
    [SerializeField, BoxGroup("Event"), ShowIf("_eventTypes", OBEventType.CONFIRM)]
        private UnityEvent _onConfirm;
    [SerializeField, BoxGroup("Event"), ShowIf("_eventTypes", OBEventType.SELECT)]
        private UnityEvent _onSelect;
    [SerializeField, BoxGroup("Event"), ShowIf("_eventTypes", OBEventType.DESELECT)]
        private UnityEvent _onDeselect;
    [SerializeField, BoxGroup("Event"), ShowIf("_eventTypes", OBEventType.HOVER_ENTER)]
        private UnityEvent _onHoverEnter;
    [SerializeField, BoxGroup("Event"), ShowIf("_eventTypes", OBEventType.HOVER_EXIT)]
        private UnityEvent _onHoverExit;

    #region GS
    public bool Interactable { get => _interactable; set => _interactable = value; }
    public OBNavigation Navigation { get => _navigation; set => _navigation = value; }
    public bool IsSelected { get => isSelected; set => isSelected = value; }

    #endregion
    #region Initialize
    private void Awake() //~REMOVE
    {
        Initialize();
    }
    public void Initialize()
    {
        InitializeVisual();
    }
    private void InitializeVisual()
    {
        switch (_visualType)
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
    //Invoke confirm events and update visuals.
    public void OnConfirm()
    {
        _onConfirm.Invoke();
    }

    //Invoke select events and update visuals.
    public void OnSelect()
    {
        isSelected = true;

        _onSelect.Invoke();

        if (isHovered)
            curVisual.ApplyHoverSelect();
        else
            curVisual.ApplySelect();
    }

    //Invoke deselect events and update visuals.
    public void OnDeselect()
    {
        isSelected = false;

        _onDeselect.Invoke();
        if (isHovered)
            curVisual.ApplyHover();
        else
            curVisual.Reset();
    }

    //Invoke hover enter events and update visuals.
    public void OnHoverEnter()
    {
        isHovered = true;

        _onHoverEnter.Invoke();
        if (!isSelected)
            curVisual.ApplyHover();
        else
            curVisual.ApplyHoverSelect();
    }

    //Invoke hover exit events and update visuals.
    public void OnHoverExit()
    {
        isHovered = false;

        _onHoverExit.Invoke();
        if (!isSelected)
            curVisual.Reset();
        else
            curVisual.ApplySelect();
    }
    #endregion
}
[Flags]public enum OBEventType
{
    NONE = 000,
    CONFIRM = 100,
    SELECT = 200,
    DESELECT = 210,
    HOVER_ENTER = 300,
    HOVER_EXIT = 310,
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
    NONE = 000,
    EXPLICIT = 100,
    HORIZONTAL = 200,
    VERTICAL = 300,
    AUTOMATIC = 400,
}

//=====================================================================================================================
[System.Serializable]
public class OBVisual
{
    public virtual void Initialize() { }
    public virtual void Reset() { }
    public virtual void ApplyHover() { }
    public virtual void ApplySelect() { }
    public virtual void ApplyHoverSelect() { }
}
public enum OBVisualType
{
    NONE = 000,
    COLOR = 100,
    SPRITE = 200,
    ANIMATION = 300,
    MATERIAL2D = 400,
    MATERIAL3D = 410,
}
//---------------------------------------------------------------------------------------------------------------------
[System.Serializable]
public class OBVMaterial2D : OBVisual
{
    [SerializeField, AllowNesting, Required]
        private SpriteRenderer _renderer;
    private Material defaultMaterial;

    [Header("Material")]
    [SerializeField, AllowNesting]
        private Material _hover;
    [SerializeField, AllowNesting]
        private Material _select;
    [SerializeField, AllowNesting, Tooltip("Activated when hovering over a selected button. If this is not set, select will be activated.")]
        private Material _hoverSelect;

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
        if (_hover == null) return;

        _renderer.material = _hover;
    }

    public override void ApplySelect()
    {
        if (_select == null) return;

        _renderer.material = _select;
    }

    public override void ApplyHoverSelect()
    {
        if (_hoverSelect == null)
            _renderer.material = _select;
        else
            _renderer.material = _hoverSelect;
    }
}

