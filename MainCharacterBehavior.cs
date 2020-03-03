using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum Direction { Left, Right };
enum JumpTrigger { PlayerControl, Enemy }; //區分跳躍為玩家控制或踩到怪物所觸發
public class MainCharacterBehavior : MonoBehaviour
{
    public Joystick Joystick; //玩家的方向控制器    
    public Rigidbody2D Rigidbody2D; //角色剛體
    public SpriteRenderer SpriteRenderer; //角色圖片
    public Animator Animator; //角色動畫控制器
    public float moveSpeed; //角色左右移動速度
    public float jumpSpeed; //角色跳躍速度
    public int token; //已收集的寶石數量
    public Text tokenNum; //顯示已收集寶石數的文字
    public bool controllable; //角色是否可控制
    public float hurtDuration; //受傷無敵時間
    private float hurtTime;//受到傷害的時間點
    public AudioSource audioSource; //音效播放器
    public AudioClip audioClipJump, audioClipHurt, audioClipDeath, audioClipLandOnEnemy; //音效片段

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (controllable)
            getControl();
        else if (Animator.GetBool("hurt") && (Time.time - hurtTime >= hurtDuration))//受傷無敵時間是否結束
        {
            controllable = true;
            resetAnimator();
        }

    }

    //判斷玩家操控指令，執行對應動作
    void getControl()
    {
        //手機控制
        switch (Joystick.Horizontal)
        {
            case 1://搖桿右滑
                move(Direction.Right);
                break;
            case -1://搖桿左滑
                move(Direction.Left);
                break;
            case 0://未控制
                stopMove();
                break;
            default:
                Debug.Log("MainCharacterBehavior.getControl().switch (Joystick.Horizontal):default");
                break;
        }
        //鍵盤控制
        if (Input.GetKey(KeyCode.RightArrow))//當按住右鍵
            move(Direction.Right);
        if (Input.GetKey(KeyCode.LeftArrow))//當按住左鍵
            move(Direction.Left);
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))//當右鍵或左鍵放開
            stopMove();
        if (Input.GetKeyDown(KeyCode.Z))//當按下Z鍵
            jumpButtonClick();
    }
    //執行角色移動(參數：方向)
    void move(Direction direction)
    {
        Animator.SetBool("moving", true);//開始跑步動畫
        switch (direction)
        {
            case Direction.Left:
                SpriteRenderer.flipX = true; //面向左
                Rigidbody2D.velocity = new Vector2(-moveSpeed, Rigidbody2D.velocity.y);//角色往左等速移動
                break;
            case Direction.Right:
                SpriteRenderer.flipX = false;//面向右
                Rigidbody2D.velocity = new Vector2(moveSpeed, Rigidbody2D.velocity.y);//角色往右等速移動
                break;
            default:
                Debug.Log("MainCharacterBehavior.move().switch (direction):default");
                break;
        }
    }
    //當停止控制角色左右移動
    void stopMove()
    {
        Animator.SetBool("moving", false);//停止跑步動畫
    }
    //當按下跳躍鍵
    public void jumpButtonClick() 
    {
        if (controllable)
            jump(JumpTrigger.PlayerControl);
    }
    //執行角色跳躍(參數：跳躍由玩家控制或踩到怪物所觸發)
    void jump(JumpTrigger jumpTrigger)
    {
        if (jumpTrigger == JumpTrigger.PlayerControl && Animator.GetBool("jumping")) //如角色已在跳躍中，則不執行跳躍鍵的動作
            return;
        Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x, jumpSpeed);//角色往上加速
        Animator.SetBool("jumping", true); //跳躍動畫

        switch (jumpTrigger)//依跳躍觸發播放不同音效
        {
            case JumpTrigger.PlayerControl:
                audioPlay(audioClipJump);
                break;
            case JumpTrigger.Enemy:
                audioPlay(audioClipLandOnEnemy);
                break;
            default:
                Debug.Log("MainCharacterBehavior.jump().switch (jumpTrigger):default");
                break;
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy") //人物撞到怪物
        {
            if (collision.GetContact(0).normal == Vector2.up) //角色在怪物上方
            {
                jump(JumpTrigger.Enemy);
            }
            else
            {
                hurt();
            }
        }
        if (collision.gameObject.tag == "platform" && collision.GetContact(0).normal.y > 0) //角色落在平台上
        {
            Animator.SetBool("jumping", false);//停止跳躍動畫
        }
    }

    //角色受傷
    void hurt()
    {
        if (SpriteRenderer.flipX) //面向左則角色往右退
            Rigidbody2D.velocity = new Vector2(moveSpeed , Rigidbody2D.velocity.y);
        else//面向右則角色往左退
            Rigidbody2D.velocity = new Vector2(-moveSpeed , Rigidbody2D.velocity.y);

        controllable = false; 
        Animator.SetBool("hurt", true); //受傷動畫
        audioPlay(audioClipHurt); //受傷音效
        hurtTime = Time.time; //受傷無敵開始時間
    }
    //執行回復初始角色動畫狀態
    void resetAnimator() 
    {
        Animator.SetBool("hurt", false);
        Animator.SetBool("moving", false);
        Animator.SetBool("jumping", false);
        Animator.SetBool("spawn", false);
    }



    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collection")//寶石
        {
            token++;
            tokenNum.text = token.ToString();
        }
        else if (collision.tag == "Death Zone")//死亡區域
        {
            spawn();
        }
        else if (collision.tag == "Victory Zone")//終點
        {
            victory(); 
        }
    }
    //執行角色重生
    void spawn()
    {
        audioPlay(audioClipDeath); //死亡音效
        controllable = false;             
        transform.position = GameObject.Find("SpawnPosition").transform.position;//角色位置回至復活點
        SpriteRenderer.flipX = false; //面向右
        Animator.SetBool("spawn", true); //重生動畫
    }
    //當重生動畫結束
    void spawnFinish()
    {
        controllable = true;
        resetAnimator();
    }
    //執行勝利動作
    void victory()
    {
        controllable = false;
        Rigidbody2D.velocity = new Vector2(0, 0);//角色靜止
        Animator.SetTrigger("Victory");//勝利動畫
    }
    //當勝利動畫結束
    void victoryFinish()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings); //載入下一個場景編號
    }
    //執行音效播放(參數：音效片段)
    void audioPlay(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

}