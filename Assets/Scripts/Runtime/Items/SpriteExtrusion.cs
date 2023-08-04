using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace PaperSouls
{
    internal class SpriteExtrusion : MonoBehaviour
    {
        [SerializeField]
        public GameObject cube;
        [SerializeField]
        private float _cubeSize = 0.01F;
        
        void Start()
        {
            Create3DSprite();
        }

        private void Create3DSprite()
        {
            List<Vector2> pixelList = new List<Vector2>();
            List<Color32> pixelColor = new List<Color32>();

            GetPixels(ref pixelList, ref pixelColor);
            for(int i=0; i<pixelList.Count; i++)
            {
                Vector2 pixel = pixelList[i];
                Color32 color = pixelColor[i];

                //create cubes for each pixel in sprite
                GameObject newObject = Instantiate(cube, transform);
                //position adjusted for cube size
                newObject.transform.position = newObject.transform.position + 
                    new Vector3((pixel.x - (1 - _cubeSize)* pixel.x)*transform.localScale.x, 
                                (pixel.y - (1 - _cubeSize) * pixel.y)*transform.localScale.y, 0);
                newObject.transform.localScale = new Vector3(_cubeSize, _cubeSize, _cubeSize);
                newObject.GetComponent<MeshRenderer>().material.color = color;
            }
        }

        private void GetPixels(ref List<Vector2> pixelList, ref List<Color32> pixelColor)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;

            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                Texture2D texture = spriteRenderer.sprite.texture;
                Color32[] pixels = texture.GetPixels32();

                int width = texture.width;
                int height = texture.height;

                // Get the non-transparent vertices
                GetNonTransparentPixels(pixels, width, height, ref pixelList, ref pixelColor);
            }
            else
            {
                Debug.LogError("SpriteRenderer or Sprite is missing.");
            }
        }

        private void GetNonTransparentPixels(Color32[] pixels, int width, int height, ref List<Vector2> pixelList, ref List<Color32> pixelColor)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // If the pixel is not transparent (alpha > 0), add its position and color
                    Color32 pixel = pixels[y * width + x];
                    if (pixel.a > 0)
                    {
                        // Assuming the sprite pivot is at (0.5, 0.5), you might need to adjust this depending on your sprite's pivot
                        var vertex = new Vector2(x - width * 0.5f, y - height * 0.5f);
                        pixelList.Add(vertex);
                        pixelColor.Add(pixel);
                    }
                }
            }
        }
    }
}
