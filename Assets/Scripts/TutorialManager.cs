using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("Task Setup")]
    [TextArea]
    public string[] tutorialTasks;   // Set task list in Inspector
    private int currentTaskIndex = 0;

    [Header("UI Reference")]
    public TextMeshProUGUI taskText;  // Assign TMP text in Inspector
    public GameObject tutorialUI;     // UI Image container

    [Header("References")]
    public DRONECONT droneController;
    public FSJoystickInput joystickInput;

    private bool taskInProgress = false;

    private void Start()
    {
        StartCoroutine(AssignDroneReferences());
    }

    private IEnumerator AssignDroneReferences()
    {
        // wait one frame to let DRONECONT.Start() set the tag
        yield return null;

        GameObject droneObj = GameObject.FindGameObjectWithTag("Player");
        if (droneObj != null)
        {
            droneController = droneObj.GetComponent<DRONECONT>();
            joystickInput = droneObj.GetComponent<FSJoystickInput>();
        }
        else
        {
            Debug.LogError("❌ No Drone with tag 'Drone' found in the scene!");
        }

        if (tutorialTasks.Length > 0 && taskText != null)
        {
            taskText.text = tutorialTasks[0];
        }
    }

    private void Update()
    {
        if (droneController == null) return; // ✅ Prevent null issues
        if (currentTaskIndex >= tutorialTasks.Length || taskInProgress) return;
      

        switch (currentTaskIndex)
        {
            case 0: // ✅ Arm the drone
                if (droneController.startupDone)
                    StartCoroutine(CompleteTask());
                break;

            case 1: // ✅ Ascend
                if (droneController.finalVertical > 0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 2: // ✅ Descend
                if (droneController.finalVertical < -0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 3: // ✅ Pitch
                if (Mathf.Abs(droneController.finalHorizontalZ) > 0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 4: // ✅ Roll
                if (Mathf.Abs(droneController.finalHorizontalX) > 0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 5: // ✅ Yaw
                if (Mathf.Abs(droneController.finalYaw) > 0.1f)
                    StartCoroutine(CompleteTask());
                break;

            case 6: // ✅ Land (grounded again)
                if (droneController.inGround)
                    StartCoroutine(CompleteTask());
                break;

            case 7: // ✅ Disarm (turn off)
                if (!droneController.startupDone)
                    StartCoroutine(CompleteTask());
                break;
        }
    }

    private IEnumerator CompleteTask()
    {
        taskInProgress = true;
        yield return new WaitForSeconds(2.0f);

        currentTaskIndex++;

        if (currentTaskIndex < tutorialTasks.Length)
        {
            taskText.text = tutorialTasks[currentTaskIndex];
        }
        else
        {
            Debug.Log("✅ Tutorial Completed!");
            if (tutorialUI != null)
                Destroy(tutorialUI);
        }

        taskInProgress = false;
    }
}
