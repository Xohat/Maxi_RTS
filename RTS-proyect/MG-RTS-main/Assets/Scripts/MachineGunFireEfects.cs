using UnityEngine;
using System.Collections;

public class MachineGunFireEfects : MonoBehaviour
{
    // references to the components which form the final effect
    private ParticleSystem gunParticles;
    private LineRenderer gunLine;
    private Light gunLight;

    private ParticleSystem smokeParticles;

    // duration of the effects
    public float effectsDisplayTime = 0.2f;

    // time between "shots" for when the auto mode is on
    public float autoPlayingWait = 0.4f;

    private bool playing = false;
    private bool playAuto = false;
    private float timer;

    private Vector3 lastEnemyPosition;
    private Transform lastEnemyTransform;
    private float timeToUpdateEnemyPosition = 0.3f, timeToUpdateEnemyPositionAux = 0f;

    void Awake ()
    {
        gunParticles = GetComponent<ParticleSystem>();
        gunLine      = GetComponent<LineRenderer  >();
        gunLight     = GetComponent<Light         >();

        Transform smoke = transform.Find("Smoke");
        if (smoke)
            smokeParticles = smoke.GetComponent<ParticleSystem>();
    }

    void Start ()
    {
        //gunParticles.enableEmission = false;
        gunParticles.Stop();
    }

    void Update ()
    {
        timer += Time.deltaTime;

        if (playing)
        {
            if (timer > effectsDisplayTime)
            {
                gunLine.enabled = false;
                gunLight.enabled = false;

                playing = false;

                if (playAuto)
                {
                    timer = 0f;
                }
            }
        }
        else if (playAuto)
        {
            timeToUpdateEnemyPositionAux += Time.deltaTime;
            //if (timeToUpdateEnemyPositionAux >= timeToUpdateEnemyPosition)
            //{
            //    lastEnemyPosition = new Vector3(lastEnemyTransform.position.x,
            //        transform.position.y, lastEnemyTransform.position.z);
            //
            //    timeToUpdateEnemyPositionAux = 0f;
            //}

            if (timer > autoPlayingWait)
            {
                PlayEffects();
            }
        }


    }

    public void SetAutomaticWeapon (bool automaticWeapon)
    {
        //playAuto = automaticWeapon;
    }

    public void PlayEffects (Vector3 shotPoint)
    {
        //lastEnemyPosition = new Vector3(enemyPosition.x, transform.position.y, enemyPosition.z);

        PlayEffects();
    }

    private void PlayEffects ()
    {
        timer = 0f;

        gunLight.enabled = true;

        //gunParticles.Stop();
        //gunParticles.Play();

        if (smokeParticles)
        {
            smokeParticles.Stop();
            smokeParticles.Play();
        }

        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);
        gunLine.SetPosition(1, new Vector3
        (
            lastEnemyPosition.x + Random.Range(-0.2f, 0.2f),
            lastEnemyPosition.y + Random.Range(-0.2f, 0.2f),
            lastEnemyPosition.z + Random.Range(-0.2f, 0.2f)
        ));

        playing = true;
    }

    public void PlayEffectsAutomatic (Vector3 shotPoint)
    {
        lastEnemyPosition = shotPoint;

        if (!playAuto)
        {
            timer = 0f;

            //lastEnemyTransform = shotPoint;
            var em = gunParticles.emission;
            em.enabled = true;
            gunParticles.Play();

            PlayEffects(shotPoint);

            playAuto = true;
        }
    }

    public void StopEffectsAutomatic ()
    {
        timer = 0f;

        gunLine.enabled = false;
        gunLight.enabled = false;

        playAuto = false;
    }

    public void StopEffects ()
    {
        timer = 0f;

        gunLine.enabled = false;
        gunLight.enabled = false;
        var em = gunParticles.emission;
        em.enabled = false;

        playAuto = false;
    }

    public bool IsPlaying ()
    {
        return (playAuto && playing);
    }

}
