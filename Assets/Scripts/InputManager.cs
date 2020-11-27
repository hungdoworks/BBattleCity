using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomEvents;

public class InputManager : MonoBehaviour
{
    public event NoArgumentEventHandler OnGamePauseResumed;
    public event NoArgumentEventHandler PlayerFired;

    public float horizontalInput { get; private set; }
    public float verticalInput { get; private set; }
    public Vector2 move { get; private set; }

    public bool useDebugControl = true;

    private bool blockInput = false;
    private bool pauseResumeGame = false;
    private bool playerLaunch = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (pauseResumeGame)
        {
            pauseResumeGame = false;

            blockInput = !blockInput;

            if (OnGamePauseResumed != null)
                OnGamePauseResumed();
        }

        if (blockInput)
        {
            horizontalInput = 0.0f;
            verticalInput = 0.0f;

            return;
        }

        if (useDebugControl)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (PlayerFired != null)
                    PlayerFired();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (OnGamePauseResumed != null)
                    OnGamePauseResumed();
            }
        }

        if (horizontalInput * verticalInput != 0.0f)
            horizontalInput = verticalInput = 0.0f;

        move = new Vector2(horizontalInput, verticalInput);

        if (playerLaunch)
        {
            playerLaunch = false;

            if (PlayerFired != null)
                PlayerFired();
        }
    }

    public void Launch()
    {
        playerLaunch = true;
    }

    public void PauseResumeGame()
    {
        pauseResumeGame = !pauseResumeGame;
    }

    public void MoveHorizontal(float direction)
    {
        horizontalInput = direction;
    }

    public void MoveVertical(float direction)
    {
        verticalInput = direction;
    }
}
