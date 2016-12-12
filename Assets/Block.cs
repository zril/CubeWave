using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    private float flySpeed = 0.2f;

    private Vector3 currentFace;

    private bool flying = false;
    private float flyCooldown = 0.1f;
    private float flyTimer = 0;

    private Vector3 velocity;
    private float friction = 1f;
    private float speedloss = 0.6f;

    private float slidePower = 0.4f;
    private float pushPower = 2f;

    // Use this for initialization
    void Start () {
        currentFace = Utils.GetFace(transform.localPosition);
	}
	
	// Update is called once per frame
	void Update () {
        flyTimer += Time.deltaTime;

        var pos = transform.localPosition;
		if (flying)
        {
            var nextPos = new Vector3(pos.x + currentFace.x * flySpeed, pos.y + currentFace.y * flySpeed, pos.z + currentFace.z * flySpeed);
            transform.localPosition = nextPos;

            if (Utils.WallCheck(nextPos))
            {
                var oldFace = currentFace;
                nextPos = Utils.WallLimit(nextPos, currentFace);
                currentFace = Utils.GetFace(nextPos);

                flying = false;
                flyTimer = 0;

                transform.localPosition = nextPos;

                Utils.CreateWave(transform.position, currentFace, 0, flySpeed);
                Utils.FlyBlocks(transform.position, currentFace, 2f);
                checkTurn();

                var audio = GameObject.FindObjectOfType<AudioSource>();
                audio.PlayOneShot(Resources.Load("sound/JUMPEND-003Mono", typeof(AudioClip)) as AudioClip);
            }
        }

        if (velocity.magnitude > 0)
        {
            transform.localPosition += velocity * Time.deltaTime;
            transform.localPosition = Utils.WallLimit(transform.localPosition, currentFace);
            velocity += -velocity * friction * Time.deltaTime;
            velocity += -speedloss * Time.deltaTime * velocity.normalized;
        }
	}

    public void Fly()
    {
        if (flyTimer > flyCooldown)
        {
            flying = true;
        }
        
    }

    public void Push(Vector3 dir, float power)
    {
        if (currentFace.x != 0)
        {
            dir.x = 0;
        }
        if (currentFace.y != 0)
        {
            dir.y = 0;
        }
        if (currentFace.z != 0)
        {
            dir.z = 0;
        }
        /*var pos = transform.localPosition;
        var dist = power * 0.3f;
        pos = new Vector3(pos.x + dir.x * dist, pos.y + dir.y * dist, pos.z + dir.z * dist);
        transform.localPosition = Utils.WallLimit(pos, currentFace);*/

        velocity += dir * pushPower * power;
    }

    public Vector3 GetCurrentFace()
    {
        return currentFace;
    }

    private void checkTurn()
    {
        var arena = GameObject.FindGameObjectWithTag("Arena");
        Debug.Log("checkturn " + currentFace + " " + transform.localPosition);
        if (currentFace.x != 0)
        {
            Debug.Log("blockturn x : " + transform.localPosition.y + " " + transform.localPosition.z);
            if (Mathf.Abs(transform.localPosition.y) > 4)
            {
                arena.GetComponent<Arena>().Turn(new Vector3(0, 0, Mathf.Sign(currentFace.x) * Mathf.Sign(transform.localPosition.y)));
            }
            if (Mathf.Abs(transform.localPosition.z) > 4)
            {
                arena.GetComponent<Arena>().Turn(new Vector3(0, -Mathf.Sign(currentFace.x) * Mathf.Sign(transform.localPosition.z), 0));
            }
        }
        if (currentFace.y != 0)
        {
            Debug.Log("blockturn y : " + transform.localPosition.x + " " + transform.localPosition.z);
            if (Mathf.Abs(transform.localPosition.x) > 4)
            {
                arena.GetComponent<Arena>().Turn(new Vector3(0, 0, -Mathf.Sign(currentFace.y) * Mathf.Sign(transform.localPosition.x)));
            }
            if (Mathf.Abs(transform.localPosition.z) > 4)
            {
                arena.GetComponent<Arena>().Turn(new Vector3(Mathf.Sign(currentFace.y) * Mathf.Sign(transform.localPosition.z), 0, 0));
            }
        }
        if (currentFace.z != 0)
        {
            Debug.Log("blockturn z : " + transform.localPosition.x + " " + transform.localPosition.y);
            if (Mathf.Abs(transform.localPosition.x) > 4)
            {
                arena.GetComponent<Arena>().Turn(new Vector3(0, Mathf.Sign(currentFace.z) * Mathf.Sign(transform.localPosition.x), 0));
            }
            if (Mathf.Abs(transform.localPosition.y) > 4)
            {
                arena.GetComponent<Arena>().Turn(new Vector3(-Mathf.Sign(currentFace.z) * Mathf.Sign(transform.localPosition.y), 0, 0));
                
            }
        }
    }

    public void Slide(Vector3 dir)
    {
        var powerFactor = 6f;
        if (dir.x != 0)
        {
            powerFactor += -Mathf.Sign(dir.x) * transform.localPosition.x;
        }
        if (dir.y != 0)
        {
            powerFactor += -Mathf.Sign(dir.y) * transform.localPosition.y;
        }
        if (dir.z != 0)
        {
            powerFactor += -Mathf.Sign(dir.z) * transform.localPosition.z;
        }
        velocity += dir * slidePower * powerFactor;
    }
}
