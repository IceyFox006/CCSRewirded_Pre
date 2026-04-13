using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class ObjectButton : MonoBehaviour
{
    [SerializeField]
        private bool _interactable = true;

    //Navigation
    [Header("Navigation")]
    [SerializeField]
        private OBNavigation _navigation;

    //Visual
    [Header("Visual")]
    [SerializeField]
        private OBVisualType _visual;
    [SerializeField, ShowIf("_visual", OBVisualType.MATERIAL)]
        private OBVMaterial _materialVisual;


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

    #endregion
    #region Event

    public void OnConfirm()
    {

    }
    public void OnSelect()
    {

    }
    public void OnDeselect()
    {

    }
    public void OnHoverEnter()
    {

    }
    public void OnHoverExit()
    {

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
    MATERIAL,
}
//---------------------------------------------------------------------------------------------------------------------
[System.Serializable]
public class OBVMaterial : OBVisual
{
    [SerializeField]
        private SpriteRenderer _renderer;

    [Header("Material")]
    [SerializeField, AllowNesting]
        private Material _hover;
    [SerializeField, AllowNesting]
        private Material _select;

    public override void Reset()
    {
        base.Reset();
    }

    public override void ApplyHover()
    {
        base.ApplyHover();
    }

    public override void ApplySelect()
    {
        base.ApplySelect();
    }
}

