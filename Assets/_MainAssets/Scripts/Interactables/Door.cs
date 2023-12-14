using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=cPltQK5LlGE 
public class Door : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public bool isOpen = false;
    [SerializeField]
    private bool isLockedDoor = false;
    [SerializeField]
    private bool isRotatingDoor = true;
    [SerializeField]
    private float speed = 1f;

    [Header("Rotation Settings")]
    [SerializeField]
    private float rotationAmount = 90f;
    [SerializeField]
    private float forwardDirection = 0;

    [Header("Lock Settings")]
    [SerializeField]
    private bool unlocksFromBehind = false;
    [SerializeField]
    private KeyID requiredKey;

    [Header("References")]
    [SerializeField]
    private GameObject player;

    private Vector3 initialRotation;
    private Vector3 forward;

    private Coroutine animationCoroutine;

    ////// Sound Effect Clips //////
    AudioSource m_AudioSource;
    [Header("Sound Effects")]
    //sound settings
    [SerializeField] float m_Volume = 0.5f;
    //standard
    [SerializeField] private AudioClip m_Unlocked;
    [SerializeField] private AudioClip m_Locked;
    [SerializeField] private AudioClip m_Open;
    [SerializeField] private AudioClip m_Close;
    //keycard
    [SerializeField] private AudioClip m_KeycardAccept;
    [SerializeField] private AudioClip m_KeycardReject;



    private void Awake()
    {
        initialRotation = transform.rotation.eulerAngles;
        forward = transform.forward;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        //Set the audio source to THIS object's AudioSource component
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.volume = m_Volume;


    }

    public void Interact()
    {
        StatusDisplay statusDisplay = GameObject.FindGameObjectWithTag("Canvas").GetComponentInChildren<StatusDisplay>();

        if (isLockedDoor)
        {
            if (unlocksFromBehind)
            {
                if (IsInteractingFromBehind(player.transform.position))
                {
                    UseDoor();
                    //play Sound
                    m_AudioSource.clip = m_KeycardAccept;
                    m_AudioSource.Play();
                    return;
                }
                else
                {
                    statusDisplay.DisplayMessage(StatusMessage.LockedSide);
                    //play Sound
                    m_AudioSource.clip = m_Locked;
                    m_AudioSource.Play();
                    return;
                }
            }

            Tool activeTool = Camera.main.GetComponent<ToolbeltController>().GetActiveTool();
            if (activeTool.GetType() != typeof(Key))
            {
                statusDisplay.DisplayMessage(StatusMessage.WrongKey);
                //play Sound
                m_AudioSource.clip = m_KeycardReject;
                m_AudioSource.Play();
                return;
            }
            else
            {
                Key activeKey = (Key)activeTool;
                if (!activeKey.IsCorrectKey(requiredKey))
                {
                    statusDisplay.DisplayMessage(StatusMessage.WrongKey);
                    //play Sound
                    m_AudioSource.clip = m_Locked;
                    m_AudioSource.Play();
                    return;
                }
            }
        }

        UseDoor();

    }

    private void UseDoor()
    {
        isLockedDoor = false;

        if (isRotatingDoor)
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open(player.transform.position);
            }
        }
    }

    public void Open(Vector3 userPosition)
    {
        if (!isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if (isRotatingDoor)
            {
                float dot = Vector3.Dot(forward, (userPosition - transform.position).normalized);
                animationCoroutine = StartCoroutine(DoRotationOpen(dot));
            }
            //play Sound
            m_AudioSource.clip = m_Open;
            m_AudioSource.Play();
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if (isRotatingDoor)
            {
                animationCoroutine = StartCoroutine(DoRotationClose());
            }
            //play Sound
            m_AudioSource.clip = m_Close;
            m_AudioSource.Play();
        }
    }

    private bool IsInteractingFromBehind(Vector3 userPosition)
    {
        float dot = Vector3.Dot(forward, (userPosition - transform.position).normalized);
        if (dot >= forwardDirection)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private IEnumerator DoRotationOpen(float forwardAmount)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (forwardAmount >= forwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, initialRotation.y - rotationAmount, 0));
            Debug.Log("forward");
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, initialRotation.y + rotationAmount, 0));
            Debug.Log("backward");
        }

        isOpen = true;

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    private IEnumerator DoRotationClose()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(initialRotation);

        isOpen = false;

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }


}
