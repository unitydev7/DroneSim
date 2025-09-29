using UnityEngine;
using TMPro;

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

    private LandingPad landingPad;
    public GameObject[] checkpointColliders;

    private void Start()
    {
        Invoke(nameof(AssignDroneReferences), 0.1f);
        landingPad = GameObject.Find("Landing_Pad").GetComponent<LandingPad>();

        // Show first task’s GIF immediately
        ShowCurrentTaskGif();

        // Deactivate all checkpoint colliders initially
        for (int i = 0; i < checkpointColliders.Length; i++)
            checkpointColliders[i].gameObject.SetActive(false);
    }

    private void AssignDroneReferences()
    {
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
            taskText.text = tutorialTasks[0];
    }

    private void Update()
    {
        if (droneController == null) return;
        if (currentTaskIndex >= tutorialTasks.Length) return;

        switch (currentTaskIndex)
        {
            case 0: // Arm the drone
                if (droneController.startupDone)
                    CompleteTask();
                break;

            case 1: // Ascend
                if (!checkpointColliders[0].activeSelf && droneController.finalVertical > 0.1f)
                    CompleteTask();
                break;

            case 2: // Descend
                if (!checkpointColliders[1].activeSelf && droneController.finalVertical < -0.1f)
                    CompleteTask();
                break;

            case 3: // Pitch forward
                if (!checkpointColliders[2].activeSelf && droneController.finalHorizontalZ > 0.1f)
                    CompleteTask();
                break;

            case 4: // Pitch backward
                if (!checkpointColliders[3].activeSelf && droneController.finalHorizontalZ < -0.1f)
                    CompleteTask();
                break;

            case 5: // Roll left
                if (!checkpointColliders[4].activeSelf && droneController.finalHorizontalX < -0.1f)
                    CompleteTask();
                break;

            case 6: // Roll right
                if (!checkpointColliders[5].activeSelf && droneController.finalHorizontalX > 0.1f)
                    CompleteTask();
                break;

            case 7: // Yaw clockwise (A)
                if (droneController.finalYaw < -0.1f )
                    CompleteTask();
                break;

            case 8: // Yaw anticlockwise (D)
                if (droneController.finalYaw > 0.1f)
                    CompleteTask();
                break;

            case 9: // Land
                if (droneController.inGround && landingPad.isLanding)
                    CompleteTask();
                break;

            case 10: // Disarm
                if (!droneController.startupDone)
                    CompleteTask();
                break;
        }
    }

    private void ActivateCheckpoint(int index)
    {
        // Only activate if it’s currently inactive
        if (!checkpointColliders[index].activeSelf)
        {
            // Ensure previous checkpoint is disabled first
            if (index == 0 || !checkpointColliders[index - 1].activeSelf)
            {
                checkpointColliders[index].SetActive(true);
            }
        }
    }

    private void CompleteTask()
    {
        // Disable current GIF
        ShowGIFs(-1);

        // Advance task
        currentTaskIndex++;

        if (currentTaskIndex < tutorialTasks.Length)
        {
            taskText.text = tutorialTasks[currentTaskIndex];
            ShowCurrentTaskGif();

            // Activate checkpoint for new task if available
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
