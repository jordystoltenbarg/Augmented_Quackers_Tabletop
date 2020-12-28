using UnityEngine;

public class InfoTile : MonoBehaviour
{
    public GameObject InfoPanel;
    private InfoDisplay infoDisplay;

    private void Start()
    {
        Pawn.onPawnReachedFinalTileWithGameObject += onPawnFinalTileReached;
        infoDisplay = InfoPanel.GetComponent<InfoDisplay>();
    }

    private void onPawnFinalTileReached(GameObject pGameObject)
    {
        if (pGameObject == this.gameObject && InfoPanel != null)
        {
            HideInfoPanel();
            //infoDisplay.RandomizeInfoText();
            infoDisplay.RandomizeLocalizedString();
            Invoke(nameof(ShowInfoPanel), 0.5f);
        }
    }

    private void HideInfoPanel()
    {
        InfoPanel.SetActive(false);
    }

    private void ShowInfoPanel()
    {
        InfoPanel.SetActive(true);
    }
}
