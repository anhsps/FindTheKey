using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlerKey : Singleton<HandlerKey>
{
    [SerializeField] private GameObject[] keys;
    [SerializeField] private int starCount;// so star an dc de sinh key[0]
    private int star => GameManager14.Instance.star;
    private int maxStar => GameManager14.Instance.maxStar;

    // Start is called before the first frame update
    void Start()
    {
        if (starCount == 0) starCount = maxStar;
        foreach (var item in keys)
            item.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActiveKey()
    {
        if (star == starCount && keys[0])
            keys[0].SetActive(true);
        else if (star == maxStar && keys[1])
            keys[1].SetActive(true);
    }
}
