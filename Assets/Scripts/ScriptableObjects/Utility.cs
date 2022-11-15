using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Utility", menuName = "ScriptableObjects/Utility")]
public class Utility : ScriptableObject
{
    CursorLockMode originalCursorLockState;
    #region time scale sheninigans
    public void SetTimescale(float ts)
    {
        Time.timeScale = ts;
    }

    public void PauseTime()
    {
        SetTimescale(0.0f);
    }

    public void StartTime()
    {
        SetTimescale(1.0f);
    }

    public void ToggleTime()
    {
        SetTimescale(Mathf.Abs(Time.timeScale - 1.0f));
    }
    #endregion

    #region SceneManagement

    public void SwitchScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void SwitchScene(Scene scene)
    {
        SwitchScene(scene.buildIndex);
    }

    public void ReloadScene()
    {
        SwitchScene(SceneManager.GetActiveScene());
    }

    public void SwitchToMainMenu()
    {
        SwitchScene(0);
    }

    #endregion

    #region Cursor Locking

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        originalCursorLockState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ConfineCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResetCursor()
    {
        Cursor.lockState = originalCursorLockState;
    }

    public void ToggleCursor()
    {
        if(Cursor.lockState == CursorLockMode.None) { ResetCursor(); return; }
        UnlockCursor();
    }

    #endregion

    #region array functionality

    /// <summary>
    /// Picks a random item from the array and returns it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    /// <returns></returns>
    public static T Pick<T>(T[] arr)
    {
        int r = Random.Range(0, arr.Length);
        return arr[r];
    }

    /// <summary>
    /// Picks a random item from the list and returns it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    /// <returns></returns>
    public static T Pick<T>(List<T> arr)
    {
        int r = Random.Range(0, arr.Count);
        return arr[r];
    }

    /// <summary>
    /// Randomizes the order of the elements in the array. This is an IN PLACE operation!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    public static void FisherYates<T>(ref T[] arr)
    {
        for (int i = arr.Length - 1; i > -0; i--)
        {
            int j = Random.Range(0, i);
            Swap(ref arr, i, j);
        }
    }

    /// <summary>
    /// Randomizes the order of the elements in the list. This is an IN PLACE operation!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    public static void FisherYates<T>(ref List<T> arr)
    {
        for (int i = arr.Count - 1; i > -0; i--)
        {
            int j = Random.Range(0, i);
            Swap(ref arr, i, j);
        }
    }

    /// <summary>
    /// Swap element left and right in the array. This is an IN PLACE operation!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public static void Swap<T>(ref T[] arr, int left, int right)
    {
        T temp = arr[left];
        arr[left] = arr[right];
        arr[right] = temp;
    }

    /// <summary>
    /// Swap element left and right in the list. This is an IN PLACE operation!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arr"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public static void Swap<T>(ref List<T> arr, int left, int right)
    {
        T temp = arr[left];
        arr[left] = arr[right];
        arr[right] = temp;
    }
    #endregion

    public void QuitGame()
    {
        Application.Quit();
    }
}
