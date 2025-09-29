using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("Task Setup")]
    [TextArea]
    public string[] tutorialTasks;   // Set task list in Inspector
    private int currentTaskIndex = 0;
    [SerializeField]
    private GameObject[] gifs;

    [Header("UI Reference")]
    public TextMeshProUGUI taskText;  // Assign TMP text in Inspector
    public GameObject tutorialUI;     // UI Image container

    [Header("References")]
    public DRONECONT droneController;
    public FSJoystickInput joystickInput;

    private bool taskInProgress = false;
    private LandingPad landingPad;

    public GameObject[] checkpointColliders;

    private DroneYawTrail droneYawTrail;


    private void Start()
    {
        StartCoroutine(AssignDroneReferences());
        landingPad = GameObject.Find("Landing_Pad").GetComponent<LandingPad>();

        // Show first task’s GIF immediately
        ShowCurrentTaskGif();

        // Deactivate all checkpoint colliders initially
        for (int i = 0; i < checkpointColliders.Length; i++)
        {
            checkpointColliders[i].gameObject.SetActive(false);
        }

        droneYawTrail=GameObject.Find("Drone_01").GetComponent<DroneYawTrail>();
    }

    private IEnumerator AssignDroneReferences()
    {
        yield return null; // wait one frame

        GameObject droneObj = GameObject.FindGameObjectWithTag("Player");
        if (droneObj != null)
        {
            droneController = droneObj.GetComponent<DRONECONT>();
            joystickInput = droneObj.GetComponent<FSJoystickInput>();
        }
        else
        {
            Debug.LogError("No Drone with tag 'Player' found in the scene!");
        }

        if (tutorialTasks.Length > 0 && taskText != null)
        {
            taskText.text = tutorialTasks[0];
        }
    }

    private void Update()
    {
        if (droneController == null) return;
        if (currentTaskIndex >= tutorialTasks.Length || taskInProgress) return;

        switch (currentTaskIndex)
        {
            case 0: // Arm the drone
                if (droneController.startupDone)
                    StartCoroutine(CompleteTask());
                break;

            case 1: // Ascend
                if (droneController.finalVertical > 0.1f && !checkpointColliders[0].activeSelf)
                    StartCoroutine(CompleteTask());
                break;

            case 2: // Descend
                if (droneController.finalVertical < -0.1f  && !checkpointColliders[1].activeSelf)
                    StartCoroutine(CompleteTask());
                break;

            case 3: // Pitch forward (↑)
                if (droneController.finalHorizontalZ > 0.1f && !checkpointColliders[2].activeSelf)
                    StartCoroutine(CompleteTask());
                break;

            case 4: // Pitch backward (↓)
                if (droneController.finalHorizontalZ < -0.1f && !checkpointColliders[3].activeSelf)
                    StartCoroutine(CompleteTask());
                break;

            case 5: // Roll left (←)
                if (droneController.finalHorizontalX < -0.1f && !checkpointColliders[4].activeSelf)
                    StartCoroutine(CompleteTask());
                break;

            case 6: // Roll right (→)
                if (droneController.finalHorizontalX > 0.1f && !checkpointColliders[5].activeSelf)
                    StartCoroutine(CompleteTask());
                break;

            case 7: // Yaw clockwise (A)
                if (droneController.finalYaw < -0.1f && droneYawTrail.hasCompletedAnticlockwise==true)
                    StartCoroutine(CompleteTask());
                break;

            case 8: // Yaw anticlockwise (D)
                if (droneController.finalYaw > 0.1f && droneYawTrail.hasCompletedClockwise == true)
                    StartCoroutine(CompleteTask());
                break;

            case 9: // Land (grounded again)
                if (droneController.inGround && landingPad.isLanding)
                    StartCoroutine(CompleteTask());
                break;

            case 10: // Disarm (turn off)
                if (!droneController.startupDone)
                    StartCoroutine(CompleteTask());
                break;
        }
    }


    private void ActivateCheckpoint(int index)
    {
        // Only activate if not already active
        if (!checkpointColliders[index].activeSelf)
        {
            // Turn off all others first
            for (int i = 0; i < checkpointColliders.Length; i++)
                checkpointColliders[i].SetActive(false);

            checkpointColliders[index].SetActive(true);
        }
    }

    private IEnumerator CompleteTask()
    {
        taskInProgress = true;
        yield return new WaitForSeconds(0.2f);

        // Disable current GIF
        ShowGIFs(-1);

        // Move to next task
        currentTaskIndex++;

        if (currentTaskIndex < tutorialTasks.Length)
        {
            taskText.text = tutorialTasks[currentTaskIndex];
            ShowCurrentTaskGif();

            // --- Activate checkpoint only after task completion ---
            if (currentTaskIndex - 1 >= 0 && currentTaskIndex - 1 < checkpointColliders.Length)
            {
                ActivateCheckpoint(currentTaskIndex - 1);
            }
        }
        else
        {
            if (tutorialUI != null)
                Destroy(tutorialUI);
        }

        taskInProgress = false;
    }

    private void ShowCurrentTaskGif()
    {
        if (currentTaskIndex < 0)
        {
            ShowGIFs(-1);
            return;
        }

        if (currentTaskIndex == 9) // Landing → no GIF
        {
            ShowGIFs(-1);
            return;
        }

        if (currentTaskIndex == 10) // Disarm → gif_9
        {
            ShowGIFs(9);
            return;
        }

        if (currentTaskIndex < gifs.Length)
            ShowGIFs(currentTaskIndex);
        else
            ShowGIFs(-1);
    }

    private void ShowGIFs(int index)
    {
        for (int i = 0; i < gifs.Length; i++)
            gifs[i].SetActive(i == index);
    }
}
