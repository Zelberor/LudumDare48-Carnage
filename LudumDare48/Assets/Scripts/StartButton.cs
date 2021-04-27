using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StartButton : MonoBehaviour
{
    public GameObject nervigerRaenz;
    public Button thisbutton;
    public GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        thisbutton.onClick.AddListener(OnKlicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnKlicked()
    {
        Debug.Log("Button klicked\"");
     //   gm.MenuActive = false;
        Destroy(nervigerRaenz);
    }
}
