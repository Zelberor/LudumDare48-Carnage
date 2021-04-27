using UnityEngine;

public class GameManager : MonoBehaviour
{

    public PlayerMovement player;
    public WorldGenerator worldgen;
    private float ttnr;
    private bool isStarted = false;

	public DataUI dataUIsetter;
	public static DataUI dataUI;
    //public bool MenuActive = true;
    // Start is called before the first frame update
    void Start()
    {
		dataUI = dataUIsetter;
        Screen.fullScreen = true;
        Cursor.lockState = CursorLockMode.Locked;



    }

    // Update is called once per frame
    void Update()
    {

        if (!isStarted)
        {
            isStarted = Input.anyKey;
            ttnr = 1f;
        
        }
        else {
            ttnr -= Time.deltaTime;
            ttnr = (ttnr < 0) ? 0 : ttnr;
            if (ttnr == 0) Next();

            var playery = player.transform.position.y;
             var height =  worldgen.getCurrentRoom().getDimensions().y;
            var floorheight = worldgen.getCurrentRoom().getFloorHeight();

            player.setMovementEnabelt(playery <= floorheight + height && playery >= floorheight);
        }

    }

    private void Next()
    {
        worldgen.goToNextRoom();
        ttnr = 10f + generateOffset();
		if (ttnr > 40f)
			ttnr = 50f;
    }

    private float generateOffset(float Multiplier =  .75f)
    {
        var Size = worldgen.getCurrentRoom().getDimensions();
        var square = (Size.x * Size.z);
        return Random.Range(0, Mathf.Sqrt(square))*Multiplier;
    }
    public int getTimeTrimmed()
    {

        return Mathf.RoundToInt(ttnr);
    
    
    }

    public static T roll<T>(T[] input, float[] weights)
    {
        var multiplier = 1f;
        var sum = 0f;
        foreach (var item in weights)
        {
            sum += item;
        }
        multiplier = 1 / sum;
        for(int index = 0;index < weights.Length;index++)
        {
            weights[index] *= multiplier;
        }
        float random = Random.value;
        float _sum=0;
        int currIndex =-1;
        do
        {
            currIndex++;
            _sum += weights[currIndex];

        } while (_sum < random);
        return input[currIndex];
    }
    public static void PlayerDed()
    {
		dataUI.showGameover();
        Time.timeScale = 0;
    }
}
