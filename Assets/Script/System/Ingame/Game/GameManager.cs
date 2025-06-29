﻿using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    float _duration = 0.5f;
    private void Awake()
    {
        ServiceLocator.Set(this);
    }

    void Start()
    {
        //FadeIn
        ServiceLocator.Get<GameUIManager>().PanelUIView.Fade(TargetType.Image, 0, _duration);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
