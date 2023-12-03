using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [SerializeField]
    float m_InputSensitivity = 1.5f;

    private bool ignoreFirstPressUp = true;
    private float ignoreTime = 1;

    bool started = false;

    private Vector2 oldInputPos;
    private Vector2 direction;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (CharacterBehaviour.instance == null)
        {
            return;
        }

        //SwipeMoving();
        SwipeMovement();
    }

    private void SwipeMovement()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // get first touch since touch count is greater than zero

            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                if (GameManager.instance.IsInEndGameStatus())
                    EndGameCharacterBehaviour.instance.Move(new Vector3(touch.deltaPosition.x * m_InputSensitivity, 0, 0));

                else
                    CharacterBehaviour.instance.Move(new Vector3(touch.deltaPosition.x * m_InputSensitivity, 0, 0));
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (GameManager.instance.IsInEndGameStatus())
                    EndGameCharacterBehaviour.instance.ResetRotAnimation();

                else
                    CharacterBehaviour.instance.Move(Vector3.zero);
            }
        }
    }

    private void SwipeMoving()
    {
        //start
        if (Input.GetMouseButtonDown(0))
        {
            oldInputPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        //moving
        if (Input.GetMouseButton(0))
        {
            direction = (new Vector2(Input.mousePosition.x, Input.mousePosition.y) - oldInputPos).normalized;

            if(GameManager.instance.IsInEndGameStatus())            
                EndGameCharacterBehaviour.instance.Move(new Vector3(direction.x * m_InputSensitivity, 0, direction.y));

            else
                CharacterBehaviour.instance.Move(new Vector3(direction.x * m_InputSensitivity, 0, direction.y));
            
            oldInputPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        //ending
        if (Input.GetMouseButtonUp(0))
        {
            oldInputPos = Vector2.zero;
            direction = Vector3.zero;

            if (GameManager.instance.IsInEndGameStatus())
                EndGameCharacterBehaviour.instance.ResetRotAnimation();

            else
                CharacterBehaviour.instance.Move(Vector3.zero);
        }
    }

    public void FirstInput()
    {
        if (!started)
        {
            StartCoroutine(IgnoreFirstUpDelay());

            started = true;
        }
    }

    private IEnumerator IgnoreFirstUpDelay()
    {
        yield return new WaitForSeconds(ignoreTime);

        ignoreFirstPressUp = false;
    }
}