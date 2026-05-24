using UnityEngine;

public class MenusUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCircuit()
    {
        //Debug.Log("StartCircuit");
        GameManager.Instance.StartCircuit();
    }

    public void EndCircuit()
    {
        Debug.Log("EndCircuit");
        GameManager.Instance.EndCircuit();
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame");
        GameManager.Instance.QuitGame();
    }
}
