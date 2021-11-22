using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemsList : MonoBehaviour
{
    [SerializeField]
    private GameObject itemsRowContainerPrefab;
    [SerializeField]
    private ScrollRect parentScrollRect;
    [SerializeField]
    private GameObject itemsContainer;

    private SceneInitializer sceneInitializer;

    [SerializeField]
    private Row[] rows;

    [System.Serializable]
    public struct Item
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private string label;
        [SerializeField]
        private GameObject jewel;
        [SerializeField]
        private JewelProperties properties;

        public string Name
        {
            get { return name; }
        }

        public string Label
        {
            get { return label; }
        }

        public GameObject Jewel
        {
            get { return jewel; }
        }

        public JewelProperties Properties
        {
            get { return properties; }
        }
    }

    [System.Serializable]
    public struct Row
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private Item[] items;

        public string Name
        {
            get { return name; }
        }

        public Item[] Items
        {
            get { return items; }
        }
    }

    private void Awake()
    {
        sceneInitializer = GameObject.FindGameObjectWithTag("Global Resource").GetComponent<SceneInitializer>();
    }

    private void Start()
    {
        foreach(Row row in rows)
        {
            GameObject rowContainer = Instantiate(itemsRowContainerPrefab, itemsContainer.transform);
            ItemsRowContainer itemsRowContainer = rowContainer.GetComponent<ItemsRowContainer>();
            itemsRowContainer.InitializeRow(row.Name, parentScrollRect);
            foreach(Item item in row.Items)
            {
                itemsRowContainer.CreateItemPanel(item.Name, item.Label, item.Jewel, item.Properties, sceneInitializer);
            }
        }
    }
}
