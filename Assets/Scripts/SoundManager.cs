using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public List<AudioClip> soundFiles;

    private AudioSource audioPlayer;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayerLaunch()
    {
        AudioClip sound = soundFiles.Find(o => o.name == "Player_Launch");

        audioPlayer.PlayOneShot(sound);
    }

    public void PlayerDestroyed()
    {
        AudioClip sound = soundFiles.Find(o => o.name == "Player_Destroyed");

        audioPlayer.PlayOneShot(sound);
    }

    public void GamePauseResume()
    {
        AudioClip sound = soundFiles.Find(o => o.name == "Game_Pause");

        audioPlayer.PlayOneShot(sound);
    }

    public void EnemyDestroyed()
    {
        AudioClip sound = soundFiles.Find(o => o.name == "Enemy_Destroyed");

        audioPlayer.PlayOneShot(sound);
    }

    public void PowerUpAppeared()
    {
        AudioClip sound = soundFiles.Find(o => o.name == "PowerUp_Appeared");

        audioPlayer.PlayOneShot(sound);
    }

    public void PowerUpObtained()
    {
        AudioClip sound = soundFiles.Find(o => o.name == "PowerUp_Obtained");

        audioPlayer.PlayOneShot(sound);
    }

    public void BulletOnBigEnemy()
    {
        AudioClip sound = soundFiles.Find(o => o.name == "Bullet_FireAtBigEnemy");

        audioPlayer.PlayOneShot(sound);
    }

    public void BulletOnTile()
    {
        AudioClip sound = soundFiles.Find(o => o.name == "Bullet_FireAtBrick");

        audioPlayer.PlayOneShot(sound);
    }

    public void BulletOnWall()
    {
        AudioClip sound = soundFiles.Find(o => o.name == "Bullet_FireAtWall");

        audioPlayer.PlayOneShot(sound);
    }
}
