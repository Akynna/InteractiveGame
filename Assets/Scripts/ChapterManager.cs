using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Custom class to compare arrays of strings of length 2
class StringArrayEqualityComparer : IEqualityComparer<string[]>
{
    public bool Equals(string[] x, string[] y)
    {
        // Two items are equal if both keys and values are equal.
        return x[0].Equals(y[0]) && x[1].Equals(y[1]) ;
    }

    public int GetHashCode(string[] obj)
    {
        return obj[0].GetHashCode();
    }
}

    public class ChapterManager : MonoBehaviour
{

    public DialoguesTable tables;

    public GameObject canvas;
    public GameObject mainChapter;
    public GameObject secondaryChapter;

    string chapterTag = "Chapter";
    string playChapterName = "PlayChapter";

    List<DialoguesTable.Row> rowTable;
    List<List<string>> validity = new List<List<string>>();

    string validationFile = "Assets/Resources/Dialogues/visited_mapping.csv";

    // Start is called before the first frame update
    void Start()
    {
        rowTable = new List<DialoguesTable.Row>(tables.GetRowList());
        validity = new List<List<string>>(FileManager.ReadCSV(validationFile, ',', false));
        
        StartTree(rowTable);
    }

    /* Starts a new tree of scenes based on the schema passed as argument */
    void StartTree(List<DialoguesTable.Row> rows)
    {
        // Start on the top left corner of the screen
        float pos_x = 0;
        float pos_y = Screen.height;

        string node = rows[0].sceneID;
        int validity = IsVisited(node);

        GameObject chap = CreateNode(node, canvas.gameObject, pos_x, pos_y, validity);

        // Displace the first button with a small offset
        pos_y -= chap.GetComponentInChildren<RectTransform>().rect.height;
        pos_x += chap.GetComponentInChildren<RectTransform>().rect.width / 2f;
        chap.transform.position = new Vector3(pos_x, pos_y);

        AddInitialListeners(chap);

        GameObject line = new GameObject();
        line.transform.SetParent(canvas.transform);
        line.AddComponent<LineRenderer>();
        line.GetComponent<LineRenderer>().SetPositions(new Vector3[] { new Vector3(pos_x, pos_y - chap.GetComponentInChildren<RectTransform>().rect.height / 2f), new Vector3(pos_x, pos_y - chap.GetComponentInChildren<RectTransform>().rect.height) });

        CreateBranches(chap, rows, validity);
    }

    int IsVisited(string node)
    {
        if (!node.Equals("end"))
        {
            Debug.Log(node);
            return int.Parse(validity[1][validity[0].IndexOf(node)]);
        }
        else
        {
            return 0;
        }
    }

    /* Function to initialize all default parameters for a chapter node */
    GameObject CreateNode(string node, GameObject parent, float pos_x, float pos_y, int validity)
    {
        GameObject chap = Instantiate(mainChapter);
        Text txt = chap.GetComponentInChildren<Text>();
        txt.text = node;
        chap.name = node;
        chap.transform.SetParent(parent.transform);

        if(validity == 0)
        {
            GetDirectButtonChildrenByName(chap, playChapterName).GetComponent<Button>().interactable = false;
        }

        return chap;
    }

    /* Add all default listeners to the argument's object */
    void AddInitialListeners(GameObject obj)
    {
        obj.GetComponent<Button>().onClick.AddListener(() => ExtendChapter(obj));
        Button playChapter = GetDirectButtonChildrenByName(obj, playChapterName).GetComponent<Button>();
        playChapter.onClick.AddListener(() => LoadDataPrevToThatPoint(obj));
    }

    /* Creates the branching for each of the childs of the parent node (chap) */
    void CreateBranches(GameObject chap, List<DialoguesTable.Row> rows, int validity)
    {
        DialoguesTable.Row rowWithChildren = rows[tables.FindAll_sceneID(rows[0].sceneID).Count - 1];

        List<string> children = new List<string>(new string[] { rowWithChildren.next_scene1, rowWithChildren.next_scene2, rowWithChildren.next_scene3 });
        children = new List<string>(children.Distinct());

        if(children.Count == 1 && children[0].Equals("end"))
        {
            return;
        }

        List<DialoguesTable.Row> res = new List<DialoguesTable.Row>(rows);
        res.RemoveRange(0, tables.FindAll_sceneID(rows[0].sceneID).Count);

        // A choice child is one which name contains the name of the father e.g: intro_0A is a choice child of intro_0
        foreach (string child in children)
        {
            List<DialoguesTable.Row> childRes = new List<DialoguesTable.Row>(res);
            foreach(string sibling in children)
            {
                if (!sibling.Equals(child))
                {
                    List<DialoguesTable.Row> childRows = new List<DialoguesTable.Row>(tables.FindAll_sceneID(sibling));
                    childRes.RemoveRange(childRes.IndexOf(childRows[0]), childRows.Count);
                }
            }
            int visited = IsVisited(child) * validity;
            GameObject childChap = CreateNode(child, chap, chap.transform.position.x, chap.transform.position.y, visited);
            childChap.SetActive(false);
            AddInitialListeners(childChap);
            CreateBranches(childChap, childRes, visited);
        }
    }

    /* Function given to chapter buttons that when clicked opens all the children nodes and displaces the buttons below */
    void ExtendChapter(GameObject chap)
    {
        // Get all the button children from the given object 
        List<GameObject> children = new List<GameObject>();
        foreach (Transform subChap in chap.transform)
        {
            if(subChap.tag == chapterTag)
                children.Add(subChap.gameObject);
        }

        // For each object below the given object displace them in order to make space for the new children
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(chapterTag))
        {
            if (obj.transform.position.y < chap.transform.position.y)
            {
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y - obj.GetComponent<RectTransform>().rect.height * (children.Count));
            }
        }

        // Make all the children for the given object appear
        for(int i = 0; i < children.Count; i++)
        {
            GameObject child = children[i];
            child.transform.position = new Vector3(chap.transform.position.x + Screen.width / 10f, chap.transform.position.y - chap.GetComponent<RectTransform>().rect.height * (i + 1));
            child.SetActive(true);
        }

        // Change the function in the button to a new one that makes the children disappear
        chap.GetComponent<Button>().onClick.RemoveAllListeners();
        chap.GetComponent<Button>().onClick.AddListener(() => ReduceChapter(chap));
    }

    void ReduceChapter(GameObject chap)
    {
        // Get all the button children from the given object
        List<GameObject> children = new List<GameObject>();
        foreach (Transform subChap in chap.transform)
        {
            if (subChap.tag == chapterTag)
                children.Add(subChap.gameObject);
        }

        // Compute the distance all the objects below will have to move once the children disappear and displace them
        int displace = ReduceAllChildren(chap.transform, chapterTag);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(chapterTag))
        {
            if (obj.transform.position.y < chap.transform.position.y)
            {
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y + obj.GetComponent<RectTransform>().rect.height * (displace));
            }
        }

        // Change the function in the button to a new one that makes the children appear
        chap.GetComponent<Button>().onClick.RemoveAllListeners();
        chap.GetComponent<Button>().onClick.AddListener(() => ExtendChapter(chap));
    }

    /* Makes all the children nodes from given object disappear. Returns the space created in units */
    private int ReduceAllChildren(Transform transform, string tag)
    {
        int acc = 0;
        foreach (Transform child in transform)
        {
            if(child.tag == tag)
            {
                acc += ReduceAllChildren(child, tag);
                if (child.gameObject.activeSelf == true)
                    acc++;
                child.gameObject.SetActive(false);
                child.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                child.gameObject.GetComponent<Button>().onClick.AddListener(() => ExtendChapter(child.gameObject));
            }
        }
        return acc;
    }

    // TODO Update this part
    /* Load all the necessary data to load the game at the chosen point with the score untouched */
    void LoadDataPrevToThatPoint(GameObject obj)
    {

        List<string> path = new List<string>();
        path.Add(obj.name);
        Transform parent = obj.transform.parent;
        while(parent != null)
        {
            path.Add(parent.gameObject.name);
            parent = parent.parent;
        }
        path.RemoveAt(path.Count - 1);

        int score = 0;
        while(path.Count >= 2)
        {
            string currentNode = path[0];
            path.RemoveAt(0);
            List<DialoguesTable.Row> possibleRows = tables.FindAll_sceneID(path[0]);
            DialoguesTable.Row scoreRow = possibleRows[possibleRows.Count - 1];
            score += GetScore(currentNode, scoreRow);
        }

        Debug.Log(score);
    }

    int GetScore(string node, DialoguesTable.Row row)
    {
        if (node.Equals(row.next_scene1))
        {
            return ParseScore(row.score1);
        } else if (node.Equals(row.next_scene2))
        {
            return ParseScore(row.score2);
        } else if (node.Equals(row.next_scene3))
        {
            return ParseScore(row.score3);
        }
        else
        {
            return 0;
        }
    }

    int ParseScore(string score)
    {
        if (score.Equals("NA"))
        {
            return 0;
        }
        else
        {
            return int.Parse(score);
        }
    }
    
    //TODO Load the game at this point
    void StartGameAtChosenPoint(string name, int score)
    {
        //TODO launch the scene with name = name and with a starting score = score
    }

    /* Gets the children buttons that are direct children of the node obj */
    GameObject GetDirectButtonChildrenByName(GameObject obj, string name)
    {
        foreach(Transform child in obj.transform)
        {
            if(child.gameObject.name.Equals(name) && child.parent.name == obj.name)
            {
                return child.gameObject;
            }
        }
        return obj;
    }

}
