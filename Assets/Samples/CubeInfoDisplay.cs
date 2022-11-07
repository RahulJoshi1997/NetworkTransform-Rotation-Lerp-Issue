using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeInfoDisplay : MonoBehaviour {
    [SerializeField] private RectTransform _uiCanvas;
    [SerializeField] private UnityEngine.UI.Text _uiText;

    void Update() {
        _uiCanvas.SetPositionAndRotation(transform.position + new Vector3(0, 0, -1f), Quaternion.identity);
        _uiText.text = $"pos: {transform.position}\nrot: {transform.eulerAngles}";
    }
}
