using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PunchPlayer : MonoBehaviour
{
    public static PunchPlayer Instance;
    public PhotonView photonView;
    public Canvas uiCanvas;
    public Text nameText;
    public PlayerMovement movement;
    public Animator animator;
    public CharacterController characterController;
    public Player lasthite;
    private Vector3 direction;
    private bool punching;
    private bool dead;


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        nameText.text = photonView.Owner.NickName;
        if(photonView.IsMine)
        {
            Instance = this;           
        }
        else
        {
            movement.enabled = false;
        }
    }

    private void Update()
    {
        uiCanvas.transform.LookAt(Camera.main.transform);
        
        if (dead) return;
        
        if (transform.position.y < -10)
        {
            OnDie();
            return;
        }

        if (!punching && Input.GetMouseButtonDown(0))
        {
            punching = true;
            movement.canMove = false;
            animator.SetTrigger("punch");
        }
    }

    private void FixedUpdate()
    {
        if (dead) return;
        
        if (direction != Vector3.zero)
        {
            characterController.Move(direction);
            direction = Vector3.Lerp(direction, Vector3.zero, Time.deltaTime * 5);
        }
    }

    public void OnDie()
    {
        if(lasthite != null)
        {
            photonView.RPC("OnKill", lasthite);
        }

        GameManager.Instance.OnDie();
    }

    [PunRPC]
    public void OnKill(PhotonMessageInfo info)
    {
        string victimName = info.Sender.NickName;
        Hashtable custom = PhotonNetwork.LocalPlayer.CustomProperties;
        if(custom.ContainsKey("kills"))
        {
            custom["kills"] = ((int) custom["kills"]) + 1;
        }
        else
        {
            custom.Add("kills", 1);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(custom);
        GameManager.Instance.OnKill(victimName);
    }

    public void OnPunch()
    {
        if(!photonView.IsMine)
        {
            return;
        }
        Physics.Raycast(
            transform.position + Vector3.up,
            transform.forward,
            out var hit, 8f,
            LayerMask.GetMask("Player")
        );

        if (hit.collider != null)
        {
            PhotonView view = hit.collider.GetComponent<PhotonView>();
            view.RPC("OnHit", view.Owner, transform.forward);
            
        }
    }

    [PunRPC]
    // public void OnHit(Vector3 punchDirection, PhotonMessageInfo info)
    // {
    //     lasthite = info.Sender;
    //     direction = punchDirection;
    // }

    public void OnPunchEnd() // animation event
    {
        punching = false;
        movement.canMove = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        direction = Vector3.zero;
    }
}