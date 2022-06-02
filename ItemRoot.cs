using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum TYPE
    { // 아이템 종류.
        NONE = -1, KEY = 0, // 없음, 철광석, 사과, 식물.
        NUM,
    }; // 아이템이 몇 종류인가 나타낸다(=3).
};
public class ItemRoot : MonoBehaviour
{
    // 아이템의 종류를 Item.TYPE형으로 반환하는 메소드.
    public Item.TYPE getItemType(GameObject item_go)
    {
        Item.TYPE type = Item.TYPE.NONE;
        if (item_go != null)
        { // 인수로 받은 GameObject가 비어있지 않으면.
            switch (item_go.tag)
            { // 태그로 분기.
                case "Key": type = Item.TYPE.KEY; break;
            }
        }
        return (type);
    }
}
