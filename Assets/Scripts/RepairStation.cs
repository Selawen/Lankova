using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairStation : MonoBehaviour
{
    public GameObject repairPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }

        repairPanel.SetActive(true);
        PlayerMovement.Instance.repairPanelActive = true;
        PlayerMovement.Instance.GetComponent<MechTurning>().enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) { return; }

        repairPanel.SetActive(false);
        PlayerMovement.Instance.repairPanelActive = false;
        PlayerMovement.Instance.GetComponent<MechTurning>().enabled = true;
    }


}
