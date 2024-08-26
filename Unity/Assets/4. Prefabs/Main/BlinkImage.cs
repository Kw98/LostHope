using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlinkImage : MonoBehaviour
{
    [SerializeField] private LoopType loopType;
    [SerializeField] private float loopTime;
    [SerializeField] private Image Image;

    // Start is called before the first frame update
    void Start()
    {
        Image.DOFade(0.0f, loopTime).SetLoops(-1, loopType);
    }
}
