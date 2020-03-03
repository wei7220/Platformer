using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenBehavior : MonoBehaviour
{
    public Animator animator;//寶石動畫控制器
    public Collider2D Collider2D;//寶石碰撞器
    public AudioSource audioSource;//音效播放器
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")//被角色撞
        {
            isCollected();
        }
    }
    //當寶石被收集
    void isCollected()
    {
        Collider2D.enabled = false;//停止觸發碰撞
        audioSource.Play();//播放音效
        animator.SetTrigger("Collected");//寶石消失動畫
    }
    //當寶石消失動畫結束
    void Destroy()
    {
        Destroy(this.gameObject);//銷毀物件
    }
}
