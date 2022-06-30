/**
 *Copyright(C) 2015 by #COMPANY#
 *All rights reserved.
 *FileName:     #SCRIPTFULLNAME#
 *Author:       #AUTHOR#
 *Version:      #VERSION#
 *UnityVersion：#UNITYVERSION#
 *Date:         #DATE#
 *Description:   
 *History:
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FaceFusion : MonoBehaviour
{
    string timess;//yyyymmddThhmmssZ
    string Datatime;//yyyymmdd
   // public Image[] image;
    string url = "https://visual.volcengineapi.com/?Action=FaceSwap&Version=2020-08-26";
    string AccessKeyID = "";
    string kSecret = "";
   // public RawImage[] rawImages;
    public byte[] postdata;

    public static FaceFusion _install;
    private void Awake()
    {
        _install = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        // postdata = Retust(rawImages[0], rawImages[1]);
        //  StartCoroutine(PostDownLoadTXT(url, postdata, Backdata));
    }

    // Update is called once per frame
    void Update()
    {

    }
    public Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        UnityEngine.Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    /// <summary>
    /// 图片转换成base64编码文本
    /// </summary>
    public String ImgToBase64String(Texture t2d)
    {
        try
        {
            Texture2D texture2D = t2d as Texture2D;

            byte[] bytesArr = DeCompress(texture2D).EncodeToPNG();
            string strbaser64 = Convert.ToBase64String(bytesArr);
            Debug.Log(Time.time + "获取当前图片base64为---" + strbaser64);
            return strbaser64;
        }
        catch (Exception e)
        {
            Debug.Log("ImgToBase64String 转换失败:" + e.Message);
            return null;
        }
    }
    /// <summary>
    ///base64转换成图片
    /// </summary>
    /// <param name="imgComponent"></param>
    /// <param name="recordBase64String"></param>
    public Texture2D Base64ToImg(string recordBase64String)
    {
        string base64 = recordBase64String;
        byte[] bytes = Convert.FromBase64String(base64);

        Texture2D tex2D = new Texture2D(100, 100, TextureFormat.RGBA32, false);
        tex2D.LoadImage(bytes);

        tex2D.Apply();
        // tex2D.;
        Debug.Log(Time.time + "完成");
        return tex2D;
        //return BaiDuAI._install.Base64ToImg(BaiDuAI._install.Body_seg(recordBase64String));


    }
    public IEnumerator PostDownLoadTXT(byte[] postData, Action<Texture2D> ac)
    {
        QianMIng();
        Debug.Log(url);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        string ss = "HMAC-SHA256 Credential=" + AccessKeyID + "/" + Datatime + "/cn-north-1/cv/request, SignedHeaders=content-type;host;x-content-sha256;x-date, Signature=" + Signature;
        Debug.Log("Authorization:" + ss);
        request.SetRequestHeader("Authorization", ss);
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        request.SetRequestHeader("X-Content-Sha256", xcontentsha256);
        request.SetRequestHeader("X-Date", timess);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.isDone && !request.isHttpError)
        {
            DownloadHandler downloadHandler_TXT = request.downloadHandler;
            Debug.Log(Time.time + "下载完成");
            Debug.Log(downloadHandler_TXT.text);
            if (!string.IsNullOrEmpty(downloadHandler_TXT.text))
            {
              //  Backdata(downloadHandler_TXT.text);
                ac(Backdata(downloadHandler_TXT.text));
            }
        }
        else
        {
            Debug.Log(request.error);
            DownloadHandler downloadHandler_TXT = request.downloadHandler;
            Debug.Log(downloadHandler_TXT.text);
         
        }
    }
    public BackData backData;
    public Texture2D  Backdata(string sr)
    {
        backData = JsonUtility.FromJson<BackData>(sr);
       return Base64ToImg(backData.data.image);
    }
    //获取当前时间
    private string GetCurrentTime()
    {
        DateTime dateTime = DateTime.UtcNow;

        string strNowTime = dateTime.ToString("yyyyMMddTHHmmssZ");
        Datatime = dateTime.ToString("yyyyMMdd");
        string st = dateTime.ToLongDateString();

        print(strNowTime);
        return strNowTime;
    }
    public Hs hs;
    string poststr;
     public  byte[] Retust(Texture raw1, Texture raw2)
    {
        string s = "";
        hs.template_base64 = ImgToBase64String(raw2);
        hs.image_base64 = ImgToBase64String(raw1);
        hs.action_id = "faceswap";
        hs.version = "2.0";
        WWWForm form = new WWWForm();
        form.AddField("image_base64", hs.image_base64);
        form.AddField("template_base64", hs.template_base64);
        form.AddField("action_id", hs.action_id);
        form.AddField("version", hs.version);

        poststr = Encoding.UTF8.GetString(form.data);
        //Debug.Log(s);
        return form.data;
    }

    public static string ToHexString(byte[] byteDatas)
    {
        string hexString = string.Empty;
        if (byteDatas != null)
        {
            StringBuilder strB = new StringBuilder();

            for (int i = 0; i < byteDatas.Length; i++)
            {
                strB.Append(byteDatas[i].ToString("X2"));
            }
            hexString = strB.ToString();
        }
        return hexString.ToLower();

    }
    string HexEncode(string plainString)
    {
        byte[] byteDatas = Encoding.UTF8.GetBytes(plainString);
        return ToHexString(byteDatas);

        // return BitConverter.ToString(Encoding.UTF8.GetBytes(plainString)).Replace("-", " ");




    }
    private string HMAC(string secret, string signKey)
    {
        var encoding = Encoding.UTF8;
        byte[] keyByte = encoding.GetBytes(signKey);
        byte[] messageBytes = encoding.GetBytes(secret);
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashmessage);
        }


    }
    protected static string ComputeHash256(string input, HashAlgorithm algorithm)
    {
        Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
        return ToHexString(hashedBytes);
    }
    private static byte[] hmacsha256(string text, byte[] secret)
    {
        //string signRet = string.Empty;
        byte[] hash;
        using (HMACSHA256 mac = new HMACSHA256(secret))
        {
            hash = mac.ComputeHash(Encoding.UTF8.GetBytes(text));
            // signRet = Convert.ToBase64String(hash);
        }
        return hash;

    }
    public string Signature;

    // string kSecret = "TnpCak5XWXpZV1U0WkRaaE5ERmxaR0ZpTmpjeVkyUXlZek0wTWpJMU1qWQ==";
    string xcontentsha256;
    public void QianMIng()
    {
        timess = GetCurrentTime();
        //  xcontentsha256 = HexEncode(Hsha(postdata));
        xcontentsha256 = ComputeHash256(poststr, new SHA256CryptoServiceProvider());
        string CanonicalRequest = "POST\n" + "/" + "\n" + "Action=FaceSwap&Version=2020-08-26\n" + "content-type:application/x-www-form-urlencoded\nhost:visual.volcengineapi.com\nx-content-sha256:" + xcontentsha256 + "\nx-date:" + timess + "\n\n" + "content-type;host;x-content-sha256;x-date\n" + xcontentsha256;
        //  Debug.Log("CanonicalRequest:" + CanonicalRequest);
        string StringToSign = "HMAC-SHA256\n" + timess + "\n" + Datatime + "/cn-north-1/cv/request\n" + ComputeHash256(CanonicalRequest, new SHA256CryptoServiceProvider());//HexEncode(Hsha(CanonicalRequest));
                                                                                                                                                                            //  Debug.Log("StringToSign:" + StringToSign);

        byte[] kDate = hmacsha256(Datatime, Encoding.UTF8.GetBytes(kSecret));
        byte[] kRegion = hmacsha256("cn-north-1", kDate);
        byte[] kService = hmacsha256("cv", kRegion);
        byte[] kSigning = hmacsha256("request", kService);

        Signature = ToHexString(hmacsha256(StringToSign, kSigning));
        Debug.Log("Signature:" + Signature);
    }
}
[System.Serializable]
public class Hs
{
    public string image_base64;
    public string template_base64;

    public string action_id;
    public string version;
}
[System.Serializable]
public class BackData
{
    public int code;
    public Datess data;
    public string message;
    public string request_id;
    public string time_elapsed;

}
[System.Serializable]
public class Datess
{
    public string image;
}
