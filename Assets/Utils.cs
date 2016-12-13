using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Utils
{
    public static bool WallCheck(Vector3 pos)
    {
        if (pos.x > 5)
        {
            return true;
        }

        if (pos.x < -5)
        {
            return true;
        }

        if (pos.y > 5)
        {
            return true;
        }

        if (pos.y < -5)
        {
            return true;
        }

        if (pos.z > 5)
        {
            return true;
        }

        if (pos.z < -5)
        {
            return true;
        }

        return false;
    }

    public static Vector3 WallLimit(Vector3 pos, Vector3 face)
    {
        if (pos.x > 5)
        {
            pos.x = 5;
        }

        if (pos.x < -5)
        {
            pos.x = -5;
        }

        if (pos.y > 5)
        {
            pos.y = 5;
        }

        if (pos.y < -5)
        {
            pos.y = -5;
        }


        if (pos.z > 5)
        {
            pos.z = 5;
        }

        if (pos.z < -5)
        {
            pos.z = -5;
        }

        if (face.x != 0)
        {
            if (Math.Abs(pos.y) > 4 && Math.Abs(pos.z) > 4)
            {
                if (Math.Abs(pos.y) > Math.Abs(pos.z))
                {
                    pos.z = 4 * Mathf.Sign(pos.z);
                } else
                {
                    pos.y = 4 * Mathf.Sign(pos.y);
                }
            }
        }

        if (face.y != 0)
        {
            if (Math.Abs(pos.x) > 4 && Math.Abs(pos.z) > 4)
            {
                if (Math.Abs(pos.x) > Math.Abs(pos.z))
                {
                    pos.z = 4 * Mathf.Sign(pos.z);
                }
                else
                {
                    pos.x = 4 * Mathf.Sign(pos.x);
                }
            }
        }

        if (face.z != 0)
        {
            if (Math.Abs(pos.x) > 4 && Math.Abs(pos.y) > 4)
            {
                if (Math.Abs(pos.x) > Math.Abs(pos.y))
                {
                    pos.y = 4 * Mathf.Sign(pos.y);
                }
                else
                {
                    pos.x = 4 * Mathf.Sign(pos.x);
                }
            }
        }

        return pos;
    }

    public static Vector3 GetFace(Vector3 currentPos)
    {
        if (currentPos.y == -5)
        {
            return Vector3.up;
        }
        if (currentPos.y == 5)
        {
            return Vector3.down;
        }
        if (currentPos.x == 5)
        {
            return Vector3.left;
        }
        if (currentPos.x == -5)
        {
            return Vector3.right;
        }
        if (currentPos.z == 5)
        {
            return Vector3.back;
        }
        if (currentPos.z == -5)
        {
            return Vector3.forward;
        }

        Debug.Log("olala");
        return Vector3.up;
    }

    public static void CreateWave(Vector3 position, Vector3 face, int playerNumber, float power)
    {
        var arena = GameObject.FindGameObjectWithTag("Arena");

        GameObject instance = UnityEngine.Object.Instantiate(Resources.Load("wave"+playerNumber, typeof(GameObject)), position, Quaternion.identity) as GameObject;
        instance.transform.parent = arena.transform;
        instance.transform.localRotation = Quaternion.LookRotation(face);
        instance.transform.localPosition += -face * 0.3f;
        Wave wavescript = instance.GetComponent<Wave>();
        wavescript.playerNumber = playerNumber;
        wavescript.face = face;
        wavescript.power = power;

        GameObject instance2 = UnityEngine.Object.Instantiate(Resources.Load("explo", typeof(GameObject)), position, Quaternion.identity) as GameObject;
        instance2.transform.parent = arena.transform;
        instance2.transform.localRotation = Quaternion.LookRotation(face);
        instance2.transform.localPosition += -face * 0.3f;
        GameObject.Destroy(instance2, 0.2f);

        //shake
        arena.GetComponent<Arena>().Shake(face, power);
    }

    public static void FlyBlocks(Vector3 position, Vector3 face, float radius)
    {
        var blocks = GameObject.FindGameObjectsWithTag("Block");

        foreach (GameObject block in blocks)
        {
            var blockScript = block.GetComponent<Block>();
            if (blockScript.GetCurrentFace().Equals(face) && block.transform.position != position)
            {
                var dist = Vector3.Distance(block.transform.position, position);
                if (dist < radius)
                {
                    blockScript.Fly();
                }
            }
        }

        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            var playerScript = player.GetComponent<Player>();
            if (playerScript.GetCurrentFace().Equals(face) && player.transform.position != position && !playerScript.isFlying())
            {
                var dist = Vector3.Distance(player.transform.position, position);
                if (dist < radius)
                {
                    playerScript.Kill();
                }
            }
        }
    }
}
