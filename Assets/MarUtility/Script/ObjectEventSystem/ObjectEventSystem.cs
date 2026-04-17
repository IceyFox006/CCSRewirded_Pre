using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectEventSystem : MonoBehaviour
{
    //SELECTION
    [SerializeField, BoxGroup("Selection")]
        private ObjectButton _firstSelected;

    [SerializeField, BoxGroup("Selection"), MinValue(0), OnValueChanged("OnVCC_IndexReplaced"),Tooltip("The max number of buttons that can be selected at once.")]
        private int _maxNumSelected = 1;
    //Confirm on Select
    private bool showConfirmOnSelect;
    [SerializeField, BoxGroup("Selection")]
        private bool _confirmOnSelect = false; //!UNIMPLEMENTED
    //Replace Select
    [SerializeField, BoxGroup("Selection"), Tooltip("Instead of preventing selection, deselects one of the selected buttons, to select the curHover on select.")]
        private bool _replaceSelection = false;
    [SerializeField, BoxGroup("Selection"), MinValue(0), ShowIf("_replaceSelection"), OnValueChanged("OnVCC_IndexReplaced"), Tooltip("The index of the button that will be deselected.")]
        private int _indexReplaced = 0;
    //Deselect Confirm
    [SerializeField, BoxGroup("Selection"), Tooltip("When confirmed, all selected buttons will be deselected.")]
        private bool _deselectOnConfirm = true;

    [ShowNonSerializedField]
        private ObjectButton curHover = null;
    private List<ObjectButton> curSelected = new List<ObjectButton>();

    //INPUT
    [SerializeField, BoxGroup("Input")]
        private bool _enableInputOnInitialize = true;
    [SerializeField, BoxGroup("Input")]
        private InputActionAsset _inputActions;
    //Move
    [SerializeField, BoxGroup("Input")]
        private string moveActionPath = "MOVE";
    private InputAction move;
    private Vector2 moveDirection;
    //Select
    [SerializeField, BoxGroup("Input")]
        private string selectActionPath = "SELECT";
    private InputAction select;
    //Confirm
    [SerializeField, BoxGroup("Input")]
        private string confirmActionPath = "CONFIRM";
    private InputAction confirm;

    private void Start() //~REMOVE
    {
        Initialize();
    }
    private void OnDestroy()
    {
        DisableInput();
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
    //Assigns actions to inputs.
    public void InitializeInput()
    {
        _inputActions.Enable();
        move = _inputActions.FindAction(moveActionPath);
        select = _inputActions.FindAction(selectActionPath);
        confirm = _inputActions.FindAction(confirmActionPath);
    }

    //Add input listeners.
    public void EnableInput()
    {
        move.performed += Move_performed;
        select.performed += Select_performed;
        confirm.performed += Confirm_performed;
    }

    //Remove input listeners.
    public void DisableInput()
    {
        move.performed -= Move_performed;
        select.performed -= Select_performed;
        confirm.performed -= Confirm_performed;
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
        if (curHover.IsSelected) //Deselect if selected.
        {
            RemoveSelected(curHover);
            return; //Deselected piece.
        }

        if (curSelected.Count < _maxNumSelected) //Select if there is room.
            AddSelected(curHover);
        else
        {
            if (_replaceSelection)
            {
                RemoveSelected(curSelected[_indexReplaced]);
                AddSelected(curHover);
            }
        }
    }

    //Confirm button.
    private void Confirm_performed(InputAction.CallbackContext obj)
    {
       for (int i = curSelected.Count - 1; i >= 0; i--)
        {
            curSelected[i].OnConfirm();

            if (_deselectOnConfirm)
                RemoveSelected(curSelected[i]);
            Debug.Log("Confirmed");
        }
    }
    #endregion

    #region Selection Management
    //Switches which button is currently being hovered over.
    public void SwitchHover(ObjectButton ob)
    {
        curHover.OnHoverExit();
        curHover = ob;
        curHover.OnHoverEnter();
    }

    //Adds ob to curSelected and selects it.
    private void AddSelected(ObjectButton ob)
    {
        ob.OnSelect();
        curSelected.Add(ob);
    }

    //Removes ob from curSelected and deselects it.
    private void RemoveSelected(ObjectButton ob)
    {
        ob.OnDeselect();
        curSelected.Remove(ob);
    }
    #endregion

    #region Check
    //Returns true if bo can be moved to.
    private bool CanMoveTo(ObjectButton bo)
            => (bo != null && bo.Interactable);
    #endregion

    #region Inspector
    private void OnVCC_IndexReplaced()
    {
        if (_indexReplaced >= _maxNumSelected)
            _indexReplaced = _maxNumSelected;
    }
    #endregion
}
