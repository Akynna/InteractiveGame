using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChapterManager : MonoBehaviour
{

    public DialoguesTable tables;

    public GameObject canvas;
    public GameObject mainChapter;
    public GameObject secondaryChapter;

    string chapterTag = "Chapter";
    string playChapterName = "PlayChapter";

    List<List<string>> scenes = new List<List<string>>();
    List<string> header = new List<string>();

    string sceneFile = "Assets/Resources/Dialogues/test.csv";

    // Start is called before the first frame update
    void Start()
    {
        scenes = new List<List<string>>(FileManager.ReadCSV(sceneFile, ',', false));
        header = new List<string>(scenes[0]);
        scenes.RemoveAt(0);
        List<string> listOfScenes = new List<string>();

        listOfScenes = scenes.Select(x => x[0]).Distinct().ToList();

        StartTree(listOfScenes);
    }

    void StartTree(List<string> scenes)
    {
        float pos_x = 0;
        float pos_y = Screen.height;

        string node = scenes[0];

        List<string> temp = new List<string>(scenes);
        temp.RemoveAt(0);

        GameObject chap = CreateNode(node, canvas.gameObject, pos_x, pos_y);

        pos_y -= chap.GetComponentInChildren<RectTransform>().rect.height;
        pos_x += chap.GetComponentInChildren<RectTransform>().rect.width / 2f;

        chap.transform.position = new Vector3(pos_x, pos_y);

        chap.GetComponent<Button>().onClick.AddListener(() => ExtendChapter(chap));
        Button playChapter = GetDirectButtonChildrenByName(chap, playChapterName).GetComponent<Button>();
        playChapter.onClick.AddListener(() => LoadDataPrevToThatPoint(chap));

        CreateBranches(chap, temp);
    }

    GameObject CreateNode(string node, GameObject parent, float pos_x, float pos_y)
    {
        GameObject chap = Instantiate(mainChapter);
        Text txt = chap.GetComponentInChildren<Text>();
        txt.text = node;
        chap.name = node;
        chap.transform.SetParent(parent.transform);

        return chap;
    }

    void CreateBranches(GameObject chap, List<string> scenes)
    {
        List<string> res = new List<string>(scenes);

        List<GameObject> children = new List<GameObject>();

        foreach (string child in scenes)
        {
            if (child.Contains(chap.transform.name))
            {
                res.RemoveAt(res.IndexOf(child));

                GameObject childChap = CreateNode(child, chap, chap.transform.position.x, chap.transform.position.y);
                childChap.SetActive(false);
                children.Add(childChap);
            }
        }

        if(children.Count == 0 && res.Count > 0)
        {
            GameObject nextNode = CreateNode(res[0], chap, chap.transform.position.x, chap.transform.position.y);
            nextNode.SetActive(false);
            res.RemoveAt(0);

            nextNode.GetComponent<Button>().onClick.AddListener(() => ExtendChapter(nextNode));
            Button playChapter = GetDirectButtonChildrenByName(nextNode, playChapterName).GetComponent<Button>();
            playChapter.onClick.AddListener(() => LoadDataPrevToThatPoint(nextNode));

            CreateBranches(nextNode, res);

        }
        else
        {
            foreach (GameObject child in children)
            {
                child.GetComponent<Button>().onClick.AddListener(() => ExtendChapter(child));
                Button playChapter = GetDirectButtonChildrenByName(child, playChapterName).GetComponent<Button>();
                playChapter.onClick.AddListener(() => LoadDataPrevToThatPoint(child));

                CreateBranches(child, res);
            }
        }
    }

    void ExtendChapter(GameObject chap)
    {

        List<GameObject> children = new List<GameObject>();
        foreach (Transform subChap in chap.transform)
        {
            if(subChap.tag == chapterTag)
                children.Add(subChap.gameObject);
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(chapterTag))
        {
            if (obj.transform.position.y < chap.transform.position.y)
            {
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y - obj.GetComponent<RectTransform>().rect.height * (children.Count));
            }
        }

        for(int i = 0; i < children.Count; i++)
        {
            GameObject child = children[i];
            child.transform.position = new Vector3(chap.transform.position.x + Screen.width / 10f, chap.transform.position.y - chap.GetComponent<RectTransform>().rect.height * (i + 1));
            child.SetActive(true);
        }

        chap.GetComponent<Button>().onClick.RemoveAllListeners();
        chap.GetComponent<Button>().onClick.AddListener(() => ReduceChapter(chap));
    }

    void ReduceChapter(GameObject chap)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform subChap in chap.transform)
        {
            if (subChap.tag == chapterTag)
                children.Add(subChap.gameObject);
        }

        int displace = ReduceAllChildren(chap.transform, chapterTag);

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(chapterTag))
        {
            if (obj.transform.position.y < chap.transform.position.y)
            {
                //Debug.Log(obj.name + " ,"  + obj.transform.position.x + " ," + obj.transform.position.y);
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y + obj.GetComponent<RectTransform>().rect.height * (displace));
            }
        }

        chap.GetComponent<Button>().onClick.RemoveAllListeners();
        chap.GetComponent<Button>().onClick.AddListener(() => ExtendChapter(chap));
    }

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

    // TODO Load into your skills the necessary skills that the player should have at this point
    void LoadDataPrevToThatPoint(GameObject obj)
    {
        
        List<string> path = new List<string>();
        path.Add(obj.name);

        Transform parent = obj.transform.parent;
        while(parent != null)
        {
            path.Insert(0, parent.gameObject.name);
            parent = parent.parent;
        }
        path.RemoveAt(0);

        List<string> names = scenes.Select(x => x[0]).ToList();
        List<List<int>> ids = new List<List<int>>();

        foreach(string sceneName in path)
        {
            List<int> tempIds = new List<int>();
            for(int i = 0; i < names.Count; i++)
            {
                string scene = names[i];
                if (scene.Equals(sceneName))
                {
                    tempIds.Add(i);
                }
            }
            if(tempIds.Count > 0)
            {
                ids.Add(tempIds);
            }
        }

        int score = 0;
        int scoreOption = -1;

        for (int i = path.Count - 1; i >= 0; i--)
        {
            string name = path[i];
            
            ids.RemoveAt(ids.Count - 1);
            if (scoreOption != -1)
            {
                List<DialoguesTable.Row> rows = tables.FindAll_sceneID(name);
                if(scoreOption == 0)
                {
                    score += int.Parse(rows[rows.Count - 1].score1);
                } else if(scoreOption == 1)
                {
                    score += int.Parse(rows[rows.Count - 1].score2);
                }
                else
                {
                    score += int.Parse(rows[rows.Count - 1].score3);
                }                
            }
            scoreOption = GetScoreOption(name);
        }

        Debug.Log(score);
        StartGameAtChosenPoint(path[path.Count - 1], score);

    }

    int GetScoreOption(string name)
    {
        if(name.Substring(name.Length - 1).Equals("A"))
        {
            Debug.Log("Choice : A");
            return 0;
        } else if (name.Substring(name.Length - 1).Equals("B"))
        {
            Debug.Log("Choice : B");
            return 1;
        }else if (name.Substring(name.Length - 1).Equals("C"))
        {
            Debug.Log("Choice : C");
            return 2;
        }
        else
        {
            Debug.Log("Choice : None");
            return -1;
        }
    }

    //TODO Load the game at this point
    void StartGameAtChosenPoint(string name, int score)
    {
        //TODO launch the scene with name = name and with a starting score = score
    }


    /*List<string> CreateBranching(string parent, GameObject parentChap, List<string> children)
    {
        List<string> res = new List<string>(children);

        foreach(string child in children)
        {
            if (child.Contains(parent))
            {
                List<string> temp = new List<string>(res);
                temp.RemoveAt(res.IndexOf(child));

                GameObject childChap = Instantiate(secondaryChapter);
                Text txt = childChap.GetComponentInChildren<Text>();
                txt.text = child;
                childChap.name = child;
                childChap.transform.SetParent(parentChap.transform);
                childChap.SetActive(false);

                res = new List<string>(CreateBranching(child, childChap, temp));
            }
            else
            {
                return res;
            }
        }
        return res;
    }*/


    /*void CreateTree(List<string> scenes)
    {
        float pos_x = Screen.width / 2f;
        float pos_y = Screen.height;
        while(scenes.Count != 0)
        {
            string node = scenes[0];
            //Debug.Log(node);
            List<string> temp = new List<string>(scenes);
            temp.RemoveAt(0);

            GameObject chap = Instantiate(mainChapter);
            Text txt = chap.GetComponentInChildren<Text>();
            txt.text = node;
            chap.name = node;
            chap.transform.SetParent(canvas.transform);

            pos_y -= chap.GetComponentInChildren<RectTransform>().rect.height;

            chap.transform.position = new Vector3(pos_x, pos_y);

            scenes = new List<string>(CreateBranching(node, chap, temp));

            chap.GetComponent<Button>().onClick.AddListener(() => ExtendChapter(chap));
        }
    }*/

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
