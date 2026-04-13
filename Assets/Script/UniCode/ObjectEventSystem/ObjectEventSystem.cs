using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectEventSystem : MonoBehaviour
{
    //Select
    [SerializeField]
        private ObjectButton _firstSelected;
    private ObjectButton curHover = null;
    private List<ObjectButton> curSelected = new List<ObjectButton>();

    //Input
    [SerializeField]
        private bool _enableInputOnInitialize = true;
    [SerializeField]
        private InputActionAsset _inputActions;
    private InputAction move;
    private Vector2 moveDirection;

    public void Initialize()
    {
        curHover = _firstSelected;

        if (_enableInputOnInitialize) 
            EnableInput();

    }

    #region Input
    public void EnableInput()
    {
        move = _inputActions.FindAction("MOVE");

        move.performed += Move_performed;
    }
    public void DisableInput()
    {
        move.performed -= Move_performed;
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        moveDirection = move.ReadValue<Vector2Int>();

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
    #endregion

    public void SwitchHover(ObjectButton bo)
    {
        curHover.OnHoverExit();
        curHover = bo;
        curHover.OnHoverEnter();
    }

    #region Check
    private bool CanMoveTo(ObjectButton bo)
    {
        return (bo != null && !bo.Interactable);
    }
    #endregion
}
