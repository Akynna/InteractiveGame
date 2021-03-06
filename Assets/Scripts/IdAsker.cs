﻿using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.UI;

public class IdAsker : MonoBehaviour
{
    public GameObject idPrefab;
    public GameObject canvas;

    private GameObject idAsker;

    // Start is called before the first frame update
    void Start()
    {
        idAsker = Instantiate(idPrefab);
        idAsker.transform.SetParent(canvas.transform);
        idAsker.GetComponentInChildren<Button>().onClick.AddListener(() => Validate());
        idAsker.transform.position = canvas.transform.position;
        idAsker.GetComponentInChildren<InputField>().onValueChanged.AddListener(delegate { RegisterID(); });

        float ratio = Convert.ToSingle(Screen.width / idAsker.GetComponent<RectTransform>().sizeDelta.x);
        idAsker.transform.localScale = new Vector3(1, 1, 1);
    }

    void RegisterID()
    {
        PlayerPrefs.SetString("ID", idAsker.GetComponentInChildren<InputField>().text);
    }

    void Validate()
    {
        Destroy(idAsker);
    }
}
