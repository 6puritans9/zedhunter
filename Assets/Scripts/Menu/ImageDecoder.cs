using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageDecoder : MonoBehaviour
{
    public RawImage userImage; // The RawImage component to display the decoded image

    public void SetUserImage(string base64String)
        {
            // Decode the Base64 string into a byte array
            byte[] imageBytes = Convert.FromBase64String(base64String);
            print($"imageBytes: {imageBytes}");

            // Create a new Texture2D
            Texture2D texture = new Texture2D(2, 2);

            // Load the byte array into the Texture2D
            if (texture.LoadImage(imageBytes))
                {
                    // Assign the Texture2D to the RawImage component
                    userImage.texture = texture;
                    // userImage.SetNativeSize();
                }
            else
                {
                    Debug.LogError("Failed to load texture from Base64 string");
                }
        }
}
