using UnityEngine;
using UnityEngine.InputSystem;

public class LaneMovement : MonoBehaviour
{
    private const float LaneWidth = 2f;
    private const float MoveSpeed = 12f;
    private const float RepeatDelay = 0.2f;

    private int lane = 1;
    private int heldDirection;
    private int queuedDirection;
    private float holdTime;
    private float fixedY;
    private float fixedZ;
    private bool moving;

    private void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
        transform.position = new Vector3(0f, fixedY, fixedZ);
    }

    private void OnDisable()
    {
        heldDirection = 0;
        queuedDirection = 0;
        holdTime = 0f;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            bool leftPressed = keyboard.leftArrowKey.wasPressedThisFrame;
            bool rightPressed = keyboard.rightArrowKey.wasPressedThisFrame;

            if (leftPressed != rightPressed)
            {
                heldDirection = leftPressed ? -1 : 1;
                queuedDirection = heldDirection;
                holdTime = 0f;
            }

            if ((heldDirection == -1 && !keyboard.leftArrowKey.isPressed) ||
                (heldDirection == 1 && !keyboard.rightArrowKey.isPressed))
            {
                heldDirection = 0;
                holdTime = 0f;
            }
        }
        else
        {
            heldDirection = 0;
            holdTime = 0f;
        }

        if (moving)
        {
            float targetX = (lane - 1) * LaneWidth;
            float x = Mathf.MoveTowards(transform.position.x, targetX, MoveSpeed * Time.deltaTime);
            transform.position = new Vector3(x, fixedY, fixedZ);

            if (x != targetX)
                return;

            moving = false;
            holdTime = 0f;
            if (queuedDirection == 0)
                return;
        }

        if (queuedDirection != 0)
        {
            int direction = queuedDirection;
            queuedDirection = 0;
            BeginMove(direction);
        }
        else if (heldDirection != 0 && !moving)
        {
            holdTime += Time.deltaTime;
            if (holdTime >= RepeatDelay)
            {
                holdTime = 0f;
                BeginMove(heldDirection);
            }
        }
    }

    private void BeginMove(int direction)
    {
        int nextLane = Mathf.Clamp(lane + direction, -2, 3 );
        if (nextLane != lane)
        {
            lane = nextLane;
            moving = true;
        }
    }
}
