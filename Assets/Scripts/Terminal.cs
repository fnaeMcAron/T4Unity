using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Termanal : MonoBehaviour
{
    public Camera cam;
    public Transform camOffset;
    public Canvas UI3d;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnInteract()
    {
        Debug.Log("test");
        cam.GetComponent<CameraFollow>().enabled = false;
        cam.transform.position = camOffset.position;
        cam.transform.rotation = camOffset.rotation;
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
