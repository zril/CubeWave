using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Arena : MonoBehaviour {
    
    private List<TurnObj> currentTurns;
    private float rotationSpeed = 90f; //degree per second

    private int playerCount;
    private bool reload = false;
    private float reloadTimer = 0;

    private Quaternion nextRot;

	// Use this for initialization
	void Start () {
        currentTurns = new List<TurnObj>();
        //Turn(new Vector3(0, 1, 0));
        //Turn(new Vector3(1, 0, 0));
        //Turn(new Vector3(0, -1, 0));

        reload = false;
        nextRot = transform.rotation;

        playerCount = Global.playerCount;

        var canvas = FindObjectOfType<Canvas>();
        for(int i = 0; i < 4; i++)
        {
            if (i >= playerCount)
            {
                canvas.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            var script = player.GetComponent<Player>();
            if (script.playerNumber > playerCount)
            {
                player.gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        reloadTimer += Time.deltaTime;
        if (reloadTimer > 0 && reload)
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        //transform.Rotate(0.1f, 0.1f, 0.1f);
        var sumrotation = new Vector3();
        List<TurnObj> toRemove = new List<TurnObj>();
        /*foreach (TurnObj turn in currentTurns)
        {
            var angle = rotationSpeed * Time.deltaTime;
            sumrotation += turn.vector * angle;
            turn.remainingRotation -= angle;

            if (turn.remainingRotation <= 0)
            {
                sumrotation += turn.vector * turn.remainingRotation;
                toRemove.Add(turn);
            }
        }*/

        if (currentTurns.Count > 0)
        {
            TurnObj turn = currentTurns[0];
            var angle = rotationSpeed * Time.deltaTime;
            sumrotation += turn.vector * angle;
            turn.remainingRotation -= angle;

            if (turn.remainingRotation <= 0)
            {
                sumrotation += turn.vector * turn.remainingRotation;
                currentTurns.RemoveAt(0);
                SlideBlocks(turn);
                //toRemove.Add(turn);
            }
        }
       

        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + sumrotation);
        //transform.Rotate(new Vector3(sumrotation.x, 0, 0));
        //transform.Rotate(new Vector3(0, sumrotation.y, 0));
        //transform.Rotate(new Vector3(0, 0, sumrotation.z));
        transform.Rotate(sumrotation);


        /*foreach (TurnObj turn in toRemove)
        {
            currentTurns.Remove(turn);
            //SlideBlocks(turn);
        }*/
    }

    public void UpdateUI()
    {
        int playersAlive = 0;
        var players = GameObject.FindGameObjectsWithTag("Player");
        var canvas = FindObjectOfType<Canvas>();
        foreach (GameObject player in players)
        {
            var script = player.GetComponent<Player>();

            if (script.GetNbLifes() > 0)
            {
                playersAlive++;
            }

            for (int i = 1; i <= 3; i++)
            {
                if (i > script.GetNbLifes())
                {
                    canvas.gameObject.transform.GetChild(script.playerNumber-1).gameObject.transform.GetChild(i-1).gameObject.SetActive(false);
                }
            }   
        }

        if (playersAlive < 2)
        {
            reload = true;
            reloadTimer = -3f;
        }
    }

    public void Turn(Vector3 vector)
    {
        Debug.Log("turn" + vector);
        //Debug.Log("turn " + vector + " -> " + transform.rotation * vector);
        //currentTurns.Add(new TurnObj(Quaternion.Inverse(transform.rotation) * vector));
        //currentTurns.Add(new TurnObj(transform.rotation * vector));
        currentTurns.Add(new TurnObj(vector));
    }

    private void SlideBlocks(TurnObj turn)
    {
        var blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks)
        {
            var script = block.GetComponent<Block>();
            //Debug.Log("--------");
            //Debug.Log("slide " + turn.vector + " " + script.GetCurrentFace());
            if ((Vector3.Scale(turn.vector, script.GetCurrentFace())).magnitude == 0)
            {
                if (script.GetCurrentFace().x == 0 && turn.vector.x == 0)
                {
                    if (turn.vector.y != 0) // face z != 0
                    {
                        //Debug.Log("slide x : vector y = " + turn.vector.y + " face z = " + script.GetCurrentFace().z);
                        //block.transform.localPosition += new Vector3(-script.GetCurrentFace().z * turn.vector.y, 0, 0);
                        script.Slide(new Vector3(-script.GetCurrentFace().z * turn.vector.y, 0, 0));
                    } else //face y != 0 et vector z != 0
                    {
                        //Debug.Log("slide x : vector z = " + turn.vector.z + " face y = " + script.GetCurrentFace().y);
                        //block.transform.localPosition += new Vector3(script.GetCurrentFace().y * turn.vector.z, 0, 0);
                        script.Slide(new Vector3(script.GetCurrentFace().y * turn.vector.z, 0, 0));
                    }
                }
                if (script.GetCurrentFace().y == 0 && turn.vector.y == 0)
                {
                    if (turn.vector.x != 0) // face z != 0
                    {
                        //Debug.Log("slide y : vector x = " + turn.vector.x + " face z = " + script.GetCurrentFace().z);
                        //block.transform.localPosition += new Vector3(0, script.GetCurrentFace().z * turn.vector.x, 0);
                        script.Slide(new Vector3(0, script.GetCurrentFace().z * turn.vector.x, 0));
                    }
                    else //face x != 0 et vector z != 0
                    {
                        //Debug.Log("slide y : vector z = " + turn.vector.z + " face x = " + script.GetCurrentFace().x);
                        //block.transform.localPosition += new Vector3(0, -script.GetCurrentFace().x * turn.vector.z, 0);
                        script.Slide(new Vector3(0, -script.GetCurrentFace().x * turn.vector.z, 0));
                    }
                }
                if (script.GetCurrentFace().z == 0 && turn.vector.z == 0)
                {
                    if (turn.vector.y != 0) // face x != 0
                    {
                        //Debug.Log("slide z : vector y = " + turn.vector.y + " face x = " + script.GetCurrentFace().x);
                        //block.transform.localPosition += new Vector3(0, 0, script.GetCurrentFace().x * turn.vector.y);
                        script.Slide(new Vector3(0, 0, script.GetCurrentFace().x * turn.vector.y));
                    }
                    else //face y != 0 et vector x != 0
                    {
                        //Debug.Log("slide z : vector x = " + turn.vector.x + " face y = " + script.GetCurrentFace().y);
                        //block.transform.localPosition += new Vector3(0, 0, -script.GetCurrentFace().y * turn.vector.x);
                        script.Slide(new Vector3(0, 0, -script.GetCurrentFace().y * turn.vector.x));
                    }
                }
            }
        }
    }

    private class TurnObj
    {
        public float remainingRotation;
        public Vector3 vector;

        public TurnObj(Vector3 vector)
        {
            this.vector = vector;
            remainingRotation = 90;
        }
    }
}
