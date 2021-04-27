using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataUI : MonoBehaviour
{
    public Text timer;
    public Text healthMonitor;

	public Text gameover;
	public Text thanks;
	public Text currentRoom;
    public PlayerMovement player;
    public GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gameover.text = "";
		thanks.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        healthMonitor.text ="HP: " + player.health;
        timer.text = "Go deeper in " + gm.getTimeTrimmed() + "s";
		currentRoom.text = "Depth: " + (Room.getCurrentID() - 1);
    }

	public void showGameover() {
		gameover.text = "GAME OVER";
		thanks.text = "Thank you for playing Wartronic's and Zelberor's LD48 game!\nYou have to close and start the game again to play another round.\nPress alt+f4 to exit.";
	}

}
