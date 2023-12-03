using SharedGame;
using STUN;
using STUN.Attributes;
using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class MLUConnectionWidget : ConnectionWidget{
    public Button btnPopulateIP;

    protected override void Awake() {
        base.Awake();
        
        btnPopulateIP.onClick.AddListener(OnPopulateIP);
    }

    private void OnPopulateIP() {
        if (!int.TryParse(inpPlayerIndex.text, out int playerIndex)) {
            return;
        }
            
        string address = "";
        string port = "";
        try
        {
            PublicIp(out address, out port);
        }
        catch (Exception)
        {
            throw;
        }
        playerIndex = Mathf.Clamp(playerIndex, 0, inpIps.Length - 1);
        inpIps[playerIndex].text = address + ":" + port;
    }

    //Use a STUN server for port forwarding, this is done for WAN P2P connections
    private void PublicIp(out string address, out string port)
    {
        if (!STUNUtils.TryParseHostAndPort("stun.schlund.de:3478", out IPEndPoint stunEndPoint))/*stun1.l.google.com:19302*/
            throw new Exception("Failed to establish connection");

        STUNClient.ReceiveTimeout = 500;
        var queryResult = STUNClient.Query(stunEndPoint, STUNQueryType.ExactNAT, true, NATTypeDetectionRFC.Rfc3489);
        if (queryResult.QueryError != STUNQueryError.Success)
            throw new Exception("Connection Failed");


        address = queryResult.PublicEndPoint.Address.ToString();
        port = queryResult.PublicEndPoint.Port.ToString();
    }

}