using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Generate_tile : MonoBehaviour
{
    private TextMeshProUGUI tileTest;
    // Start is called before the first frame update
    void Start()
    {
        // Sprawdzenie, czy komponent istnieje
        tileTest = transform.GetComponent<TextMeshProUGUI>();

        if (tileTest == null)
        {
            Debug.LogError("Nie znaleziono komponentu TextMeshProUGUI na obiekcie: " + gameObject.name);
        }
        else
        {
            Debug.Log("Znaleziono komponent TextMeshProUGUI: " + tileTest.text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Random_tile()
    {
        tileTest = this.transform.GetComponent<TextMeshProUGUI>();
        int randomNumber = Random.Range(1, 14);
        Color[] colors = { Color.red, Color.blue, Color.black, new UnityEngine.Color(1f, 0.647f, 0f), Color.yellow, new UnityEngine.Color(0.5f, 0f, 0.5f) };
        int randomIndex = Random.Range(0, colors.Length);
                                                          
        if (tileTest != null)
        {
             tileTest.text = randomNumber.ToString(); // Ustaw nowy tekst
            tileTest.color = colors[randomIndex]; // Zmieñ kolor na czerwony

        }
        else
        {
           Debug.Log("Nie znaleziono komponentu Tile!");
        }
        //Debug.Log("numer: "+ randomNumber + "\nkolor: "+ colors[randomIndex]);
    }
}
