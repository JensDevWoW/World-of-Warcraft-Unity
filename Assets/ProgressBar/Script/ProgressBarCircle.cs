/*
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]

public class ProgressBarCircle : MonoBehaviour {
    [Header("Title Setting")]
    public string Title;
    public Color TitleColor;
    public Font TitleFont;    

    [Header("Bar Setting")]
    public Color BarColor;
    public Color BarBackGroundColor;
    public Color MaskColor;
    public Sprite BarBackGroundSprite;
    [Range(1f, 100f)]
    public int Alert = 20;
    public Color BarAlertColor;

    [Header("Sound Alert")]
    public AudioClip sound;
    public bool repeat = false;
    public float RepearRate = 1f;

    private Image bar, barBackground,Mask;
    private float nextPlay;
    private AudioSource audiosource;
    private Text txtTitle;
    private float barValue;
    public float BarValue
    {
        get { return barValue; }

        set
        {
            value = Mathf.Clamp(value, 0, 100);
            barValue = value;
            UpdateValue(barValue);

        }
    }

    private void Awake()
    {

        txtTitle = transform.Find("Text").GetComponent<Text>();
        barBackground = transform.Find("BarBackgroundCircle").GetComponent<Image>();
        bar = transform.Find("BarCircle").GetComponent<Image>();
        audiosource = GetComponent<AudioSource>();
        Mask= transform.Find("Mask").GetComponent<Image>();
    }

    private void Start()
    {
        txtTitle.text = Title;
        txtTitle.color = TitleColor;
        txtTitle.font = TitleFont;
       

        bar.color = BarColor;
        Mask.color = MaskColor;
        barBackground.color = BarBackGroundColor;
        barBackground.sprite = BarBackGroundSprite;

        UpdateValue(barValue);


    }

    void UpdateValue(float val)
    {
       
        bar.fillAmount = -(val / 100) + 1f;

        txtTitle.text = Title + " " + val + "%";

        if (Alert >= val)
        {
            barBackground.color = BarAlertColor;
        }
        else
        {
            barBackground.color = BarBackGroundColor;
        }

    }


    private void Update()
    {
       

        if (!Application.isPlaying)
        {
           
            UpdateValue(50);
            txtTitle.color = TitleColor;
            txtTitle.font = TitleFont;
            Mask.color = MaskColor;
            bar.color = BarColor;
            barBackground.color = BarBackGroundColor;
            barBackground.sprite = BarBackGroundSprite;
            
        }
        else
        {
            if (Alert >= barValue && Time.time > nextPlay)
            {
                nextPlay = Time.time + RepearRate;
                audiosource.PlayOneShot(sound);
            }
        }
    }

}
