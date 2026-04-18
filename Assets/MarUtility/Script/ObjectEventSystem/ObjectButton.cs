using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [SerializeField, BoxGroup("Visual"), EnumFlags]
        private OBVisualType _visualType;
    [SerializeField, BoxGroup("Visual"), ShowIf("_visualType", OBVisualType.COLOR)]
        private OBVColor _colorVisual;
    [SerializeField, BoxGroup("Visual"), ShowIf("_visualType", OBVisualType.SPRITE)]
        private OBVSprite _spriteVisual;
    [SerializeField, BoxGroup("Visual"), ShowIf("_visualType", OBVisualType.ANIMATION)]
        private OBVAnimation _animationVisual;
    [SerializeField, BoxGroup("Visual"), ShowIf("_visualType", OBVisualType.MATERIAL2D)]
        private OBVMaterial2D _material2DVisual;
    private List<OBVisual> curVisuals = new List<OBVisual>();

    //Event
    [SerializeField, Tooltip("Invokes events at the end of the confirm visual instead of the beginning.")]
        private bool _invokeAtEndOfAnimation = true; //!UNIMPLEMENTED
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
        //Add visuals.
        if (_visualType.HasFlag(OBVisualType.COLOR))
            curVisuals.Add(_colorVisual);
        if (_visualType.HasFlag(OBVisualType.SPRITE))
            curVisuals.Add(_spriteVisual);
        if (_visualType.HasFlag(OBVisualType.ANIMATION))
            curVisuals.Add(_animationVisual);
        if (_visualType.HasFlag(OBVisualType.MATERIAL2D))
            curVisuals.Add(_material2DVisual);

        //Initialize visuals.
        foreach(OBVisual visual in curVisuals)
            visual.Initialize();
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
        {
            foreach (OBVisual visual in curVisuals)
                visual.ApplyHoverSelect();
        }
        else
        {
            foreach (OBVisual visual in curVisuals)
                visual.ApplySelect();
        }
    }

    //Invoke deselect events and update visuals.
    public void OnDeselect()
    {
        isSelected = false;

        _onDeselect.Invoke();
        if (isHovered)
        {
            foreach (OBVisual visual in curVisuals)
                visual.ApplyHover();
        }
        else
        {
            foreach (OBVisual visual in curVisuals)
                visual.Reset();
        }
    }

    //Invoke hover enter events and update visuals.
    public void OnHoverEnter()
    {
        isHovered = true;

        _onHoverEnter.Invoke();
        if (!isSelected)
        {
            foreach (OBVisual visual in curVisuals)
                visual.ApplyHover();
        }
        else
        {
            foreach (OBVisual visual in curVisuals)
                visual.ApplyHoverSelect();
        }
    }

    //Invoke hover exit events and update visuals.
    public void OnHoverExit()
    {
        isHovered = false;

        _onHoverExit.Invoke();
        if (!isSelected)
        {
            foreach (OBVisual visual in curVisuals)
                visual.Reset();
        }
        else
        {
            foreach (OBVisual visual in curVisuals)
                visual.ApplySelect();
        }
    }
    #endregion
}
[Flags]public enum OBEventType
{
    NONE = 0 << 000,
    CONFIRM = 1 << 100,
    SELECT = 1 << 200,
    DESELECT = 1 << 210,
    HOVER_ENTER = 1 << 300,
    HOVER_EXIT = 1 << 310,
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
[Flags]public enum OBVisualType
{
    NONE = 0 << 000,
    COLOR = 1 << 100,
    SPRITE = 1 << 200,
    ANIMATION = 1 << 300,
    MATERIAL2D = 1 << 400,
    MATERIAL3D = 1 << 410,
}
//---------------------------------------------------------------------------------------------------------------------
[System.Serializable]
public class OBVColor : OBVisual
{
    [SerializeField, AllowNesting, Required]
    private SpriteRenderer _renderer;
    private Color defaultColor;

    [Header("Color")]
    [SerializeField, AllowNesting]
    private Color _hover = Color.white;
    [SerializeField, AllowNesting]
    private Color _select = Color.white;
    [SerializeField, AllowNesting, Tooltip("Activated when hovering over a selected button. If this is not set, select will be activated.")]
    private Color _hoverSelect = Color.white;

    public override void Initialize()
    {
        defaultColor = _renderer.color;
    }

    public override void Reset()
    {
        _renderer.color = defaultColor;
    }

    public override void ApplyHover()
    {
        if (_hover == null) return;

        _renderer.color = _hover;
    }

    public override void ApplySelect()
    {
        if (_select == null) return;

        _renderer.color = _select;
    }

    public override void ApplyHoverSelect()
    {
        if (_hoverSelect == null)
            ApplySelect();
        else
            _renderer.color = _hoverSelect;
    }
}
//---------------------------------------------------------------------------------------------------------------------
[System.Serializable]
public class OBVSprite : OBVisual
{
    [SerializeField, AllowNesting, Required]
        private SpriteRenderer _renderer;
    private Sprite defaultSprite;

    [Header("Sprite")]
    [SerializeField, AllowNesting]
        private Sprite _hover;
    [SerializeField, AllowNesting]
        private Sprite _select;
    [SerializeField, AllowNesting, Tooltip("Activated when hovering over a selected button. If this is not set, select will be activated.")]
        private Sprite _hoverSelect;

    public override void Initialize()
    {
        defaultSprite = _renderer.sprite;
    }

    public override void Reset()
    {
        _renderer.sprite = defaultSprite;
    }

    public override void ApplyHover()
    {
        if (_hover == null) return;

        _renderer.sprite = _hover;
    }

    public override void ApplySelect()
    {
        if (_select == null) return;

        _renderer.sprite = _select;
    }

    public override void ApplyHoverSelect()
    {
        if (_hoverSelect == null)
            ApplySelect();
        else
            _renderer.sprite = _hoverSelect;
    }
}
//---------------------------------------------------------------------------------------------------------------------
[System.Serializable]
public class OBVAnimation : OBVisual
{
    [SerializeField, AllowNesting, Required]
        private Animator _animator;
    [SerializeField, AllowNesting, Required, OnValueChanged("OnVCC_AnimatorOC"), InspectorName("Animation OC"), Tooltip("Must override the \"OBJECT_BUTTON_AC\".")]
        private AnimatorOverrideController _animatorOC;

    public override void Initialize()
    {
        _animator.runtimeAnimatorController = _animatorOC;
    }

    public override void Reset()
    {
        _animator.SetBool("IS_HOVERED", false);
        _animator.SetBool("IS_SELECTED", false);
    }

    public override void ApplyHover()
    {
        _animator.SetBool("IS_HOVERED", true);
        _animator.SetBool("IS_SELECTED", false);
    }


    public override void ApplySelect()
    {
        _animator.SetBool("IS_HOVERED", false);
        _animator.SetBool("IS_SELECTED", true);
    }

    public override void ApplyHoverSelect()
    {
        _animator.SetBool("IS_HOVERED", true);
        _animator.SetBool("IS_SELECTED", true);
    }

    #region Inspector
    private void OnVCC_AnimatorOC()
    {
        if (_animatorOC == null) return;

        if (!_animatorOC.runtimeAnimatorController.name.Equals("OBJECT_BUTTON_AC"))
        {
            Debug.LogError(_animator.gameObject.name + "'s AnimationOC must be an animator override controller of \"OBJECT_BUTTON_AC\".");
            _animatorOC = null;
            return;
        }
    }
    #endregion
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
            ApplySelect();
        else
            _renderer.material = _hoverSelect;
    }
}


