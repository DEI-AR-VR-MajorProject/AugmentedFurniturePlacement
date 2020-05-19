using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageGallery : MonoBehaviour
{

    public RawImage Img;
    public GameObject ButtonPrefab;
    public Transform content;


    // Start is called before the first frame update
    private void Start()
    {
        Texture[] textures = Resources.LoadAll<Texture>("images");

        foreach (Texture t in textures)
        {
            //Debug.Log(t);
            if (t.name.Equals("error") || t.name.Equals("download") || t.name.Equals("loading"))
                Debug.Log(t);
            else
            {
                GameObject ThisButton = Instantiate(ButtonPrefab) as GameObject;
                ThisButton.transform.SetParent(content);
                ThisButton.GetComponent<RawImage>().texture = t;

                ThisButton.GetComponentInChildren<Text>().text = t.name;
                ThisButton.name = t.name;

                ThisButton.GetComponent<Button>().onClick.AddListener(() => OnClickSelectFurniture(t.name));

            }
        }
    }

    public void OnClickSelectFurniture(string name)
    {
         
    }

    
}
