var numSocket = new Rete.Socket("Number");
var floatSocket = new Rete.Socket("Float");
var strSocket = new Rete.Socket("String");

var current_dialogue_node_id = 1;
var current_choices_node_id = 1;

// Rearrange the positions of elements inside a node (dialogue)
function adjustDialogueLayoutNodes(node_id) {

    // Get the current node and its children
    node = document.getElementsByName('scene' + node_id)[0].parentNode.parentNode.parentNode;
    childNodes = node.childNodes;

    // Put the previous input scene ID at the top of the node
    node.insertBefore(childNodes[12], childNodes[1]);

    // Put the Output Label at the end of the node
    node.insertBefore(childNodes[4], childNodes[childNodes.length-1]);
}

// Rearrange the positions of elements inside a node (choices)
function adjustChoicesLayoutNodes(node_id) {

    // Get the current node and its children
    node = document.getElementsByName('text' + node_id)[0].parentNode.parentNode.parentNode;
    childNodes = node.childNodes;

    // Rearrange answers' boxes
    node.insertBefore(childNodes[8], childNodes[4]);
    node.insertBefore(childNodes[9], childNodes[6]);
    node.insertBefore(childNodes[10], childNodes[8]);

    // Rearrange Scores' boxes
    node.insertBefore(childNodes[11], childNodes[5]);
    node.insertBefore(childNodes[12], childNodes[8]);
    node.insertBefore(childNodes[13], childNodes[11]);

    // Put the Input Label at the top of the node
    node.insertBefore(childNodes[14], childNodes[3]);
    node.insertBefore(childNodes[17], childNodes[1]);
}

class BabyTextControl extends Rete.Control {

    constructor(emitter, key, name, readonly, type='text') {
        super(key);
        this.emitter = emitter;
        this.key = key;
        this.type = type;

        if(type=='number') {
            this.template = `${name}<input class="score" name="${key}" type="${type}" :readonly="readonly" :value="value" @input="change($event)"/>`;
        } else {
            this.template = `${name}<input name="${key}" type="${type}" :readonly="readonly" :value="value" @input="change($event)"/>`;
        }

        if(key == 'scene') {
            this.template = `${name}<input name="${key + current_dialogue_node_id}" type="${type}" :readonly="readonly" :value="value" @input="change($event)"/>`;
        }

        this.scope = {
            value: null,
            readonly,
            change: this.change.bind(this)
        };
    }

    onChange() {}

    change(e) {
        this.scope.value = this.type === 'number' ? +e.target.value : e.target.value;
        this.update();
        this.onChange();
    }

    update() {
        if (this.key)
            this.putData(this.key, this.scope.value)
        this.emitter.trigger('process');
        this._alight.scan();
    }

    mounted() {
        this.scope.value = this.getData(this.key) || (this.type === 'number' ? 0 : '...');
        this.update();
    }

    setValue(val) {
        this.scope.value = val;
        this._alight.scan()
    }
}

class TextControl extends Rete.Control {

    constructor(emitter, key, input_title, readonly, type='string', input_type='textarea') {
        super();
        this.emitter = emitter;
        this.key = key;
        this.type = type;

        if(input_title != '') {
            this.template = `${input_title}<br><${input_type} name="${key}" rows="2" :readonly="readonly" :value="value" @input="change($event)"/>`;
        } else {
            this.template = `${input_title}<${input_type} name="${key}" rows="2" :readonly="readonly" :value="value" @input="change($event)"/>`;
        }

        this.scope = {
            value: null,
            readonly,
            change: this.change.bind(this)
        };
    }

    onChange() {}

    change(e) {
        this.scope.value = this.type === 'number' ? +e.target.value : e.target.value;
        this.update();
        this.onChange();
    }

    update() {
        if (this.key)
            this.putData(this.key, this.scope.value)
        this.emitter.trigger('process');
        this._alight.scan();
    }

    mounted() {
        this.scope.value = this.getData(this.key) || (this.type === 'number' ? 0 : '...');
        this.update();
    }

    setValue(val) {
        this.scope.value = val;
        this._alight.scan()
    }
}

class SelectorControl extends Rete.Control {

    constructor(emitter, key, name, values, readonly, type='string') {
        super();
        this.emitter = emitter;
        this.key = key;
        this.type = type;
        this.values = values;

        var options_string = '';

        for(var i = 0, len = values.length; i < len; ++i) {
            options_string += '<option value=\"' + values[i].toString() + '\"/>' + values[i].toString() + '</option>';
        }

        this.template = `${name}<br><select name="${key}" :readonly="readonly" :value="value" @input="change($event)">${options_string}</select>`;

        this.scope = {
            value: null,
            readonly,
            change: this.change.bind(this)
        };
    }

    onChange() {}

    change(e) {
        this.scope.value = this.type === 'number' ? +e.target.value : e.target.value;
        this.update();
        this.onChange();
    }

    update() {
        if (this.key)
            this.putData(this.key, this.scope.value)
        this.emitter.trigger('process');
        this._alight.scan();
    }

    mounted() {
        this.scope.value = this.getData(this.key) || (this.type === 'number' ? 0 : this.values[0]);
        this.update();
    }

    setValue(val) {
        this.scope.value = val;
        this._alight.scan()
    }
}

class StartComponent extends Rete.Component {

    constructor() {
        super("Start");
        this.module = {
            socket: strSocket
        }
    }

    builder(node) {
        var next_scene_out = new Rete.Output('first_scene', "First Scene", strSocket, false);

        return node
            .addOutput(next_scene_out);
    }
}

class EndComponent extends Rete.Component {

    constructor() {
        super("End");
        this.module = {
            socket: strSocket
        }
    }

    builder(node) {
        var next_scene_out = new Rete.Input('last_scene', "Last Scene(s)", strSocket, true);

        return node
            .addInput(next_scene_out);
    }
}

class DialogueComponent extends Rete.Component {

    constructor() {
        super("Dialogue");
        this.module = {
            socket: strSocket
        }
        this.template
    }

    builder(node) {
        var previous_scene_ID = new Rete.Input('previous_scene', "Previous Scene", strSocket, true);
        var character_name = new TextControl(this.editor, 'character_name', 'Character Name', false, 'text', 'input');
        var dialogue = new TextControl(this.editor, 'dialogue', 'Dialogue', false, 'text');
        var skills = new SelectorControl(this.editor, 'skill', 'Skill', ['Task', 'Empathy'], true);

        var scene_ID = new BabyTextControl(this.editor, 'scene', 'Scene ID', false, 'text');

        var out1 = new Rete.Output('next_scene', "Next Scene", strSocket, false);

        return node
            .addControl(scene_ID)
            .addInput(previous_scene_ID)
            .addControl(character_name)
            .addControl(dialogue)
            .addControl(skills)
            .addOutput(out1);
    }

    worker(node, inputs, outputs) {

        var scene_id = node.data.scene;
        outputs['next_scene'] = scene_id;   
    }

    created(node) {

        // Rearrange visually elements in the node
        adjustDialogueLayoutNodes(current_dialogue_node_id);
        current_dialogue_node_id = current_dialogue_node_id + 1;
    }

    destroyed(node) {
    }

}

class ChoicesComponent extends Rete.Component {

    constructor() {
        // Display the name of the node
        super("Choices");

        this.module = {
            socket: strSocket
        }

        this.task = {
            outputs: {answer1: 'output', answer2: 'output', answer3: 'output'}
        }
    }

    // Node's features
    builder(node) {

        // The question that leads to the answers
        var input = new Rete.Input('previous_scene', 'Scene ID', strSocket, false);
        var last_dialogue = new BabyTextControl(this.editor, 'preview_scene', '', true, 'text');

        // Answers
        var out1 = new Rete.Output('answer1', 'Answer 1', strSocket, false);
        var out2 = new Rete.Output('answer2', 'Answer 2', strSocket, false);
        var out3 = new Rete.Output('answer3', 'Answer 3', strSocket, false);

        var answers = [];

        for(var i = 0; i < 3; ++i) {
            answers[i] = new TextControl( this.editor,
                'text' + (i + current_choices_node_id).toString(),
                '',
                false,
                'textarea');
        }
            
        var scores = [];

        for(var i = 0; i < 3; ++i) {
            scores[i] = new BabyTextControl( this.editor,
                'score' + (i + current_choices_node_id).toString(),
                'Score ' + (i + 1).toString(),
                false,
                'number');
        }

        return node
            .addInput(input)
            .addControl(answers[0])
            .addControl(answers[1])
            .addControl(answers[2])
            .addControl(scores[0])
            .addControl(scores[1])
            .addControl(scores[2])
            .addOutput(out1)
            .addOutput(out2)
            .addOutput(out3)
            .addControl(last_dialogue);
    }

    // How the node process the data
    worker(node, inputs, outputs,  {
        silent
    } = {}  ) {

        var scene_id = inputs['previous_scene'][0];
        
        // Change dynamically the name of the scene to which it belongs to
        if (!silent)
            this.editor.nodes.find(n => n.id == node.id).controls.get('preview_scene').setValue(scene_id);

    }

    created(node) {
        
        // Rearrange visually elements in the node
        adjustChoicesLayoutNodes(current_choices_node_id);
        current_choices_node_id = current_choices_node_id + 3;
    }

    destroyed(node) {
    }

}



