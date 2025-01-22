using UnityEngine;
using UnityEngine.UI;

public class VisualizeStar : MonoBehaviour
{
    [SerializeField]SaveDataJson saveDataJson;
    [SerializeField] Image[] Star;
    [SerializeField] float alpha;

    private void OnEnable()
    {
        for (int i = 0; i < Star.Length; i++)
        {
            var tempColor = Star[i].color;
            tempColor.a = alpha / 255f;
            Star[i].color = tempColor;
        }

        saveDataJson.VisualizeStar(GetIndex() - 1);

        for (int i = 0; i < saveDataJson.starCount; i++)
        {
            var tempColor = Star[i].color;
            tempColor.a = 1f;
            Star[i].color = tempColor;
        }
        if (saveDataJson.Data.LevelIndex >= GetIndex()-1)
        {
            this.GetComponent<Button>().interactable = true;
        }
        else
        {
            this.GetComponent<Button>().interactable = false;
        }

    }

    public int GetIndex()
    {
        return int.Parse(this.name.Split(' ')[1]);
    }


}
