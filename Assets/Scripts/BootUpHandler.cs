using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BootUpHandler : MonoBehaviour
{
    public GameObject[] bootUpObjects, menuObjects;
    public GameObject[] tutorial;
    public bool skipBootup = false;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obi in bootUpObjects) { obi.SetActive(false); }
        foreach (GameObject obi in menuObjects) { obi.SetActive(false); }
        foreach (GameObject obi in tutorial) { obi.SetActive(false); }        

        StartCoroutine(MenuSequence());
    }

    IEnumerator MenuSequence()
    {
        yield return new WaitForSeconds(0.25f);
        menuObjects[0].SetActive(true);

        yield return new WaitForSeconds(0.1f);
        menuObjects[1].SetActive(true);

        yield return new WaitForSeconds(0.2f);
        menuObjects[2].SetActive(true);

        yield return new WaitForSeconds(0.3f);
        menuObjects[2].SetActive(false);

        yield return new WaitForSeconds(0.1f);
        menuObjects[2].SetActive(true);

        yield return new WaitForSeconds(0.25f);
        menuObjects[2].SetActive(false);

        yield return new WaitForSeconds(0.2f);
        menuObjects[2].SetActive(true);

        yield return new WaitForSeconds(0.5f);
        menuObjects[3].SetActive(true);

        yield return new WaitForSeconds(0.2f);
        menuObjects[4].SetActive(true);

        yield return new WaitForSeconds(0.5f);
        menuObjects[5].SetActive(true);

        yield return new WaitForSeconds(0.2f);
        menuObjects[6].SetActive(true);

        yield return new WaitForSeconds(0.5f);
        menuObjects[7].SetActive(true);

        yield return new WaitForSeconds(0.2f);
        menuObjects[8].SetActive(true);

        yield return new WaitForSeconds(0.5f);
        menuObjects[9].SetActive(true);

        yield return new WaitForSeconds(0.2f);
        menuObjects[10].SetActive(true);

        yield return new WaitForSeconds(0.1f);
        menuObjects[6].SetActive(false);

        yield return new WaitForSeconds(0.15f);
        menuObjects[6].SetActive(true);

        yield return new WaitForSeconds(0.1f);
        menuObjects[6].SetActive(false);

        yield return new WaitForSeconds(0.15f);
        menuObjects[11].SetActive(true);

        yield return new WaitForSeconds(0.1f);
        menuObjects[6].SetActive(true);

        yield return new WaitForSeconds(0.1f);
        menuObjects[12].SetActive(true);
    }

    public void BootUp()
    {
        StartCoroutine(BootUpSequence());
    }

    public void Tutorial()
    {
        StartCoroutine(TutorialSequence());
    }

    IEnumerator BootUpSequence()
    {
        if (!skipBootup)
        {
            for (int i = 0; i < bootUpObjects.Length; i++)
            {
                yield return StartCoroutine(BootItem(bootUpObjects[i]));
            }
        }
        else
        {
            foreach (GameObject obi in bootUpObjects) { obi.SetActive(true); }

        }
        GetComponent<PlayerMovement>().BootUp();
        GetComponent<GrenadeLauncher>().BootUp();
        GetComponent<GrenadeLauncher>().FillGrenades();
        GetComponent<DamageHandler>().BootUp();
    }

    IEnumerator BootItem(GameObject obi)
    {
        yield return new WaitForSeconds(Random.Range(0.05f, 0.45f));
        obi.SetActive(true);
        if(Random.Range(0,100) < 20) { StartCoroutine(FlickerItem(obi)); }
    }

    IEnumerator FlickerItem(GameObject obi)
    {
        for (int i = 0; i < Random.Range(1,6); i++)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            obi.SetActive(false);

            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            obi.SetActive(true);
        }
    }

    IEnumerator TutorialSequence()
    {
        yield break;
    }
}
