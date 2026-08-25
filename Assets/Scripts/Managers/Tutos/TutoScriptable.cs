using UnityEngine;

[CreateAssetMenu(fileName = "Tuto", menuName = "Tutos/New Tuto")]
public class TutoData : ScriptableObject
{
    public string title;

    [TextArea]
    public string text;

    public Sprite[] images;
}