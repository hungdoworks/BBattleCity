using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelStats
{
    public int levelRequired;
    public int health;
    public float speed;
    public float gunCooldown;
    public float gunPower;
}

public class Player : TankBaseBehavior
{
    public MovingAnimation[] movingAnimations;

    public AudioClip playerIdleSound;
    public AudioClip playerMovingSound;

    public int maxPlayerLevel = 4;

    private AnimatorOverrideController animatorOverrideController;
    private AnimationClipOverrides clipOverrides;

    private InputManager inputManager;

    private AudioSource audioPlayer;
    private bool startUpSoundFinished = false;

    // For demo only
    private PlayerLevelStats[] predefinedPlayerStats = new PlayerLevelStats[4];

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        isDestroyOnDead = false;

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);

        audioPlayer = GetComponent<AudioSource>();

        StartCoroutine(WaitForStartUpSoundFinished());

        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        inputManager.PlayerFired += Launch;

        // For demo only
        CreatePredefinedLevelStats();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Vector2 move = inputManager.move;

        // Update look direction based on move direction
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            // ChangeDirection(move);
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();

            animator.SetFloat("Horizontal", inputManager.horizontalInput);
            animator.SetFloat("Vertical", inputManager.verticalInput);

            if (startUpSoundFinished && audioPlayer.clip.name != playerMovingSound.name)
            {
                audioPlayer.clip = playerMovingSound;
                audioPlayer.Play();
            }
        }
        else
        {
            if (startUpSoundFinished && audioPlayer.clip.name != playerIdleSound.name)
            {
                audioPlayer.clip = playerIdleSound;
                audioPlayer.Play();
            }
        }
    }

    void FixedUpdate()
    {
        bool canMove = true;
        Vector2 position = rigidbody2d.position;

        position.x = position.x + GetFinalSpeed() * inputManager.horizontalInput * Time.deltaTime;
        position.y = position.y + GetFinalSpeed() * inputManager.verticalInput * Time.deltaTime;

        if (inputManager.horizontalInput != 0 || inputManager.verticalInput != 0)
        {
            canMove = TryMoveForward(position);
        }

        if (canMove)
            rigidbody2d.MovePosition(position);
    }

    void UpdateStatsByLevel()
    {
        foreach (PlayerLevelStats stats in predefinedPlayerStats)
        {
            if (stats.levelRequired == level)
            {
                gunCooldown = stats.gunCooldown;
                gunPower = stats.gunPower;
                health = stats.health;
                speed = stats.speed;
            }
        }
    }

    void UpdateAnimationByLevel()
    {
        // Update sprite of player based on player's level
        int animIndex = level - 1;
        clipOverrides["PlayerMoving_up"] = movingAnimations[animIndex].moveUp;
        clipOverrides["PlayerMoving_left"] = movingAnimations[animIndex].moveLeft;
        clipOverrides["PlayerMoving_down"] = movingAnimations[animIndex].moveDown;
        clipOverrides["PlayerMoving_right"] = movingAnimations[animIndex].moveRight;
        animatorOverrideController.ApplyOverrides(clipOverrides);
    }

    IEnumerator WaitForStartUpSoundFinished()
    {
        audioPlayer.Play();

        yield return new WaitForSeconds(audioPlayer.clip.length);

        startUpSoundFinished = true;

        audioPlayer.loop = true;
    }

    public void ChangeLevel(int amount)
    {
        level += amount;

        if (level < 1)
            level = 1;
        else if (level > maxPlayerLevel)
            level = maxPlayerLevel;

        UpdateStatsByLevel();
        UpdateAnimationByLevel();

        // Workaround for position bug when change sprite after level up.
        Vector2 pos = rigidbody2d.position;
        pos.x = Mathf.Round(pos.x * 10) / 10;
        pos.y = Mathf.Round(pos.y * 10) / 10;

        rigidbody2d.position = pos;
    }

    public void Launch()
    {
        if (isGunCooldown == false)
            SoundManager.Instance.PlayerLaunch();

        Launch(LayerMask.NameToLayer("PlayerBullet"));
    }

    public void ToggleSound()
    {
        if (audioPlayer.isPlaying)
            audioPlayer.Pause();
        else
            audioPlayer.UnPause();
    }

    public void Reset()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        lookDirection.Set(0, 1);
    }

    // For demo only
    void CreatePredefinedLevelStats()
    {
        predefinedPlayerStats[0] = new PlayerLevelStats();
        predefinedPlayerStats[0].levelRequired = 1;
        predefinedPlayerStats[0].gunCooldown = 1;
        predefinedPlayerStats[0].gunPower = 300;
        predefinedPlayerStats[0].health = 1;
        predefinedPlayerStats[0].speed = 3;

        predefinedPlayerStats[1] = new PlayerLevelStats();
        predefinedPlayerStats[1].levelRequired = 2;
        predefinedPlayerStats[1].gunCooldown = 0.8f;
        predefinedPlayerStats[1].gunPower = 500;
        predefinedPlayerStats[1].health = 1;
        predefinedPlayerStats[1].speed = 5;

        predefinedPlayerStats[2] = new PlayerLevelStats();
        predefinedPlayerStats[2].levelRequired = 3;
        predefinedPlayerStats[2].gunCooldown = 0.75f;
        predefinedPlayerStats[2].gunPower = 500;
        predefinedPlayerStats[2].health = 2;
        predefinedPlayerStats[2].speed = 5;

        predefinedPlayerStats[3] = new PlayerLevelStats();
        predefinedPlayerStats[3].levelRequired = 4;
        predefinedPlayerStats[3].gunCooldown = 0.5f;
        predefinedPlayerStats[3].gunPower = 600;
        predefinedPlayerStats[3].health = 4;
        predefinedPlayerStats[3].speed = 5;
    }
}
