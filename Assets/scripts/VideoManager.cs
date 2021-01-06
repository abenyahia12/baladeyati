﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class VideoManager : MonoBehaviour
{
    [SerializeField] RawImage m_PlayTexture;
    [SerializeField] VideoPlayer m_VideoPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        m_VideoPlayer.source = VideoSource.Url;
    }
    public void StartVideo()
    {
        m_VideoPlayer.Play();
        m_PlayTexture.color = Color.white;
    }
    public void StopVideo()
    {
        m_VideoPlayer.Stop();

        m_PlayTexture.color = Color.black;
    }
    public void SetupLink(string filename)
    {
#if UNITY_EDITOR
        m_VideoPlayer.url = Application.dataPath + "/StreamingAssets/" + filename;
#elif UNITY_ANDROID
m_VideoPlayer.url = "jar:file://" + Application.dataPath + "!/assets/" +filename;
#elif UNITY_IOS
m_VideoPlayer.url = Application.dataPath + "/Raw/" +filename; 
#endif

    }
}
