using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private float horizontalRawInput;
    private float verticalRawInput;

    private ButtonState jumpButtonState;
    private ButtonState switchButtonState;
    private ButtonState upButtonState;
    private ButtonState downButtonState;
    private ButtonState leftButtonState;
    private ButtonState rightButtonState;

    private Dictionary<KeyCode, ButtonState> keyStates;

    private float internalTimer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        keyStates = new Dictionary<KeyCode, ButtonState>()
        {
            {KeyCode.Space, jumpButtonState},
            {KeyCode.LeftShift, switchButtonState},
            {KeyCode.UpArrow, upButtonState},
            {KeyCode.DownArrow, downButtonState},
            {KeyCode.LeftArrow, leftButtonState},
            {KeyCode.RightArrow, rightButtonState},
        };

        Dictionary<KeyCode, ButtonState> newKeyStates = new Dictionary<KeyCode, ButtonState>();
        foreach (KeyValuePair<KeyCode, ButtonState> state in keyStates)
        {
            KeyCode keyCode = state.Key;
            ButtonState buttonState = state.Value;

            buttonState.timeAtLastDown = Mathf.NegativeInfinity;
            buttonState.timeAtLastUp = Mathf.NegativeInfinity;

            newKeyStates[keyCode] = buttonState;
        }
        keyStates = newKeyStates;

        internalTimer = 0;
    }

    private void UpdateKeyStates()
    {
        Dictionary<KeyCode, ButtonState> newKeyStates = new Dictionary<KeyCode, ButtonState>();
        foreach (KeyValuePair<KeyCode, ButtonState> state in keyStates)
        {
            KeyCode keyCode = state.Key;
            ButtonState buttonState = state.Value;

            if (IsDown(keyCode))
            {
                buttonState.timeAtLastDown = internalTimer;
            }
            if (IsUp(keyCode))
            {
                buttonState.timeAtLastUp = internalTimer;
            }

            newKeyStates[keyCode] = buttonState;
        }
        keyStates = newKeyStates;
    }

    private void CalculateAxes()
    {
        bool left = IsPressed(KeyCode.LeftArrow) || IsPressed(KeyCode.A);
        bool right = IsPressed(KeyCode.RightArrow) || IsPressed(KeyCode.D);
        bool up = IsPressed(KeyCode.UpArrow) || IsPressed(KeyCode.W);
        bool down = IsPressed(KeyCode.DownArrow) || IsPressed(KeyCode.S);

        horizontalRawInput = (left ? -1 : 0) + (right ? 1 : 0);
        verticalRawInput = (down ? -1 : 0) + (up ? 1 : 0);
    }

    private void Update()
    {
        UpdateKeyStates();
        CalculateAxes();

        internalTimer += Time.unscaledDeltaTime;
    }

    public float GetHorizontalRaw() => horizontalRawInput;
    public float GetVerticalRaw() => verticalRawInput;

    public bool IsPressed(KeyCode key) => Input.GetKey(key);
    public bool IsDown(KeyCode key) => Input.GetKeyDown(key);
    public bool IsUp(KeyCode key) => Input.GetKeyUp(key);

    public float SecondsSincePressed(KeyCode key)
    {
        if (keyStates.ContainsKey(key))
        {
            return internalTimer - keyStates[key].timeAtLastDown;
        }
        else
        {
            throw new System.Exception($"Key {key} not bound to anything!");
        }
    }

    public void ConsumeBuffer(KeyCode key)
    {
        ButtonState state = keyStates[key];
        state.timeAtLastDown = Mathf.NegativeInfinity;
        keyStates[key] = state;
    }

    private struct ButtonState
    {
        public float timeAtLastDown;
        public float timeAtLastUp;
    }
}
