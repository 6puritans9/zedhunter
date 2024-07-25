using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using UnityEngine.Networking;
using System.IO;

public class ImageUploader : MonoBehaviour
    {
        public Button selectFileButton; // Button to open the file dialog
        public RawImage displayImage; // RawImage component to display the selected image

        private string filePath;
        private byte[] imageBytes;
        private string base64EncodedImage;

        void Start()
            {
                selectFileButton.onClick.AddListener(OpenFileDialog);
            }

        public void OpenFileDialog()
            {
                StartCoroutine(ShowFileBrowser());
            }

        private IEnumerator ShowFileBrowser()
            {
                // Show a load file dialog and wait for a response from the user
                yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null,
                    "Select Image", "Select");

                // Check if the user selected a file
                if (FileBrowser.Success)
                    {
                        filePath = FileBrowser.Result[0];
                        // filePathText.text = "Selected file: " + filePath;
                        StartCoroutine(LoadImage(filePath));
                    }
            }

        private IEnumerator LoadImage(string path)
            {
                yield return null;

                if (File.Exists(path))
                    {
                        imageBytes = File.ReadAllBytes(path);
                        Texture2D texture = new Texture2D(2, 2);
                        if (texture.LoadImage(imageBytes))
                            {
                                print("load image success");
                                displayImage.texture = texture;
                                // displayImage.SetNativeSize();
                                base64EncodedImage = Convert.ToBase64String(imageBytes);
                                print($"Base64 Image: {base64EncodedImage}");
                            }
                        else
                            {
                                Debug.LogError("Failed to load image.");
                            }
                    }
                else
                    {
                        Debug.LogError("File does not exist.");
                    }
            }

        public string GetBase64EncodedImage()
            {
                return base64EncodedImage;
            }
        
    }