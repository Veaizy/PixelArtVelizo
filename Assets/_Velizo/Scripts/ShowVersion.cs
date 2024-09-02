using TMPro;
using UnityEngine;

/// <summary> Show the Clawful version in a TextMeshProUGUI </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class ShowVersion : MonoBehaviour
{
    /// <summary> automatically find text that will hold the version string
    /// One day you should implement the s&box way: https://github.com/Facepunch/sbox-issues/issues/4659 instead </summary>
    private TextMeshProUGUI versionLabel
    {
        get
        {
            return _versionLabel ??= this.GetComponent<TextMeshProUGUI>(); // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator
        }
    }
    private TextMeshProUGUI _versionLabel;

    void Start()
    {
        OnValidate();
    }

    void OnValidate()
    {
        versionLabel.text = Application.version;
    }
}
