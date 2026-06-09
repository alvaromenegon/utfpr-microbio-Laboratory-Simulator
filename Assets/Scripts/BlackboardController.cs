
using System;
using UnityEngine;
using UnityEngine.UI;

public class BlackboardController : MonoBehaviour {
    [SerializeField] private Text title;
    [SerializeField] private Text blackboardText;
    

    public void SetTitle(string title) {
        this.title.text = title;
    }

    public void SetBlackboardText(string text) {
        blackboardText.text = text;
    }

    // Use this for initialization
    void Start() {

     }

        // Update is called once per frame
        void Update() {

        }
    }