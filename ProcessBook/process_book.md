# Interactive Game : Process Book



## Table of contents

- [Concept of the game](#concept-of-the-game)
    - [Inspirations](#inspirations)
- [Design](#design)
    - [Environment](#enviroment)
        - [Backgrounds](#background)
        - [Choice of colors](#choice-of-colors)
    - [Characters](#characters)
- Creating the game
    - Importing the data
    - Creating the UI
- Story Timeline
    - Tree choices
    - Dialogue Manager
- Useful links






# Concept of the game

The main goal of this project was to create something that will allow young students to learn how to interact with each other or with other persons in the context of their apprenticeship.

Many situations can be possible : how to talk politely to a new client in a shop, how to approach other colleagues, how to welcome someone new, and so on.
All of these everyday applications may seem easy to handle for some people, but for others that have more difficulties with communicating, this game can be a great way of training !

In this game,  the user is immersed in a story and meet multiple different characters. Depending on the choices the player has decided to take will derive different scenarios. The objective is to learn how people would react to what the user has decided to say / to do and to see the multiple possible endings.



## Inspirations

Similar experiences already exists in the industry of gaming. A lot of people have been reading _**Choose Your Path Story Books**_ since their childhood and now this concept has been transferred by many developers into video games. What makes this kind of entertainment so popular is the possibility to test different scenarios and see their consequences.

A lot of games that I have experienced inspired me for this project.	



[Talk about famous video games on this subject]



# Design

For the design, I wanted to create an environment that would suit to whatever universe we are in. For this project, I was asked to create a story in a hospital context.



## Environment

### Background





### Choice of colors





TODO :

- Find backgrounds and others design resources
- Add "ask your name" 
- Draw characters
- Find scenarios
- Make choices place randomly
- Create a table of scenes





# Creating the game

The game conception can be simplified into two main tasks : handling the data and make a dynamic User Interface. 



## Importing the data

The data is a set of dialogues that will be displayed in the game. I have chosen to store them in a csv file, since it is a format that is quite easy to handle and also because it gives a nice visualization of the dialogues and all the other complementary information.

After many tests to see which structure was the best, I ended up with a file with the following columns:

[show image]





* sceneID: Identifies a scene
* character: The character's name who is speaking
* dialogue: The text dialogue that the user will read
* good_answer: The good answer the user can give
* neutral_answer:



### CSV2Table

Since we are working in Unity, the first thing I needed was to write a parser that would read the csv file and store everything in a nice and clean way. After a bit of research, I found on the Asset Store the **CSV2Table** Asset (https://assetstore.unity.com/packages/tools/utilities/csv2table-36443), which generates a C# code for a CSV parser.









## Creating the User Interface

Generally, in games of type *story*, 





# Story Timeline

This part covers the technical part of the game and explains how I have





TODO :

Random feedback
Change face

Scores 
Test audio files



Add column to say when it is the end of a scene







Useful links :

https://www.youtube.com/watch?v=zc8ac_qUXQY

https://blog.kongregate.com/choosing-fonts-for-your-game/

