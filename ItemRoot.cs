using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum TYPE
    { // ������ ����.
        NONE = -1, KEY = 0, // ����, ö����, ���, �Ĺ�.
        NUM,
    }; // �������� �� �����ΰ� ��Ÿ����(=3).
};
public class ItemRoot : MonoBehaviour
{
    // �������� ������ Item.TYPE������ ��ȯ�ϴ� �޼ҵ�.
    public Item.TYPE getItemType(GameObject item_go)
    {
        Item.TYPE type = Item.TYPE.NONE;
        if (item_go != null)
        { // �μ��� ���� GameObject�� ������� ������.
            switch (item_go.tag)
            { // �±׷� �б�.
                case "Key": type = Item.TYPE.KEY; break;
            }
        }
        return (type);
    }
}
