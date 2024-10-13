using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform playerTransform1;
    public Transform playerTransform2;
    public float offsetX = 0.0f;            
    public float offsetY = 0.0f;           
    public float offsetZ = -10.0f;
    Vector3 TargetPos;
    // Start is called before the first frame update
    void Start()
    {
        string character = PlayerPrefs.GetString("PlayerCharcter", "PlayerCharcter");
    }

    // Update is called once per frame
    void Update()
    {
        string character = PlayerPrefs.GetString("PlayerCharcter", "PlayerCharcter");
        if (character != null)
        {
            switch (character)
            {
                case "character1":
                    TargetPos = new Vector3(
                        playerTransform1.transform.position.x + offsetX,
                        playerTransform1.transform.position.y + offsetY,
                        playerTransform1.transform.position.z + offsetZ
                    );
                    break;
                case "character2":
                    TargetPos = new Vector3(
                        playerTransform2.transform.position.x + offsetX,
                        playerTransform2.transform.position.y + offsetY,
                        playerTransform2.transform.position.z + offsetZ
                    );
                    break;
            }
        }
        transform.position = TargetPos;
    }
}
