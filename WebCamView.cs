using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class WebCamView : MonoBehaviour
{
    // リアルタイム画像表示用UI
    public RawImage realtimeView;
    // キャプチャ画像表示用UI
    public RawImage capturedView;

    // 画像送信先
    public string sendToAddress;
    public int sendToPort;

    // カメラ画像用テクスチャ
    WebCamTexture webcamTexture;
    
    void Start()
    {
        // 最初に見つかったカメラ画像をQVGAで取得
        WebCamDevice camDevice = WebCamTexture.devices[0];
        webcamTexture = new WebCamTexture(camDevice.name, 320, 240);

        // UIのテクスチャに指定して、リアルタイムに更新
        realtimeView.texture = webcamTexture;
        webcamTexture.Play();
    }

    public void capturePixels()
    {
        // リアルタイム画像を複製して新しいテクスチャを生成
        var capturedTex = new Texture2D(webcamTexture.width, webcamTexture.height);
        capturedTex.SetPixels(webcamTexture.GetPixels());
        capturedTex.Apply();

        // UIのテクスチャに指定
        capturedView.texture = capturedTex;

        // JPEGエンコードして、生データを送信
        var jpg = capturedTex.EncodeToJPG(30);
        udpSend(jpg, sendToAddress, sendToPort);
    }

    // UDPデータ送信用関数
    void udpSend(byte[] data, string ipStr, int port)
    {
        //データと送信先を設定
        SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
        socketEventArg.SetBuffer(data, 0, data.Length);
        socketEventArg.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);

        //UDPソケットを作成して送信
        new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp).SendToAsync(socketEventArg);
    }
}
