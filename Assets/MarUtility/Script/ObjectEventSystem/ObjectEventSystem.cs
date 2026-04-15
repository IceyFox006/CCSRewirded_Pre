using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectEventSystem : MonoBehaviour
{
    //Select
    [SerializeField]
        private ObjectButton _firstSelected;
    public ObjectButton curHover = null;
    private List<ObjectButton> curSelected = new List<ObjectButton>();

    //Input
    [SerializeField]
        private bool _enableInputOnInitialize = true;
    [SerializeField]
        private InputActionAsset _inputActions;
    [SerializeField]
        private string moveActionPath = "MOVE";
    private InputAction move;
    private Vector2 moveDirection;
    [SerializeField]
        private string selectActionPath = "SELECT";
    private InputAction select;
    [SerializeField]
        private string confirmActionPath = "CONFIRM";
    private InputAction confirm;

    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        curHover = _firstSelected;
        _firstSelected.OnHoverEnter();

        InitializeInput();
        if (_enableInputOnInitialize) 
            EnableInput();

    }

    #region Input
    public void InitializeInput()
    {
        _inputActions.Enable();
        move = _inputActions.FindAction(moveActionPath);
        select = _inputActions.FindAction(selectActionPath);
        confirm = _inputActions.FindAction(confirmActionPath);
    }
    public void EnableInput()
    {
        move.performed += Move_performed;
        select.performed += Select_performed;
    }

    public void DisableInput()
    {
        move.performed -= Move_performed;
        select.performed -= Select_performed;
    }

    //Switches hover to button in direction.
    private void Move_performed(InputAction.CallbackContext obj)
    {
        moveDirection = move.ReadValue<Vector2>();

        //Switch Hover
        if (moveDirection == Vector2.up && CanMoveTo(curHover.Navigation.Up)) //Up
            SwitchHover(curHover.Navigation.Up);
        else if (moveDirection == Vector2.down && CanMoveTo(curHover.Navigation.Down)) //Down
            SwitchHover(curHover.Navigation.Down);
        else if (moveDirection == Vector2.left && CanMoveTo(curHover.Navigation.Left)) //Left
            SwitchHover(curHover.Navigation.Left);
        else if (moveDirection == Vector2.right && CanMoveTo(curHover.Navigation.Right)) //Right
            SwitchHover(curHover.Navigation.Right);
    }

    //Select if button is not already selected, deselect if it is.
    private void Select_performed(InputAction.CallbackContext obj)
    {
        if (!curHover.IsSelected)
            curHover.OnSelect();
        else
            curHover.OnDeselect();
    }
    #endregion

    //Switches which button is currently being hovered over.
    public void SwitchHover(ObjectButton bo)
    {
        curHover.OnHoverExit();
        curHover = bo;
        curHover.OnHoverEnter();
    }

    #region Check
    //Returns true if bo can be moved to.
    private bool CanMoveTo(ObjectButton bo)
    {
        return (bo != null && bo.Interactable);
    }
    #endregion
}
