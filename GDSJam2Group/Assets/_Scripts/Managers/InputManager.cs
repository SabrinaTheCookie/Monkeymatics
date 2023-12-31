using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputManager : Singleton<InputManager>
{
    [FormerlySerializedAs("lockMouse")] public bool lockCursor;
    private PlayerInput playerInput;
    [SerializeField] private Vector2 lookDelta;
    [SerializeField] private Vector2 pointerPos;

    public static event Action OnPrimaryPressed;
    public static event Action OnSecondaryPressed;
    public static event Action<Vector2> OnLookUpdated;
    public static event Action<Vector2> OnPointerUpdated;

    //Left Upper
    public bool leftUpper;
    public static event Action<bool> onLeftUpper;
    //Left Lower
    public bool leftLower;
    public static event Action<bool> onLeftLower;
    //Right Lower
    public bool rightLower;
    public static event Action<bool> onRightLower;
    //Right Upper
    public bool rightUpper;
    public static event Action<bool> onRightUpper;
    //Tail
    public bool tail;
    public static event Action<bool> onTail;
    //Expire
    public bool expire;
    public static event Action<bool> onExpire;



    protected override void Awake()
    {
        base.Awake();
        if(playerInput == null) playerInput = GetComponent<PlayerInput>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (!playerInput.camera)
            playerInput.camera = Camera.main;

        if (lockCursor)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.Confined;


    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentControlScheme != "Gamepad") return;

        Vector2 pos = context.ReadValue<Vector2>();
        lookDelta = pos;
        OnLookUpdated?.Invoke(lookDelta);
    }

    public void OnPointerUpdate(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInput.currentControlScheme != "Keyboard&Mouse") return;

        Vector2 pos = context.ReadValue<Vector2>();
        //Min of 0,0
        pointerPos = Vector2.Max(pos, Vector2.zero);
        //Normalize
        pointerPos /= new Vector2(Screen.width, Screen.height);
        //Max of 1,1
        pointerPos = Vector2.Min(pointerPos, Vector2.one);
        //Range of -0.5-0.5
        pointerPos -= Vector2.one / 2;
        OnPointerUpdated?.Invoke(pointerPos);

    }

    public void OnPrimary(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnPrimaryPressed?.Invoke();
        }
        else if (context.canceled)
        {

        }
            
    }public void OnSecondary(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSecondaryPressed?.Invoke();
        }
        else if (context.canceled)
        {

        }
            
    }

    public void OnLeftUpper(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            leftUpper = true;
        }
        else if (context.canceled)
        {
            leftUpper = false;
        }
        onLeftUpper?.Invoke(leftUpper);
    }

    public void OnLeftLower(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            leftLower = true;
        }   
        else if (context.canceled)
        {
            leftLower = false;
        }
        onLeftLower?.Invoke(leftLower);

    }

    public void OnRightUpper(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rightUpper = true;
        }
        else if (context.canceled)
        {
            rightUpper = false;
        }
        onRightUpper?.Invoke(rightUpper);

    }

    public void OnRightLower(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rightLower = true;
        }
        else if (context.canceled)
        {
            rightLower = false;
        }
        onRightLower?.Invoke(rightLower);

    }

    public void OnTail(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tail = true;
        }
        else if (context.canceled)
        {
            tail = false;
        }
        onTail?.Invoke(tail);

    }
    public void OnExpire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            expire = true;
        }
        else if (context.canceled)
        {
            expire = false;
        }
        onExpire?.Invoke(expire);

    }




}
