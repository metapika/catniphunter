using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCore.Menu;

public class PreviousPage : MonoBehaviour
{
    public List<PageType> pageHistory;
    public PageController PageController;
    public GameObject menuBackground;

    void Start()
    {
        menuBackground.SetActive(true);
        PageController = PageController.instance;
        pageHistory = new List<PageType>();
        pageHistory.Clear();
    }

    void Update()
    {
        foreach (Page _page in PageController.pages)
        {
            if(PageController.PageIsOn(_page.type)) {
                if(!pageHistory.Contains(_page.type)) {
                    if(_page.type != PageType.Loading) {
                        pageHistory.Add(_page.type);
                    }
                }
            }
        }

        if(pageHistory.Count > 0) {
            if(Input.GetButtonDown("MenuBack"))
            {
                PageType currentPage = pageHistory[pageHistory.Count - 1];
                PageType targetPage = PageType.None;

                menuBackground.SetActive(true);
                
                if(currentPage != PageType.MainMenu) {
                    targetPage = pageHistory[pageHistory.Count - 2];
                    pageHistory.Remove(currentPage);
                    PageController.TurnPageOff(currentPage, targetPage);
                }
            }
        }
    }
}
