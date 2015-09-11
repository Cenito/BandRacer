using BandRacer.Platform;
using BandRacer.Plugin;
using UnityEngine;
using System.Collections;

public class CarMovement : MonoBehaviour
{
    public float acceleration = 0.3f;
    public float braking = 0.3f;
    public float steering = 4.0f;
    public float maxVelocity = 15f;

    private PlayerInputs m_Inputs;
    private Rigidbody2D m_Rigidbody2D;

    // Use this for initialization
    void Start()
    {
        m_Inputs = GetComponent<PlayerInputs>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private float GetXAngle(int player)
    {
        if (player == 1)
        {
            return PlatformBase.Current.XAnglePlayer1;
        }
        return PlatformBase.Current.XAnglePlayer2;
    }

    private float GetYAngle(int player)
    {
        if (player == 1)
        {
            return PlatformBase.Current.YAnglePlayer1;
        }
        return PlatformBase.Current.YAnglePlayer2;
    }

    void Update()
    {
        EngineSound();
    }

    // update for physics
    void FixedUpdate()
    {
        if (!RaceManager.Instance.RaceStarted)
            return;

        if (Platform.Current == null)
        {
            PlatformBase.Current = new Platform();
        }

#if UNITY_EDITOR
        float rot = transform.localEulerAngles.z - m_Inputs.x * steering;
#else
        float yAngle = GetYAngle(m_Inputs.playerNumber);
        float rot = transform.localEulerAngles.z - yAngle * steering;
#endif
        transform.localEulerAngles = new Vector3(0.0f, 0.0f, rot);

        // acceleration/braking
        float velocity = m_Rigidbody2D.velocity.magnitude;

#if UNITY_EDITOR
        float y = m_Inputs.y;
#else
        float y = GetXAngle(m_Inputs.playerNumber);
#endif

        if (y > 0.01f)
        {
            velocity += y * acceleration;
        }
        else if (y < -0.01f)
        {
            velocity += y * braking;
        }

        // apply car movement
        m_Rigidbody2D.AddForce(transform.right * y * acceleration * 50.0f);

        Vector3 aimedVelocity = transform.right*velocity;
        
        m_Rigidbody2D.velocity = aimedVelocity;
        m_Rigidbody2D.angularVelocity = 0.0f;
    }

    private void EngineSound()
    {
        if (!RaceManager.Instance.RaceStarted)
        {
            var audioSource = GetComponent<AudioSource>();
            audioSource.volume = 0f;
        }
        else
        {
            var audioSource = GetComponent<AudioSource>();
            audioSource.volume = 1f;
            audioSource.pitch = m_Rigidbody2D.velocity.magnitude/maxVelocity + 1;
        }
    }
}