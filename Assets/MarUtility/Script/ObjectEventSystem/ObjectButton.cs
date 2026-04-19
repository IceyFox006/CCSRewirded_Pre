using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField, BoxGroup("Visual"), ShowIf("_visualType", OBVisualType.COLOR_SPRITE)]
        private OBVColorSprite _colorSpriteVisual;
    [SerializeField, BoxGroup("Visual"), ShowIf("_visualType", OBVisualType.SPRITE)]
        private OBVSprite _spriteVisual;
    [SerializeField, BoxGroup("Visual"), ShowIf("_visualType", OBVisualType.ANIMATION)]
        private OBVAnimation _animationVisual;
    [SerializeField, BoxGroup("Visual"), ShowIf("_visualType", OBVisualType.MATERIAL2D)]
        private OBVMaterial2D _material2DVisual;
    private List<OBVisual> curVisuals = new List<OBVisual>();

    //Event
    [SerializeField, Tooltip("Invokes events at the end of the confirm visual instead of the beginning.")]
        private bool _invokeAtEndOfConfirmVisual = true; //!UNIMPLEMENTED
    [SerializeField, EnumFlags, BoxGroup("Event")]
        private OBEventType _eventTypes;
    [SerializeField, BoxGroup("Event"), ShowIf("_eventTypes", OBEventType.CONFIRM)]
        private UnityEvent _onConfirmEvents;
    [SerializeField, BoxGroup("Event"), ShowIf("_eventTypes", OBEventType.SELECT)]
        private UnityEvent _onSelectEvents;
    [SerializeField, BoxGroup("Event"), ShowIf("_eventTypes", OBEventType.DESELECT)]
        private UnityEvent _onDeselectEvents;
    [SerializeField, BoxGroup("Event"), ShowIf("_eventTypes", OBEventType.HOVER_ENTER)]
        private UnityEvent _onHoverEnterEvents;
    [SerializeField, BoxGroup("Event"), ShowIf("_eventTypes", OBEventType.HOVER_EXIT)]
        private UnityEvent _onHoverExitEvents;

    #region GS
    public bool Interactable { get => _interactable; set => _interactable = value; }
    public OBNavigation Navigation { get => _navigation; set => _navigation = value; }
    public bool IsSelected { get => isSelected; set => isSelected = value; }
    public UnityEvent OnConfirmEvents { get => _onConfirmEvents; set => _onConfirmEvents = value; }

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
        if (_visualType.HasFlag(OBVisualType.COLOR_SPRITE))
            curVisuals.Add(_colorSpriteVisual);
        if (_visualType.HasFlag(OBVisualType.SPRITE))
            curVisuals.Add(_spriteVisual);
        if (_visualType.HasFlag(OBVisualType.ANIMATION))
            curVisuals.Add(_animationVisual);
        if (_visualType.HasFlag(OBVisualType.MATERIAL2D))
            curVisuals.Add(_material2DVisual);

        //Initialize visuals.
        foreach(OBVisual visual in curVisuals)
            visual.Initialize(this);
    }
    #endregion
    #region Event
    //Invoke confirm events and update visuals.
    public void OnConfirm()
    {
        if (!_invokeAtEndOfConfirmVisual)
            _onConfirmEvents.Invoke();

        foreach (OBVisual visual in curVisuals)
            StartCoroutine(ConfirmCD(visual));
    }

    //Invoke select events and update visuals.
    public void OnSelect()
    {
        isSelected = true;

        _onSelectEvents.Invoke();

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

        _onDeselectEvents.Invoke();
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

        _onHoverEnterEvents.Invoke();
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

        _onHoverExitEvents.Invoke();
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

    private IEnumerator ConfirmCD(OBVisual visual)
    {
        visual.ApplyConfirm();

        yield return new WaitForSeconds(visual.ConfirmVisualDuration);

        if (_invokeAtEndOfConfirmVisual)
            visual.Ob.OnConfirmEvents.Invoke();

        if (isHovered && isSelected)
            visual.ApplyHoverSelect();
        else if (isHovered)
            visual.ApplyHover();
        else if (isSelected)
            visual.ApplySelect();
        else
            visual.Reset();
    }
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
    protected ObjectButton ob;
    [SerializeField, AllowNesting, MinValue(0), ShowIf("ShowIf_ConfirmVisualDuration"), Tooltip("How many seconds the confirm visual lasts.")]
        protected float _confirmVisualDuration = 0.1f;

    #region GS
    public float ConfirmVisualDuration { get => _confirmVisualDuration; set => _confirmVisualDuration = value; }
    public ObjectButton Ob { get => ob; set => ob = value; }
    #endregion

    public virtual void Initialize(ObjectButton ob) 
    {
        this.ob = ob;
    }
    public virtual void Reset() { }
    public virtual void ApplyHover() { }
    public virtual void ApplySelect() { }
    public virtual void ApplyHoverSelect() { }
    public virtual void ApplyConfirm() { }

    #region Inspector
    private bool ShowIf_ConfirmVisualDuration()
    {
        if (GetType().Equals(typeof(OBVColorSprite))) return true;
        if (GetType().Equals(typeof(OBVSprite))) return true;
        if (GetType().Equals(typeof(OBVMaterial2D))) return true;

        return false;
    }
    #endregion
}
[Flags]public enum OBVisualType
{
    NONE = 0 << 000,
    COLOR_SPRITE = 1 << 100,
    COLOR_IMAGE = 1 << 110,
    SPRITE = 1 << 200,
    ANIMATION = 1 << 300,
    MATERIAL2D = 1 << 400,
    MATERIAL3D = 1 << 410, //!UNIMPLEMENTED
}
//---------------------------------------------------------------------------------------------------------------------
[System.Serializable]
public class OBVColor : OBVisual
{
    protected Color defaultColor;

    [Header("Color")]
    [SerializeField, AllowNesting]
        protected Color _hover = Color.gray8;
    [SerializeField, AllowNesting]
        protected Color _select = Color.gray6;
    [SerializeField, AllowNesting, Tooltip("Activated when hovering over a selected button. If this is not set, select will be activated.")]
        protected Color _hoverSelect = Color.gray4;
    [SerializeField, AllowNesting]
        protected Color _confirm = Color.gray2;
}
//---------------------------------------------------------------------------------------------------------------------
[System.Serializable]
public class OBVColorSprite : OBVColor
{
    [SerializeField, AllowNesting, Required]
        private SpriteRenderer _renderer;

    public override void Initialize(ObjectButton ob)
    {
        base.Initialize(ob);
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
    public override void ApplyConfirm()
    {
        _renderer.color = _confirm;
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
    [SerializeField, AllowNesting]
        private Sprite _confirm;

    public override void Initialize(ObjectButton ob)
    {
        base.Initialize(ob);
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

    public override void ApplyConfirm()
    {
        if (_confirm == null) return;

        _renderer.sprite = _confirm;
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

    public override void Initialize(ObjectButton ob)
    {
        base.Initialize(ob);
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

    public override void ApplyConfirm()
    {
        _animator.Play("CONFIRM");
        _confirmVisualDuration = _animator.GetCurrentAnimatorClipInfo(0).Length;
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
    [SerializeField, AllowNesting]
        private Material _confirm;

    public override void Initialize(ObjectButton ob)
    {
        base.Initialize(ob);
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
    public override void ApplyConfirm()
    {
        if (_confirm == null) return;

        _renderer.material = _confirm;
    }
}


