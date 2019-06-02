const modulesData = {
    "index.rete": {
        "data": {
            "id": "demo@0.1.0",
            "nodes": {
              "1": {
                "id": 1,
                "data": {},
                "inputs": {},
                "outputs": {
                  "first_scene": {
                    "connections": [
                      {
                        "node": 6,
                        "input": "previous_scene",
                        "data": {}
                      }
                    ]
                  }
                },
                "position": [
                  234,
                  208
                ],
                "name": "Start"
              },
              "5": {
                "id": 5,
                "data": {},
                "inputs": {
                  "last_scene": {
                    "connections": [
                      {
                        "node": 7,
                        "output": "answer1",
                        "data": {}
                      },
                      {
                        "node": 7,
                        "output": "answer3",
                        "data": {}
                      },
                      {
                        "node": 7,
                        "output": "answer2",
                        "data": {}
                      }
                    ]
                  }
                },
                "outputs": {},
                "position": [
                  1472,
                  212
                ],
                "name": "End"
              },
              "6": {
                "id": 6,
                "data": {
                  "scene": "example_id",
                  "character_name": "Bob",
                  "dialogue": "Hi",
                  "skill": "Empathy"
                },
                "inputs": {
                  "previous_scene": {
                    "connections": [
                      {
                        "node": 1,
                        "output": "first_scene",
                        "data": {}
                      }
                    ]
                  }
                },
                "outputs": {
                  "next_scene": {
                    "connections": [
                      {
                        "node": 7,
                        "input": "previous_scene",
                        "data": {}
                      }
                    ]
                  }
                },
                "position": [
                  594.0273416684897,
                  83.04213440043193
                ],
                "name": "Dialogue"
              },
              "7": {
                "id": 7,
                "data": {
                  "text4": "Answer 1",
                  "text5": "Answer 2",
                  "text6": "Answer 3",
                  "score4": 5,
                  "score5": -5,
                  "score6": 0,
                  "preview_scene": "..."
                },
                "inputs": {
                  "previous_scene": {
                    "connections": [
                      {
                        "node": 6,
                        "output": "next_scene",
                        "data": {}
                      }
                    ]
                  }
                },
                "outputs": {
                  "answer1": {
                    "connections": [
                      {
                        "node": 5,
                        "input": "last_scene",
                        "data": {}
                      }
                    ]
                  },
                  "answer2": {
                    "connections": [
                      {
                        "node": 5,
                        "input": "last_scene",
                        "data": {}
                      }
                    ]
                  },
                  "answer3": {
                    "connections": [
                      {
                        "node": 5,
                        "input": "last_scene",
                        "data": {}
                      }
                    ]
                  }
                },
                "position": [
                  1003.1377310434116,
                  9.265419198241046
                ],
                "name": "Choices"
              }
            }
          }
    }
};