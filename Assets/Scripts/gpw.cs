/*
Visualize data from SEDAC's Gridded Population of the World data set - https://sedac.ciesin.columbia.edu/data/set/gpw-v4-population-count-rev11
Parses the population counts and converts the data into a greyscale texture.
The data file must be in ASCII format, with its extension changed to ".txt" in order for Unity to recognize it as a TextAsset.
*/
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class gpw : MonoBehaviour
{
    public float speed = 15f;
    private float max = 0f;
    public TextAsset datafile;

    private int IntHeaderVal(string s)
        {
        string[] tokens = s.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
        return int.Parse(tokens[1]);
        }

    private float FloatHeaderVal(string s)
        {
        string[] tokens = s.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
        return float.Parse(tokens[1]);
        }

    private Texture2D InitTexture(int width, int height)
        {
        Texture2D texture = new Texture2D(width,height);
        var initColors = new Color32[width*height];
        for (int i=0; i < width*height; i++)
            initColors[i] = Color.black;
        texture.SetPixels32(initColors);
        GetComponent<Renderer>().material.mainTexture = texture;
        return texture;
        }

    private Color TransferFunction(float data)
        {
        Color color;
        if (data > max)
        	color = Color.red;
        else if (data > 0)
        	color = Color.green * data/max;
	else
		color = Color.blue;
        return color;
        }
        
    void Start()
        {
        var separators = new string[] { "\n", "\r\n", "\r" };
        string[] lines = datafile.text.Split(separators, StringSplitOptions.None);
        int ncols = IntHeaderVal(lines[0]);
        int nrows = IntHeaderVal(lines[1]);
        float xllcorner = FloatHeaderVal(lines[2]);
        float yllcorner = FloatHeaderVal(lines[3]);    
        float cellsize = FloatHeaderVal(lines[4]);    
        int width = (int)(360.0f/cellsize);
        int height = (int)(180.0f/cellsize);
        Texture2D texture = InitTexture(width,height);
	    
     	for (int i = 0; i < nrows; i++)
            {
            int texRow = (nrows-1)-i + (int)((90+yllcorner)/cellsize);
            string[] vals = lines[i+6].Split(' ');
            for (int j=0; (j < ncols); j++)
            	{
            	int texCol = j+(int)((180+xllcorner)/cellsize);
                float pop = float.Parse(vals[j]);
                if (pop > max) {
                    // Debug.Log(max);
                    max = pop; 
                }
                texture.SetPixel(texCol, texRow, TransferFunction(pop));
                }
            }
	
	texture.Apply();
    }
    void Update()
    {
        transform.RotateAround(transform.position, transform.up, Time.deltaTime * speed);
    }
}
