using UnityEngine;
using UnityEngine.UI;

public class DDRTest: MonoBehaviour
{
    public Image[] images;
    
    private void Awake()
    {
        foreach (var image in images)
        {
            image.color = Color.clear;
        }
    }

    private void Update()
    {
        
    }
}