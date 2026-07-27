using UnityEngine;

public class MenuUiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject browserPanel;

    public void OpenBrowserPanel()
    {
        browserPanel.SetActive(true);
    }
}
