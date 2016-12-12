using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int playerNumber = 1;
    private int nbLifes;

    private Vector3 currentFace = Vector3.up;

    private float rotSpeed = 6f;
    private float jumpPower = 0.3f;
    private float jumpPowerLoss = 2f;
    private float moveSpeed = 0.05f;
    private float backSpeed = 0.02f;
    private float jumpCooldown = 0.8f;
    private float doubleJumpTime = 0.3f;

    private KeyCode upKey = KeyCode.UpArrow;
    private KeyCode downKey = KeyCode.DownArrow;
    private KeyCode leftKey = KeyCode.LeftArrow;
    private KeyCode rightKey = KeyCode.RightArrow;
    private KeyCode jumpKey = KeyCode.RightControl;
    
    
    private float jumpSpeed = 0f;
    private float jumpTimer = 0f;
    private float deathTimer = 0f;
    private bool dead = false;
    private int doubleJumpCount = 0;

    // Use this for initialization
    void Start () {
        //currentFace = GetNewFace(transform.localPosition);
        nbLifes = 3;
        switch (playerNumber)
        {
            case 1:
                upKey = KeyCode.UpArrow;
                downKey = KeyCode.DownArrow;
                leftKey = KeyCode.LeftArrow;
                rightKey = KeyCode.RightArrow;
                jumpKey = KeyCode.RightControl;
                init(Vector3.up);
                break;
            case 2:
                upKey = KeyCode.E;
                downKey = KeyCode.D;
                leftKey = KeyCode.S;
                rightKey = KeyCode.F;
                jumpKey = KeyCode.LeftControl;
                init(Vector3.down);
                break;
            case 3:
                upKey = KeyCode.O;
                downKey = KeyCode.L;
                leftKey = KeyCode.K;
                rightKey = KeyCode.M;
                jumpKey = KeyCode.Space;
                init(Vector3.left);
                break;
            case 4:
                upKey = KeyCode.Keypad5;
                downKey = KeyCode.Keypad6;
                leftKey = KeyCode.Keypad3;
                rightKey = KeyCode.Keypad9;
                jumpKey = KeyCode.KeypadEnter;
                init(Vector3.right);
                break;
            default:
                Debug.Log("player undefined");
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
        deathTimer += Time.deltaTime;
        jumpTimer += Time.deltaTime;

        var arena = GameObject.FindGameObjectWithTag("Arena");

        var pos = transform.localPosition;
        var forward = Quaternion.Inverse(arena.transform.rotation) * -gameObject.transform.TransformDirection(Vector3.up);

        if (deathTimer > 0 && dead && nbLifes > 0)
        {
            dead = false;
            init(-currentFace);
            GetComponent<Renderer>().enabled = true;
        }

        
        if (jumpTimer > 0 && doubleJumpCount > 0)
        {
            //Debug.Log("fail time");
            doubleJumpCount = 0;
        }
        
        if (jumpSpeed > 0 && doubleJumpCount > 0 && Input.GetKeyDown(jumpKey))
        {
            //Debug.Log("fail input");
            doubleJumpCount = 0;
        }

        if (jumpSpeed == 0)
        {
            if (Input.GetKey(upKey))
            {
                var forwardSpeed = forward * moveSpeed;
                var nextPos = new Vector3(pos.x + forwardSpeed.x, pos.y + forwardSpeed.y, pos.z + forwardSpeed.z);
                nextPos = Utils.WallLimit(nextPos, currentFace);
                transform.localPosition = nextPos;
            }

            if (Input.GetKey(downKey))
            {
                var backwardSpeed = -forward * backSpeed;
                var nextPos = new Vector3(pos.x + backwardSpeed.x, pos.y + backwardSpeed.y, pos.z + backwardSpeed.z);
                nextPos = Utils.WallLimit(nextPos, currentFace);
                transform.localPosition = nextPos;
            }

            if (Input.GetKey(leftKey))
            {
                transform.Rotate(0, 0, -rotSpeed);
            }

            if (Input.GetKey(rightKey))
            {
                transform.Rotate(0, 0, rotSpeed);
            }

            if (Input.GetKeyDown(jumpKey))
            {
                //jump
                jumpSpeed = jumpPower;
                if (jumpTimer < 0)
                {
                    jumpSpeed += jumpPower * 0.20f * Mathf.Min(doubleJumpCount, 5);
                }
                doubleJumpCount += 1;
                // Debug.Log(doubleJumpCount);

                var audio = GameObject.FindObjectOfType<AudioSource>();
                audio.PlayOneShot(Resources.Load("sound/JUMPBEGIN-001Mono", typeof(AudioClip)) as AudioClip);
                audio.PlayOneShot(Resources.Load("sound/JUMPAIR-001Mono", typeof(AudioClip)) as AudioClip);
            }
        } else
        {
            jumpSpeed = jumpSpeed - jumpSpeed * Time.deltaTime * jumpPowerLoss;
            if (jumpSpeed < 0.05)
            {
                jumpSpeed = 0.05f;
            }
            var forwardSpeed = forward * jumpSpeed;
            var upSpeed = currentFace * jumpSpeed;

            var nextPos = new Vector3(pos.x + forwardSpeed.x + upSpeed.x, pos.y + forwardSpeed.y + upSpeed.y, pos.z + forwardSpeed.z + upSpeed.z);
            
            transform.localPosition = nextPos;

            if (Utils.WallCheck(nextPos))
            {
                var oldFace = currentFace;
                nextPos = Utils.WallLimit(nextPos, currentFace);
                currentFace = Utils.GetFace(nextPos);
                jumpTimer = -doubleJumpTime;
                transform.localRotation = Quaternion.LookRotation(currentFace, -oldFace);

                transform.localPosition = nextPos;
                //Debug.Log(jumpSpeed);
                Utils.CreateWave(transform.position, currentFace, playerNumber, jumpSpeed);
                Utils.FlyBlocks(transform.position, currentFace, 2f);

                jumpSpeed = 0f;

                var audio = GameObject.FindObjectOfType<AudioSource>();
                audio.PlayOneShot(Resources.Load("sound/JUMPEND-001Mono", typeof(AudioClip)) as AudioClip);
            }
        }
    }

    private void init(Vector3 face)
    {
        transform.localPosition = new Vector3(face.x * -5, face.y * -5, face.z * -5);
        currentFace = face;
        transform.localRotation = Quaternion.LookRotation(currentFace);
    }

    public Vector3 GetCurrentFace()
    {
        return currentFace;
    }

    public void Kill()
    {
        if (!dead)
        {
            GetComponent<Renderer>().enabled = false;
            deathTimer = -1f;
            dead = true;
            nbLifes--;
            Debug.Log("player " + playerNumber + " kill : " + nbLifes + " lifes");

            var arena = GameObject.FindGameObjectWithTag("Arena");
            arena.GetComponent<Arena>().UpdateUI();

            var audio = GameObject.FindObjectOfType<AudioSource>();
            audio.PlayOneShot(Resources.Load("sound/SHOCKMono", typeof(AudioClip)) as AudioClip);
        }
    }
    
    public int GetNbLifes()
    {
        return nbLifes;
    }
}
