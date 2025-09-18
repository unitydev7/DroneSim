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

    private void Start()
    {
        StartCoroutine(AssignDroneReferences());
        landingPad = GameObject.Find("Landing_Pad").GetComponent<LandingPad>();

        // Show first task’s GIF immediately
        ShowCurrentTaskGif();
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
            Debug.LogError(" No Drone with tag 'Player' found in the scene!");
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
                if (droneController.finalVertical > 0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 2: // Descend
                if (droneController.finalVertical < -0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 3: // Pitch forward (↑)
                if (droneController.finalHorizontalZ > 0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 4: // Pitch backward (↓)
                if (droneController.finalHorizontalZ < -0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 5: // Roll left (←)
                if (droneController.finalHorizontalX < -0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 6: // Roll right (→)
                if (droneController.finalHorizontalX > 0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 7: // Yaw clockwise (A)
                if (droneController.finalYaw < -0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 8: // Yaw anticlockwise (D)
                if (droneController.finalYaw > 0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 9: // Land (grounded again)
                if (droneController.inGround && landingPad.isLanding == true)
                    StartCoroutine(CompleteTask());
                break;

            case 10: // Disarm (turn off)
                if (!droneController.startupDone)
                    StartCoroutine(CompleteTask());
                break;
        }
    }

    private IEnumerator CompleteTask()
    {
        taskInProgress = true;
        yield return new WaitForSeconds(1.5f);

        // Disable current GIF
        ShowGIFs(-1);

        // Move to next task
        currentTaskIndex++;

        if (currentTaskIndex < tutorialTasks.Length)
        {
            taskText.text = tutorialTasks[currentTaskIndex];
            ShowCurrentTaskGif(); // Show next task’s GIF immediately
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

        // Special case: Landing (task 9) → no GIF
        if (currentTaskIndex == 9)
        {
            ShowGIFs(-1);
            return;
        }

        // Special case: Disarm (task 10) → show gif_9
        if (currentTaskIndex == 10)
        {
            ShowGIFs(9);
            return;
        }

        // Normal mapping (task index = gif index)
        if (currentTaskIndex < gifs.Length)
        {
            ShowGIFs(currentTaskIndex);
        }
        else
        {
            ShowGIFs(-1);
        }

    }

    private void ShowGIFs(int index)
    {
        for (int i = 0; i < gifs.Length; i++)
        {
            gifs[i].SetActive(i == index);
        }
    }
}
