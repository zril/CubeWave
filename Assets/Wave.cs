using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

    public int playerNumber;
    public Vector3 face;
    public float power;

    private float waveSpeed = 1;
    private float currentSize = 0; //todo ?
    private float currentHitboxSize = 0;

    private float hitboxThickness = 0.3f;

    private int pixelPerUnit = 100;
    private int pixelRadius = 1024;
    private int wavePowerFactor = 3800;

    private float speedLoss = 0.8f;
    private float minSpeed = 0.3f;

    private List<GameObject> blockTargets;

    // Use this for initialization
    void Start () {
        blockTargets = new List<GameObject>();
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        currentSize = 0f;
        waveSpeed = power * wavePowerFactor / pixelRadius;
    }
	
	// Update is called once per frame
	void Update () {
        waveSpeed = waveSpeed - waveSpeed * Time.deltaTime * speedLoss;
        currentSize += Time.deltaTime * waveSpeed;
        transform.localScale = new Vector3(currentSize, currentSize, currentSize);

        currentHitboxSize = currentSize * pixelRadius / pixelPerUnit;
        //Debug.Log(currentHitboxSize);

        if (waveSpeed < minSpeed)
        {
            Destroy(gameObject);
        }

        CheckCollision();
    }


    private void CheckCollision()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            var playerScript = player.GetComponent<Player>();
            if (playerScript.GetCurrentFace().Equals(face) && playerNumber != playerScript.playerNumber)
            {
                var dist = Vector3.Distance(player.transform.localPosition, gameObject.transform.localPosition);
                if (Mathf.Abs(dist - currentHitboxSize) < hitboxThickness)
                {
                    //Debug.Log("collision " + playerNumber + " ->" + playerScript.playerNumber);
                    playerScript.Kill();
                }
            }
        }

        var blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks)
        {
            if (!blockTargets.Contains(block))
            {
                var blockScript = block.GetComponent<Block>();
                if (blockScript.GetCurrentFace().Equals(face))
                {
                    var dist = Vector3.Distance(block.transform.localPosition, transform.localPosition + face * 0.4f);
                    if (dist > 0 && Mathf.Abs(dist - currentHitboxSize) < hitboxThickness)
                    {
                        var dir = block.transform.localPosition - transform.localPosition;
                        blockTargets.Add(block);
                        blockScript.Push(dir.normalized, waveSpeed);
                    }
                }
            }
        }
    }
}
