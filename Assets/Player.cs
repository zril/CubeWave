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
    private float moveSpeed = 0.07f;
    private float backSpeed = 0.02f;
    private float jumpCooldown = 1.0f;
    private float doubleJumpTime = 0.25f;
    private float doubleJumpBonus = 0.15f;
    private int maxJumpCount = 3;

    private KeyCode upKey = KeyCode.UpArrow;
    private KeyCode downKey = KeyCode.DownArrow;
    private KeyCode leftKey = KeyCode.LeftArrow;
    private KeyCode rightKey = KeyCode.RightArrow;
    private KeyCode jumpKey = KeyCode.RightControl;
    private bool jumpKeyFlag = true;
    
    
    private float jumpSpeed = 0f;
    private float jumpTimer = 0f;
    private float jumpInputTimer = 0f;
    private float deathTimer = 0f;
    private bool dead = false;
    private int doubleJumpCount = 0;

    // Use this for initialization
    void Start () {
        //currentFace = GetNewFace(transform.localPosition);
        nbLifes = 3;
        deathTimer = 2f;
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

    private bool GetUpKey()
    {
        return Input.GetKey(upKey) || Input.GetAxis("Vertical "+ playerNumber) > 0.9f;
    }

    private bool GetDownKey()
    {
        return Input.GetKey(downKey) || Input.GetAxis("Vertical " + playerNumber) < -0.9f;
    }

    private bool GetRightKey()
    {
        return Input.GetKey(rightKey) || Input.GetAxis("Horizontal " + playerNumber) > 0.5f;
    }

    private bool GetLeftKey()
    {
        return Input.GetKey(leftKey) || Input.GetAxis("Horizontal " + playerNumber) < -0.5f;
    }

    private bool GetJumpKeyDown()
    {
        if (jumpKeyFlag && (Input.GetKeyDown(jumpKey) || Input.GetAxisRaw("Jump " + playerNumber) > 0f))
        {
            jumpKeyFlag = false;
            return true;
        } else
        {
            return false;
        }
        

    }

    // Update is called once per frame
    void Update () {
        deathTimer += Time.deltaTime;
        jumpTimer += Time.deltaTime;
        jumpInputTimer += Time.deltaTime;

        var arena = GameObject.FindGameObjectWithTag("Arena");

        var pos = transform.localPosition;
        var forward = Quaternion.Inverse(arena.transform.rotation) * -gameObject.transform.TransformDirection(Vector3.up);

        if (deathTimer > 0 && dead && nbLifes > 0)
        {
            dead = false;
            init(-currentFace);
            gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
        }

        
        if (jumpSpeed == 0 && jumpTimer > 0 && doubleJumpCount > 0)
        {
            Debug.Log("fail time");
            doubleJumpCount = 0;
        }
        
        if (jumpSpeed > 0 && doubleJumpCount > 0 && GetJumpKeyDown())
        {
            Debug.Log("fail input");
            doubleJumpCount = 0;
        }

        if (Input.GetAxisRaw("Jump " + playerNumber) == 0)
        {
            jumpKeyFlag = true;
        }

        if (jumpSpeed == 0)
        {
            if (GetUpKey() && !dead)
            {
                var forwardSpeed = forward * moveSpeed;
                var nextPos = new Vector3(pos.x + forwardSpeed.x, pos.y + forwardSpeed.y, pos.z + forwardSpeed.z);
                nextPos = Utils.WallLimit(nextPos, currentFace);
                transform.localPosition = nextPos;
            }

            if (GetDownKey() && !dead)
            {
                var backwardSpeed = -forward * backSpeed;
                var nextPos = new Vector3(pos.x + backwardSpeed.x, pos.y + backwardSpeed.y, pos.z + backwardSpeed.z);
                nextPos = Utils.WallLimit(nextPos, currentFace);
                transform.localPosition = nextPos;
            }

            if (GetLeftKey() && !dead)
            {
                transform.Rotate(0, 0, -rotSpeed);
            }

            if (GetRightKey() && !dead)
            {
                transform.Rotate(0, 0, rotSpeed);
            }

            if (GetJumpKeyDown() && !dead && (jumpInputTimer > 0 || doubleJumpCount > 0)) //cooldown ou double jump
            {
                //jump
                jumpSpeed = jumpPower;
                if (jumpTimer < 0)
                {
                    jumpSpeed += jumpPower * doubleJumpBonus * doubleJumpCount;
                }
                doubleJumpCount += 1;
                // Debug.Log(doubleJumpCount);
                if (doubleJumpCount >= maxJumpCount)
                {
                    doubleJumpCount = 0;
                }

                var audio = GameObject.FindObjectOfType<AudioSource>();
                audio.PlayOneShot(Resources.Load("sound/JUMPBEGIN-001Mono", typeof(AudioClip)) as AudioClip);

                var random = Mathf.Ceil(Random.value * 6);
                audio.PlayOneShot(Resources.Load("sound/JUMPAIR-00"+ random + "Mono", typeof(AudioClip)) as AudioClip);

                var pschit = UnityEngine.Object.Instantiate(Resources.Load("fart", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
                pschit.transform.parent = arena.transform;
                pschit.transform.localRotation = Quaternion.LookRotation(-forward);
                //pschit.transform.Rotate(0, 0, 0);

                jumpInputTimer = -jumpCooldown;
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


        //var dir = new Vector3(forward.x + currentFace.x - pos.x, pos.y + forward.y + currentFace.y -pos.y, pos.z + forward.z + currentFace.z - pos.z);
        //DrawLine(transform.localPosition, transform.localPosition + dir * 10, Color.red);

    }

    private void init(Vector3 face)
    {
        transform.localPosition = new Vector3(face.x * -5, face.y * -5, face.z * -5);
        currentFace = face;
        transform.localRotation = Quaternion.LookRotation(currentFace, Vector3.back);
    }

    public Vector3 GetCurrentFace()
    {
        return currentFace;
    }

    public void Kill()
    {
        if (!dead && deathTimer > 2f)
        {
            gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
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

    public bool isFlying()
    {
        return jumpSpeed > 0;
    }
    
    private void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, 0.02f);
    }


}
