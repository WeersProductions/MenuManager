using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeersProductions;

public class MCChatBox : MCMenu
{
    [SerializeField]
    private Text _chatHistory;

    [SerializeField]
    private InputField _inputField;
    
    public override void Show(object data)
    {
        if (data == null)
        {
            // We don't have any chat history.
        }
        else
        {
            // We have chat history, we should show it.
            if (data is ChatData chatData)
            {
                _chatHistory.text = chatData.ChatHistory;
            }
            else
            {
                Debug.LogError($"Received {data}, which cannot be cast to ChatData.");
            }
        }
        
        base.Show(data);
    }

    public void Send()
    {
        if (string.IsNullOrEmpty(_inputField.text))
        {
            return;
        }
        _chatHistory.text += $"{DateTime.Now.ToShortTimeString()} - {_inputField.text}{Environment.NewLine}";
        _inputField.text = "";
    }

    public void Close()
    {
        Hide();
    }
}
