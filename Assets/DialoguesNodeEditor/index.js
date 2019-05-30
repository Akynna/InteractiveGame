var container = document.querySelector('#rete');
var editor = null;
var initialData = () => ({id: 'demo@0.1.0', nodes: {}});
var modules = {
    ...modulesData
}
var currentModule = {};

const JsRenderPlugin = {
    install(editor, params = {}) {
      editor.on("rendercontrol", ({ el, control }) => {
        if (control.render && control.render !== "js") return;
  
        control.handler(el, editor);
      });
    }
};

// Initialize the array of data
const data_header = [
    ['sceneID',
    'character', 'character_image',
    'dialogue', 'dialogue_audio',
    'main_skill', 'sub_skill',
    'answer1', 'answer2', 'answer3',
    'score1', 'score2', 'score3',
    'next_scene1', 'next_scene2', 'next_scene3',
    'background', 'background_music']
];

function writeCSVfile() {

    // Get the list of nodes currently in the editor
    var list_nodes = editor.toJSON().nodes;

    var current_node;

    story_data = dataToList(list_nodes);
    
    for (var key in list_nodes) {
        //console.log(data_content[key].data.scene);
        //var next_node_id = data_content[key].outputs.next_scene.connections[0].node;
        //var next_scene = data_content[next_node_id].data.scene;
        //console.log(next_scene);

        //const keys = Object.keys(list_nodes[key].data);

        //console.log(list_nodes[key]);
    }

    // console.log(story_data);

    // Write the story data into a CSV file
    let csvContent = "data:text/csv;charset=utf-8," 
        + story_data.map(e => e.join(",")).join("\n");

    var encodedUri = encodeURI(csvContent);
    window.open(encodedUri);
}

// Convert the JSON data into a list
function dataToList(list_nodes) {

    // Initialize the table of data
    story_data = data_header;

    var start_found = false;
    var end_found = false;

    var first_node_id;

    // Get the first scene
    for (var key in list_nodes) {

        // Find the start of the scene
        if (list_nodes[key].name == 'Start') {

            // Take a look at the connections of the Start Node
            start_connections = list_nodes[key].outputs.first_scene.connections;
            
            if (start_connections.length == 0) {
                alert('You must connect the Start Node to any other Dialogue Node !');
                return;
            } else {

                // Set the first Node ID
                first_node_id = start_connections[0].node;

                if (list_nodes[first_node_id].name == 'Choices') {
                    alert('You can\'t connect the Start Node with the Choices Node.\n' +
                        'You have to connect it to a Dialogue Node.');
                    return;
                } else if (list_nodes[first_node_id].name == 'End') {
                    alert('You can\'t connect the Start Node to the End, otherwise it will be an empty story :c');
                    return;
                }
            }
            start_found = true;
        }

        // Also get the ID of the End node
        if (list_nodes[key].name == 'End') {
            end_found = true;

            // Also check if the End Node is connected
            end_connections = list_nodes[key].inputs.last_scene.connections;

            if (end_connections.length == 0) {
                alert('The End Node has to be connected');
                return;
            }
        }
    }

    if (!start_found) {
        alert('You must add the Start Node and connect it :)');
        return null;
    }

    if (!end_found) {
        alert('You must add the End Node and connect it too :D');
        return null;
    }

    // Build the full story data table
    final_data = storeNodeData(first_node_id, true, list_nodes, story_data);

    // Check if the graph was well built
    if (final_data == null) {
        alert('Please check again if all your Nodes are well connected');
    }

    return final_data;
}

// Helper function to store a Node's data and
function storeNodeData(id, isSingleNode, nodes, story_data) {

    // Check if there are multiple IDs or not (multiple paths)
    if (!isSingleNode) {

        // Build the list of IDs so that IDs are unique
        unique_ids = new Set([id]);

        // Call the function multiple times to go through all possible paths
        for (id in unique_ids) {
            storeNodeData(id, true, nodes, story_data);
        }

        return story_data;

    } else {

        // Initialize a new row full of NAs
        var row = Array(18).fill('NA');

        // Check the type of the current Node
        node_type = nodes[id].name;

        console.log(node_type);

        // If we reach the End Node, fill the row accordingly
        // and return the final full CSV data
        if (node_type == 'End') {

            row[0] = 'end';
            story_data.push(row);

            console.log('CEST BON');

            return story_data;
        }

        // In the general case, we will fall the the Dialogue Node case
        if (node_type == 'Dialogue') {

            // Get the data of the current Dialogue Node
            current_data = nodes[id].data;

            // Store the data of the current Node
            row[0] = current_data.scene;
            row[1] = current_data.character_name;
            row[3] = current_data.dialogue;
            row[5] = current_data.skill;

            // Look at the next connected Node
            connected_nodes = nodes[id].outputs.next_scene.connections;

            // Get its ID if there is a connected Node
            if (connected_nodes.length != 0) {
                next_id = connected_nodes[0].node;
            } else {
                alert('All dialogues must be connected to another Dialogue Node or the End Node !');
                return null;
            }

            next_type = nodes[next_id].name;

            // If it's a Choices node, continue to fill the current row
            if (next_type == 'Choices') {

                next_nodes = [];
                next_data = nodes[next_id].data;

                const keys = Object.keys(next_data);

                // Store the different answers' texts
                row[7] = next_data[keys[0]];
                row[8] = next_data[keys[1]];
                row[9] = next_data[keys[2]];

                // Store the different scores' texts
                row[10] = next_data[keys[3]];
                row[11] = next_data[keys[4]];
                row[12] = next_data[keys[5]];

                // Get the Node ID(s) corresponding to each answer
                next_connection1 = nodes[next_id].outputs.answer1.connections;
                next_connection2 = nodes[next_id].outputs.answer2.connections;
                next_connection3 = nodes[next_id].outputs.answer3.connections;

                if (next_connection1.length == 0 || next_connection2.length == 0 || next_connection3.length == 0) {
                    alert('Some Answer Node(s) is / are not connected.');
                    return null;
                }

                // Get the next nodes
                next_node1 = nodes[next_connection1[0].node];
                next_node2 = nodes[next_connection2[0].node];
                next_node3 = nodes[next_connection3[0].node];

                // Store the different next scene IDs
                if (next_node1.name == 'End') {
                    row[13] = 'end';
                } else {
                    row[13] = next_node1.data.scene;
                }

                if (next_node2.name == 'End') {
                    row[14] = 'end';
                } else {
                    row[14] = next_node2.data.scene;
                }

                if (next_node3.name == 'End') {
                    row[15] = 'end';
                } else {
                    row[15] = next_node3.data.scene;
                }

                next_nodes_ids = new Set([next_node1, next_node2, next_node3]);

                // Add the row to the data
                story_data.push(row);

                // Continue to store the next data
                storeNodeData(next_nodes_ids, false, nodes, story_data);

                return story_data;

            // Otherwise, take the next scene
            } else if (next_type == 'Dialogue') {

                next_scene_id = nodes[next_id].data.scene;

                // Store the same next scene ID
                row[13] = next_scene_id;
                row[14] = next_scene_id;
                row[15] = next_scene_id;

                // Store the row in the data file
                story_data.push(row);

                // Continue to store the next data
                storeNodeData(next_id, true, nodes, story_data);

                return story_data;

            } else if (next_type == 'End') {

                // Store the same next scene ID
                row[13] = 'end';
                row[14] = 'end';
                row[15] = 'end';

                // Store the row in the data file
                story_data.push(row);

                // Continue to store the next data
                storeNodeData(next_id, true, nodes, story_data);

                return story_data;
            }            
        }
    }
    return null;
}

function addModule() {
    modules['story'+Object.keys(modules).length+'.rete'] = { data: initialData() }
}

async function openModule(name) {
    currentModule.data = editor.toJSON();
    
    currentModule = modules[name];
    await editor.fromJSON(currentModule.data);
}

alight('#modules', { modules, writeCSVfile, addModule, openModule });


var editor = new Rete.NodeEditor("demo@0.1.0", container);
editor.use(ConnectionPlugin, { curvature: 0.4 });
editor.use(AlightRenderPlugin);
editor.use(ContextMenuPlugin);
editor.use(JsRenderPlugin);
/*editor.use(CommentPlugin, { 
    margin: 20 // indent for new frame comments by default 30 (px)
})*/
//editor.use(MinimapPlugin);


var engine = new Rete.Engine("demo@0.1.0");

editor.use(ModulePlugin, { engine, modules });

var components = [
    new StartComponent(),
    new DialogueComponent(),
    new ChoicesComponent(),
    new EndComponent()
  ];

components.map(c => {
    editor.register(c);
    engine.register(c);
});


editor.on("process connectioncreated connectionremoved", () => {
    requestAnimationFrame(async () => {
        await engine.abort();
        await engine.process(editor.toJSON());
    });
});

editor.view.resize();

/*editor.trigger('addcomment', ({ type: 'frame', text, nodes }))
editor.trigger('addcomment', ({ type: 'inline', text, position }))

editor.trigger('removecomment', { comment })
editor.trigger('removecomment', { type })*/

openModule('index.rete')