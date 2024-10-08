using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TakeTile : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform parentTransform;
    GameObject newTile = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Dodanie karty do rêki gracza
    public void takeNewTile()
    {
        Debug.Log("Wziêto p³ytkê");
        newTile = Instantiate(tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        
        newTile.transform.SetParent(this.transform);
        int childCount = this.transform.childCount;
        newTile.GetComponent<Tile>().number= childCount;//tileTest = this.transform.GetComponent<TextMeshProUGUI>();
        newTile.GetComponent<Tile>().numberColor = UnityEngine.Color.red;
        newTile.name = "Tile_nr_" + childCount;
        TextMeshProUGUI textComponent = newTile.transform.Find("Object/Number_Color").GetComponent<TextMeshProUGUI>();//"ChildObject/GrandChildObject/Text"
        textComponent.text = childCount.ToString(); // Ustaw nowy tekst
        textComponent.color = UnityEngine.Color.red; // Zmieñ kolor na czerwony
        LayoutElement le = newTile.AddComponent<LayoutElement>();
    }

}
