using UnityEngine;

public class InfoManager : MonoBehaviour
{
    public static InfoManager singleton = null;
    [HideInInspector] public InfoText[] infoTexts;
    private InfoText currentInfo;
    public InfoText CurrentInfo => currentInfo;

    private void Awake()
    {
        InfoManager[] IM = FindObjectsOfType<InfoManager>();

        if (IM.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;

        LoadInfoTexts();
    }

    private void LoadInfoTexts()
    {
        infoTexts = Resources.LoadAll<InfoText>("Information");
    }

    public void SetInfoText(int index)
    {
        currentInfo = infoTexts[index];
    }
}
