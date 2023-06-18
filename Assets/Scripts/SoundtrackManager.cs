using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundtrackManager : MonoBehaviour
{
    private static SoundtrackManager instance = null;
    
    [SerializeField]
    private AudioSource[] audio;

    [SerializeField]
    private Slider volumeSlider;

    public Text volumeText;

    private void Awake()
    {
        if (instance == null)
        { 
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        if (instance == this) return; 
        Destroy(gameObject);
    }


    void Start()
    {
        audio[0].enabled = true;
        audio[1].enabled = false;

        volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChanged);
    }

    void Update(){
        if(SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 0){      
            audio[0].enabled = true;
            audio[1].enabled = false;
        } else {
            audio[0].enabled = false;
            audio[1].enabled = true;
        }
    }

    // Hàm xử lý sự kiện thay đổi giá trị của Slider
    void OnVolumeSliderValueChanged(float value)
    {
        // Cập nhật âm lượng dựa trên giá trị của Slider
        foreach (AudioSource source in audio)
        {
            source.volume = value;
        }

        // Hiển thị chỉ số âm lượng
        volumeText.text = Mathf.RoundToInt(value * 100).ToString();
    }
}
