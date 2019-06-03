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

    var list_nodes = [];
    var story_data = [];

    // Get the list of nodes currently in the editor
    list_nodes = editor.toJSON().nodes;
    story_data = dataToList(list_nodes);
    
    //For debugging
    for (var key in list_nodes) {
        //console.log(list_nodes[key]);
    }

    // Write the story data into a CSV file
    let csvContent = "data:text/csv;charset=utf-8," 
        + story_data.map(e => e.join(",")).join("\n");

    var encodedUri = encodeURI(csvContent);

    window.open(encodedUri);
}

// Convert the JSON data into a list
function dataToList(list_nodes) {

    // Initialization
    var start_found = false;
    var end_found = false;

    var first_node_id = 0;
    var end_node_id = 0;

    // Get the first scene
    for (var key in list_nodes) {

        // Find the start of the scene
        if (list_nodes[key].name == 'Start') {

            // Take a look at the connections of the Start Node
            start_connections = list_nodes[key].outputs.first_scene.connections;
            
            if (start_connections.length == 0) {
                alert('You must connect the Start Node to any other Dialogue Node !');
                return null;
            } else {

                // Set the first Node ID
                first_node_id = start_connections[0].node;

                // Check that the first node ID is NOT a Choices or a End Node
                if (list_nodes[first_node_id].name == 'Choices') {
                    alert('You can\'t connect the Start Node with the Choices Node.\n' +
                        'You have to connect it to a Dialogue Node.');
                    return null;
                } else if (list_nodes[first_node_id].name == 'End') {
                    alert('You can\'t connect the Start Node to the End, otherwise it will be an empty story :c');
                    return null;
                }
            }
            start_found = true;
        }

        // Get the ID of the End node
        if (list_nodes[key].name == 'End') {
            end_found = true;
            end_node_id = key;

            // Check if the End Node is connected
            end_connections = list_nodes[key].inputs.last_scene.connections;

            if (end_connections.length == 0) {
                alert('The End Node has to be connected');
                return null;
            }
        }
    }

    // Additional checks
    if (!start_found) {
        alert('You must add the Start Node and connect it :)');
        return null;
    }

    if (!end_found) {
        alert('You must add the End Node and connect it too :D');
        return null;
    }

    // Initialize the final data table
    var final_data = new Set(data_header);

    // Build the full story data table
    recursiveData(first_node_id, true, end_node_id, list_nodes, final_data, false);

    // Check if the graph was well built
    if (final_data == null) {
        alert('Please check again if all your Nodes are well connected');
    }

    return Array.from(final_data);
}

// Helper function to store a Node's data
function recursiveData(current_node_id, isSingleNode, end_node_id, list_nodes, accList) {


    if (!isSingleNode) {

        // Reconvert the set into an array
        var ids = Array.from(current_node_id);

        // Call the function multiple times to go through all possible paths
        for (var i=0; i < ids.length; i++) {
            recursiveData(ids[i], true, end_node_id, list_nodes, accList);
        }

    } else {

        // Initialization
        var row = Array(18).fill('NA');

        if (current_node_id == end_node_id) {

            var end_found = false;

            // Check that the End row wasn't already added
            var list_rows = Array.from(accList);
            for (var i=0; i < list_rows.length; i++) {
                if (list_rows[i][0] == 'end') {
                    end_found = true;
                }
            }

            if (!end_found) {
                row[0] = 'end';
                accList.add(row);
            }
            
        } else {

            // Check the type of the current Node
            var node_type = list_nodes[current_node_id].name;

            // In the general case, we will fall the the Dialogue Node case
            if (node_type == 'Dialogue') {

                //====================================
                //          First columns
                //====================================

                // Get the data of the current Dialogue Node
                var current_data = list_nodes[current_node_id].data;

                // Store the data of the current Node
                row[0] = current_data.scene;
                row[1] = current_data.character_name;
                row[3] = current_data.dialogue;
                row[5] = current_data.skill;

                // Look at the next connected Node
                var connected_nodes = list_nodes[current_node_id].outputs.next_scene.connections;

                // Get its ID if there is a connected Node
                var next_id = 0;
                if (connected_nodes.length != 0) {
                    next_id = connected_nodes[0].node;
                } else {
                    alert('All dialogues must be connected to another Dialogue Node or the End Node !');
                    accList = null;
                }

                //====================================
                //          Next columns
                //====================================

                // Look at the type of the next Node
                var next_type = list_nodes[next_id].name;

                // If it's a Choices node, continue to fill the current row
                if (next_type == 'Choices') {

                    var next_nodes = [];
                    var next_data = list_nodes[next_id].data;

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
                    next_connection1 = list_nodes[next_id].outputs.answer1.connections;
                    next_connection2 = list_nodes[next_id].outputs.answer2.connections;
                    next_connection3 = list_nodes[next_id].outputs.answer3.connections;

                    if (next_connection1.length == 0 || next_connection2.length == 0 || next_connection3.length == 0) {
                        alert('Some Answer Node(s) is / are not connected.');
                        accList = null;
                    }

                    // Get the next nodes
                    var next_node1 = list_nodes[next_connection1[0].node];
                    var next_node2 = list_nodes[next_connection2[0].node];
                    var next_node3 = list_nodes[next_connection3[0].node];

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

                    next_nodes_ids = new Set([next_node1.id, next_node2.id, next_node3.id]);

                    // Add the row to the data
                    accList.add(row);
                    recursiveData(next_nodes_ids, false, end_node_id, list_nodes, accList);


                // Otherwise, simply take the next scene
                } else if (next_type == 'Dialogue') {

                    var next_scene_id = list_nodes[next_id].data.scene;

                    // Store the same next scene ID
                    row[13] = next_scene_id;
                    row[14] = next_scene_id;
                    row[15] = next_scene_id;

                    // Store the row in the data file
                    accList.add(row);
                    recursiveData(next_id, true, end_node_id, list_nodes, accList);

                } else if (next_type == 'End') {

                    // Store the same next scene ID
                    row[13] = 'end';
                    row[14] = 'end';
                    row[15] = 'end';

                    // Store the row in the data file
                    accList.add(row);
                    recursiveData(next_id, true, end_node_id, list_nodes, accList);
                } 
            }
        }
    }
}

function addModule() {
    modules['story' + Object.keys(modules).length+'.rete'] = { 
        //data: initialData()
    }
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