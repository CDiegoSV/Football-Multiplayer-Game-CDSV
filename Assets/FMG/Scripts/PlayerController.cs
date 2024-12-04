using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerBall;

    [SerializeField] bool canKick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && canKick && playerBall.GetChild(0) != null)
        {
            GameObject ball = playerBall.GetChild(0).gameObject;
            ball.GetComponent<Rigidbody>().isKinematic = false;
            ball.transform.SetParent(null, true);
            ball.GetComponent<Rigidbody>().AddForce(transform.forward * 5f, ForceMode.Impulse);
            StartCoroutine(CoolDownTimer());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball") && !canKick)
        {
            other.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.position = new Vector3(playerBall.position.x, playerBall.position.y + other.gameObject.transform.position.y, playerBall.position.z);
            other.transform.SetParent(playerBall, true);
            canKick = true;
        }
    }


    private IEnumerator CoolDownTimer()
    {
        yield return new WaitForSeconds(1);
        canKick = false;
    }
}
