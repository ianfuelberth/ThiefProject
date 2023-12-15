using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interactable Door. Opens upon Interaction.
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

    [Header("Sound Settings")]
    //sound settings
    [SerializeField] float m_Volume = 0.5f;

    [Header("References")]
    [SerializeField]
    private GameObject player;

    private Vector3 initialRotation;
    private Vector3 forward;

    private Coroutine animationCoroutine;


    private void Awake()
    {
        initialRotation = transform.rotation.eulerAngles;
        forward = transform.forward;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        Resources.Load<AudioClip>("");

    }

    public void Interact()
    {
        StatusDisplay statusDisplay = GameObject.FindGameObjectWithTag("Canvas").GetComponentInChildren<StatusDisplay>();

        // Check if door is locked
        if (isLockedDoor)
        {
            // Handle interaction if door can be unlocked from behind
            if (unlocksFromBehind)
            {
                if (IsInteractingFromBehind(player.transform.position))
                {
                    SoundManager.PlaySound(gameObject, SoundEffect.Door_Unlock, m_Volume);
                    UseDoor();
                    return;
                }
                else
                {
                    SoundManager.PlaySound(gameObject, SoundEffect.Door_Locked, m_Volume);
                    statusDisplay.DisplayMessage(StatusMessage.LockedSide);
                    return;
                }
            }

            // Check if player is holding the correct key.
            Tool activeTool = Camera.main.GetComponent<ToolbeltController>().GetActiveTool();
            if (activeTool.GetType() != typeof(Key))
            {
                SoundManager.PlaySound(gameObject, SoundEffect.Door_Locked, m_Volume);
                statusDisplay.DisplayMessage(StatusMessage.WrongKey);
                return;
            }
            else
            {
                Key activeKey = (Key)activeTool;
                if (!activeKey.IsCorrectKey(requiredKey))
                {
                    SoundManager.PlaySound(gameObject, SoundEffect.Keycard_Reject, m_Volume);
                    statusDisplay.DisplayMessage(StatusMessage.WrongKey);
                    return;
                }
                else
                {
                    SoundManager.PlaySound(gameObject, SoundEffect.Keycard_Accept, m_Volume);
                }
            }
        }

        UseDoor();

    }

    // Used by AI to automatically open nearby doors.
    public void DoorOpenOverride(Navigation nav)
    {
        if (nav != null && !isOpen)
        {
            UseDoor();
        }
    }

    // Opens/Closes the door based on current state.
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

    // Opens the door away from the user's position
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
        }
    }

    // Closes the door
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
        }
    }

    // returns true if the player is interacting from the "back" side of the door.
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

    // Handles the animation of the door opening.
    private IEnumerator DoRotationOpen(float forwardAmount)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (forwardAmount >= forwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, initialRotation.y - rotationAmount, 0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, initialRotation.y + rotationAmount, 0));
        }

        isOpen = true;
        SoundManager.PlaySound(gameObject, SoundEffect.Door_Open, m_Volume);

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    // Handles the animation of the door closing.
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
        SoundManager.PlaySound(gameObject, SoundEffect.Door_Close, m_Volume);
    }

    /*
    private void OnCollisionEnter(Collision collision) {
        Debug.Log(collision);
        if(collision.gameObject.tag == "AI")
        {
            Debug.Log("Collision with the AI!");
            Open(collision.gameObject.transform.position);
        }
    }
    */
}
