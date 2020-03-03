using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public Rigidbody2D Rigidbody2D; //怪物剛體    
    public SpriteRenderer spriteRenderer; //怪物圖片
    public Collider2D Collider2D; //怪物碰撞器
    public Animator animator; //怪物動畫控制器
    public int HP; //怪物血量

    public bool movable; //怪物是否會移動
    public float moveSpeed; //怪物移動速度
    private Direction moveDirection = Direction.Left; //怪物移動方向
    public GameObject leftBorder, rightBorder; //怪物左右移動的邊界
    private float leftBorderX, rightBorderX; //怪物左右移動的邊界的X座標
    

    

    private void Start()
    {
        //取得怪物移動邊界的X座標後，銷毀物件
        leftBorderX = leftBorder.transform.position.x; 
        rightBorderX = rightBorder.transform.position.x; 
        Destroy(leftBorder);
        Destroy(rightBorder);
        
        if (movable)
            animator.SetBool("moving", true); //播放移動動畫

    }
    private void Update()
    {
        if (movable && animator.GetBool("moving")) //若移動動畫播放中
            move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.GetContact(0).normal == Vector2.down)　//若被玩家踩到
        {
            stopMove();

            //若血還夠則受傷，否則死亡
            if (HP >= 2) 
                hurt();
            else
                death();
        }
    }

    //執行怪物移動
    private void move()
    {
        if (moveDirection == Direction.Left) //若目前移動方向為左
        {
            Rigidbody2D.velocity = new Vector2(-moveSpeed, Rigidbody2D.velocity.y); //怪物往左等速移動
            if (transform.position.x < leftBorderX) //若超出左邊界
            {
                moveDirection = Direction.Right; //移動方向改為右
                spriteRenderer.flipX = true;
            }               
        }
        else //若目前移動方向為右，結構同上
        {
            Rigidbody2D.velocity = new Vector2(moveSpeed, Rigidbody2D.velocity.y);
            if (transform.position.x > rightBorderX)
            {
                moveDirection = Direction.Left;
                spriteRenderer.flipX = false;
            }  
        }           
    }
    //執行停止移動
    void stopMove()
    {
        Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y); //左右移動速度歸0
        animator.SetBool("moving", false); //停止移動動畫
    }
    //怪物受傷
    void hurt()
    {
        animator.SetBool("hurt", true); //播放受傷動畫
        HP--; //血量扣1
    }
        
    //當受傷動畫結束
    void recoveFromHurt() 
    {
        animator.SetBool("hurt", false); //停止受傷動畫
        if (movable)
            animator.SetBool("moving", true); //播放移動動畫
    }
    //執行怪物死亡
    void death()
    {
        Collider2D.enabled = false; //停止觸發碰撞
        animator.SetTrigger("death"); //播放死亡動畫
    }
    //當死亡動畫結束
    void Destroy() 
    {
        Destroy(this.gameObject);//銷毀物件
    }
}
