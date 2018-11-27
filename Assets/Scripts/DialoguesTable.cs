﻿// This code automatically generated by TableCodeGen
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialoguesTable : MonoBehaviour
{
    public TextAsset file;
	public List<Row> rowList = new List<Row>();
	public bool isLoaded = false;

	public void Awake () {
		rowList = new List<Row>();
		Load(file);
		Debug.Log("CSV file loaded.");
	}


	[System.Serializable]
public class Row
	{
		public string sceneID;
		public string character;
		public string dialogue;
		public string dialogue_audio;
		public string good_answer;
		public string bad_answer;
		public string neutral_answer;
		public string good_score;
		public string bad_score;
		public string neutral_score;
		public string next_scene_good;
		public string next_scene_bad;
		public string next_scene_neutral;
		public string background;
		public string background_music;

	}

	public bool IsLoaded()
	{
		return isLoaded;
	}

	public List<Row> GetRowList()
	{
		return rowList;
	}

    public HashSet<string> getCharacterNames()
	{

		HashSet<string> names = new HashSet<string>();

		foreach(Row row in rowList)
		{
			names.Add(row.character);
		}

		return names;
	}

	public void Load(TextAsset csv)
	{
		rowList.Clear();
		string[][] grid = CsvParser2.Parse(csv.text);
		for(int i = 1 ; i < grid.Length ; i++)
		{
			Row row = new Row();
			row.sceneID = grid[i][0];
			row.character = grid[i][1];
			row.dialogue = grid[i][2];
			row.dialogue_audio = grid[i][3];
			row.good_answer = grid[i][4];
			row.bad_answer = grid[i][5];
			row.neutral_answer = grid[i][6];
			row.good_score = grid[i][7];
			row.bad_score = grid[i][8];
			row.neutral_score = grid[i][9];
			row.next_scene_good = grid[i][10];
			row.next_scene_bad = grid[i][11];
			row.next_scene_neutral = grid[i][12];
			row.background = grid[i][13];
			row.background_music = grid[i][14];

			rowList.Add(row);
		}
		isLoaded = true;
	}

	public int NumRows()
	{
		return rowList.Count;
	}

	public Row GetAt(int i)
	{
		if(rowList.Count <= i)
			return null;
		return rowList[i];
	}

	public Row Find_sceneID(string find)
	{
		return rowList.Find(x => x.sceneID == find);
	}
	public List<Row> FindAll_sceneID(string find)
	{
		return rowList.FindAll(x => x.sceneID == find);
	}
	public Row Find_character(string find)
	{
		return rowList.Find(x => x.character == find);
	}
	public List<Row> FindAll_character(string find)
	{
		return rowList.FindAll(x => x.character == find);
	}
	public Row Find_dialogue(string find)
	{
		return rowList.Find(x => x.dialogue == find);
	}
	public List<Row> FindAll_dialogue(string find)
	{
		return rowList.FindAll(x => x.dialogue == find);
	}
	public Row Find_dialogue_audio(string find)
	{
		return rowList.Find(x => x.dialogue_audio == find);
	}
	public List<Row> FindAll_dialogue_audio(string find)
	{
		return rowList.FindAll(x => x.dialogue_audio == find);
	}
	public Row Find_good_answer(string find)
	{
		return rowList.Find(x => x.good_answer == find);
	}
	public List<Row> FindAll_good_answer(string find)
	{
		return rowList.FindAll(x => x.good_answer == find);
	}
	public Row Find_bad_answer(string find)
	{
		return rowList.Find(x => x.bad_answer == find);
	}
	public List<Row> FindAll_bad_answer(string find)
	{
		return rowList.FindAll(x => x.bad_answer == find);
	}
	public Row Find_neutral_answer(string find)
	{
		return rowList.Find(x => x.neutral_answer == find);
	}
	public List<Row> FindAll_neutral_answer(string find)
	{
		return rowList.FindAll(x => x.neutral_answer == find);
	}
	public Row Find_good_score(string find)
	{
		return rowList.Find(x => x.good_score == find);
	}
	public List<Row> FindAll_good_score(string find)
	{
		return rowList.FindAll(x => x.good_score == find);
	}
	public Row Find_bad_score(string find)
	{
		return rowList.Find(x => x.bad_score == find);
	}
	public List<Row> FindAll_bad_score(string find)
	{
		return rowList.FindAll(x => x.bad_score == find);
	}
	public Row Find_neutral_score(string find)
	{
		return rowList.Find(x => x.neutral_score == find);
	}
	public List<Row> FindAll_neutral_score(string find)
	{
		return rowList.FindAll(x => x.neutral_score == find);
	}
	public Row Find_next_scene_good(string find)
	{
		return rowList.Find(x => x.next_scene_good == find);
	}
	public List<Row> FindAll_next_scene_good(string find)
	{
		return rowList.FindAll(x => x.next_scene_good == find);
	}
	public Row Find_next_scene_bad(string find)
	{
		return rowList.Find(x => x.next_scene_bad == find);
	}
	public List<Row> FindAll_next_scene_bad(string find)
	{
		return rowList.FindAll(x => x.next_scene_bad == find);
	}
	public Row Find_next_scene_neutral(string find)
	{
		return rowList.Find(x => x.next_scene_neutral == find);
	}
	public List<Row> FindAll_next_scene_neutral(string find)
	{
		return rowList.FindAll(x => x.next_scene_neutral == find);
	}
	public Row Find_background(string find)
	{
		return rowList.Find(x => x.background == find);
	}
	public List<Row> FindAll_background(string find)
	{
		return rowList.FindAll(x => x.background == find);
	}
	public Row Find_background_music(string find)
	{
		return rowList.Find(x => x.background_music == find);
	}
	public List<Row> FindAll_background_music(string find)
	{
		return rowList.FindAll(x => x.background_music == find);
	}

}