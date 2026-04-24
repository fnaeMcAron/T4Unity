using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Terminal : MonoBehaviour
{
    public Camera cam;
    public Transform camOffset;
    public Canvas UI3d;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /*public void OnInteract()
    {
        Debug.Log("test");
        cam.GetComponent<CameraFollow>().enabled = false;
        cam.transform.position = camOffset.position;
        cam.transform.rotation = camOffset.rotation;
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
    }*/
    public void OnInteract()
    {
        DungeonMaster.Instance.PushState(new sub_TerminalState(this));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
