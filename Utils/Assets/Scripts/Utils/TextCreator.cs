using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Utils
{
    public static class TextCreator
    {
        public static TextMesh CreateWorldText(
            string text,
            Transform parent = null,
            Vector3 localPosition = default(Vector3),
            int fontSize = 40,
            Color? color = null,
            TextAnchor textAnchor = TextAnchor.UpperLeft,
            TextAlignment textAlignment = TextAlignment.Left,
            int sortingOrder = 5000)
        {

            Color textColor = (color == null) ? Color.white : (Color)color;

            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.fontSize = fontSize;
            textMesh.text = text;
            textMesh.color = textColor;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMesh;
        }

        public static void CreatePopupText(
            string text,
            float popupSpeed = 20f,
            float disappearSpeed = 1f,
            Transform parent = null,
            Vector3 localPosition = default(Vector3),
            Camera? cameraToFace = null,
            int fontSize = 15,
            Color? color = null,
            Color? outlineColor = null,
            float outlineWidth = 0.2f,
            Vector2 textAnchor = default(Vector2),
            TMPro.TextAlignmentOptions textAlignment = TMPro.TextAlignmentOptions.Midline,
            int sortingOrder = 5000)
        {
            Color textColor = (color == null) ? Color.white : (Color)color;
            Color textOutlineColor = (outlineColor == null) ? Color.black : (Color)color;

            GameObject popupText = new GameObject("Popup_Text", typeof(TextMeshPro));
            if (cameraToFace != null)
            {
                popupText.transform.LookAt(cameraToFace.transform);
                popupText.transform.rotation = Quaternion.LookRotation(cameraToFace.transform.forward);
            }

            Transform transform = popupText.transform;
            transform.SetParent(parent, false);
            
            TextMeshPro textMeshPro = popupText.GetComponent<TextMeshPro>();
            textMeshPro.rectTransform.sizeDelta = new Vector2(0.1f, 0.1f);
            textMeshPro.enableWordWrapping = false;
            textMeshPro.rectTransform.position = localPosition;
            textMeshPro.rectTransform.anchoredPosition = textAnchor;
            textMeshPro.alignment = textAlignment;
            textMeshPro.fontSize = fontSize;
            textMeshPro.text = text;
            textMeshPro.overflowMode = TextOverflowModes.Overflow;
            textMeshPro.faceColor = textColor;
            Material mat = textMeshPro.fontSharedMaterial;
            mat.shaderKeywords = new string[] { "OUTLINE_ON" };
            textMeshPro.outlineColor = textOutlineColor;
            textMeshPro.outlineWidth = outlineWidth;
            textMeshPro.color = Color.white;
            textMeshPro.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            popupText.AddComponent<PopupText>().StartPopup(textMeshPro, localPosition, popupSpeed, disappearSpeed);
        }


        public class PopupText : MonoBehaviour
        {
            private TextMeshPro text;
            private Color textColor;
            private float popupSpeed;
            private float disappearTime;
            private float disappearSpeed;

            IEnumerator PopupCoroutine()
            {
                while (textColor.a > 0)
                {
                    transform.position += new Vector3(0, popupSpeed) * Time.deltaTime;

                    disappearTime -= Time.deltaTime;
                    if (disappearTime < 0)
                    {
                        textColor.a -= disappearSpeed * Time.deltaTime;
                        text.color = textColor;
                    }

                    yield return null;
                }

                Destroy(gameObject);
            }

            public void StartPopup(TextMeshPro text, Vector3 position, float popupSpeed, float disappearSpeed)
            {
                this.text = text;
                textColor = text.color;
                transform.position = position;
                this.popupSpeed = popupSpeed;
                this.disappearSpeed = disappearSpeed;
                disappearTime = popupSpeed / 4f;

                StartCoroutine(PopupCoroutine());
            }
        }
    }
}
