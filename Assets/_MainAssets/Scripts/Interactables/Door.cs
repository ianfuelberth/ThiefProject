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


    private void Awake()
    {
        initialRotation = transform.rotation.eulerAngles;
        forward = transform.forward;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
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
                    return;
                }
                else
                {
                    statusDisplay.DisplayMessage(StatusMessage.LockedSide);
                    return;
                }
            }

            Tool activeTool = Camera.main.GetComponent<ToolbeltController>().GetActiveTool();
            if (activeTool.GetType() != typeof(Key))
            {
                statusDisplay.DisplayMessage(StatusMessage.WrongKey);
                return;
            }
            else
            {
                Key activeKey = (Key)activeTool;
                if (!activeKey.IsCorrectKey(requiredKey))
                {
                    statusDisplay.DisplayMessage(StatusMessage.WrongKey);
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
