using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour {

    public System.Action<MenuItem> MenuItemSelectCallbak = delegate { };

    Button itemButton;

    public Image image
    {
        get {
            if (itemButton != null)
            {
                return itemButton.image;
            }
            else
            { return GetComponent<Image>();
            }
        }
    }

    // Use this for initialization
    void Start () {
        itemButton = GetComponent<Button>();
        if(itemButton)
        {
            itemButton.onClick.AddListener(OnBtnClick);
        }
    }
	
	// Update is called once per frame
	void OnBtnClick () {
		if(MenuItemSelectCallbak != null)
        {
            MenuItemSelectCallbak.Invoke(this);
        }
	}
}
