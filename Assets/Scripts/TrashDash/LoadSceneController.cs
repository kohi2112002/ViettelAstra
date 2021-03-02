using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LoadSceneController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public void FadeOut()
    {
        anim.Play("FadeOut");
    }
}
