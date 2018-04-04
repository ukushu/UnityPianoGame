using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PianoKeyScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    private bool _mustBeNamedOnStart = true;
    private float _speedOfSoundAttenuation = 1.0f;
    private int _minOctaveNum = 2;
    private int _maxOctaveNum = 4;


    private AudioSource _audioSource;
    private float _semitone_offset = 0;

    private bool _keyIsStillPressed = false;

    private string CurrNoteName
    {
        get
        {
            return transform.name;
        }
    }
    private int CurrCctaveNum = 0;

    private string GetOfficialNoteName(string noteName)
    {
        switch (noteName)
        {
            case "C":
                return "do";
            case "D":
                return "re";
            case "E":
                return "mi";
            case "F":
                return "fa";
            case "G":
                return "sol";
            case "A":
                return "la";
            case "B":
                return "si";
        }

        return "Error";
    }
    
    void Start () {
        _audioSource = gameObject.AddComponent<AudioSource>();
        
        //Getting number of Octave from parent obj name
        if (gameObject.transform.parent.name.Contains("Octave_"))
        {
            CurrCctaveNum = Int32.Parse(gameObject.transform.parent.name.Replace("Octave_", ""));
        }

        //Generating data for generation of notes that is absent in resources
        if (CurrCctaveNum < _minOctaveNum)
        {
            _audioSource.clip = (AudioClip)Resources.Load("Notes/" + _minOctaveNum + transform.name);

            var octaveOffset = _minOctaveNum - CurrCctaveNum;
            _semitone_offset = - (11 * octaveOffset + octaveOffset);
        }
        else if (CurrCctaveNum > _maxOctaveNum)
        {
            _audioSource.clip = (AudioClip)Resources.Load("Notes/" + _maxOctaveNum + transform.name);

            var octaveOffset = CurrCctaveNum - _maxOctaveNum;
            _semitone_offset = 11 * octaveOffset + octaveOffset;
        }
        else //assign audiofile in case of note exist in resources
        {
            _audioSource.clip = (AudioClip)Resources.Load("Notes/" + CurrCctaveNum + transform.name);
        }

        SetPianoKeyNoteName();
    }

    private void Update()
    {
        AttenuateNoteIfNeeded();
    }

    private void SetPianoKeyNoteName()
    {
        if (_mustBeNamedOnStart)
        {
            var textObj = gameObject.transform.FindChild("Text");

            if (textObj != null)
            {
                textObj.GetComponent<Text>().text = CurrNoteName + CurrCctaveNum + "\r\n\r\n" + GetOfficialNoteName(CurrNoteName);
            }
        }
    }

    private void AttenuateNoteIfNeeded()
    {
        if (_keyIsStillPressed == false && _audioSource.volume >= 0.05)
        {
            _audioSource.volume = _audioSource.volume - _speedOfSoundAttenuation * Time.deltaTime;
        }
    }

    //On Key Down
    public void OnPointerDown(PointerEventData eventData)
    {
        _audioSource.volume = 1;
        PlayNote();
    }

    //On Key Up
    public void OnPointerUp(PointerEventData eventData)
    {
        _keyIsStillPressed = false;
    }
    
    public void PlayNote() {
        _keyIsStillPressed = true;

        _audioSource.Stop();
        //needed for generation of notes from octaves that we have in resources
        _audioSource.pitch = Mathf.Pow (2f, _semitone_offset/12.0f);
		_audioSource.Play ();
	}
    

}
