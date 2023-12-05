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
        if (isLockedDoor)
        {
            Tool activeTool = Camera.main.GetComponent<ToolbeltController>().GetActiveTool();
            // check for key
            if (activeTool.GetType() == typeof(Key))
            {
                Key activeKey = (Key)activeTool;
                if (activeKey.IsCorrectKey(requiredKey))
                {
                    if (isRotatingDoor)
                    {
                        if (isOpen)
                        {
                            Close();
                        }
                        else
                        {
                            Open(player.transform.position);
                            isLockedDoor = false;
                        }

                        return;
                    }
                    else
                    {
                        Debug.Log("Door: isRotatingDoor==false is not implemented.");
                    }

                    
                }
            }

            Debug.Log("You are not currently holding the correct key.");
            GameObject.FindGameObjectWithTag("Canvas").GetComponentInChildren<StatusDisplay>().DisplayMessage(StatusMessage.WrongKey);
        }
        else
        {
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
            else
            {
                Debug.Log("Door: isRotatingDoor==false is not implemented.");
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
