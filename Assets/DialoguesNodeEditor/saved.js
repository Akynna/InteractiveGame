var numSocket = new Rete.Socket("Number");
var floatSocket = new Rete.Socket("Float");
var strSocket = new Rete.Socket("String");

var strSocket = new Rete.Socket("String");


function adjustLayoutNodes() {
    document.querySelectorAll('.node.choices').forEach(dialogue_node => {
        childNodes = dialogue_node.childNodes
        dialogue_node.insertBefore(childNodes[11], childNodes[4]);
        dialogue_node.insertBefore(childNodes[12], childNodes[6]);
        dialogue_node.insertBefore(childNodes[13], childNodes[8]);
    }); 
}

class TextControl extends Rete.Control {

    constructor(emitter, key, readonly, type = 'text') {
        super();
        this.emitter = emitter;
        this.key = key;
        this.type = type;
        this.template = `<input type="${type}" :readonly="readonly" :value="value" @input="change($event)"/>`;

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

class InputControl extends Rete.Control {
    constructor(key) {
      super(key);
      this.render = "js";
      this.key = key;
    }
  
    handler(el, editor) {
      var input = document.createElement("input");
      el.appendChild(input);
  
      var text = this.getData(this.key) || "Some message..";
  
      input.value = text;
      this.putData(this.key, text);
      input.addEventListener("change", () => {
        this.putData(this.key, input.value);
      });
    }
}


class InputComponent extends Rete.Component {

    constructor() {
        super("Input");
        this.module = {
            nodeType: 'input',
            socket: numSocket
        }
    }

    builder(node) {
        var out1 = new Rete.Output('output', "Number", numSocket);
        var ctrl = new TextControl(this.editor, 'name');

        return node.addControl(ctrl).addOutput(out1);
    }
}


class ModuleComponent extends Rete.Component {

    constructor() {
        super("Module");
        this.module = {
            nodeType: 'module'
        }
    }

    builder(node) {
        var ctrl = new TextControl(this.editor, 'module');
        ctrl.onChange = () => {
            console.log(this)
            this.updateModuleSockets(node);
            node._alight.scan();
        }
        return node.addControl(ctrl);
    }

    change(node, item) {
        node.data.module = item;
        this.editor.trigger('process');
    }
}


class OutputComponent extends Rete.Component {

    constructor() {
        super("Output");
        this.module = {
            nodeType: 'output',
            socket: numSocket
        }
    }

    builder(node) {
        var inp = new Rete.Input('input', "Number", numSocket);
        var ctrl = new TextControl(this.editor, 'name');

        return node.addControl(ctrl).addInput(inp);
    }
}


class OutputFloatComponent extends Rete.Component {

    constructor() {
        super("Float Output");
        this.module = {
            nodeType: 'output',
            socket: floatSocket
        }
    }

    builder(node) {
        var inp = new Rete.Input('float', "Float", floatSocket);
        var ctrl = new TextControl(this.editor, 'name');

        return node.addControl(ctrl).addInput(inp);
    }
}

class NumComponent extends Rete.Component {

    constructor() {
        super("Number");
    }

    builder(node) {
        var out1 = new Rete.Output('num', "Number", numSocket);
        var ctrl = new TextControl(this.editor, 'num', false, 'number');

        return node.addControl(ctrl).addOutput(out1);
    }

    worker(node, inputs, outputs) {
        outputs['num'] = node.data.num;
    }
}


class AddComponent extends Rete.Component {
    constructor() {
        super("Add");
    }

    builder(node) {
        var inp1 = new Rete.Input('num1', "Number", numSocket);
        var inp2 = new Rete.Input('num2', "Number", numSocket);
        var out = new Rete.Output('num', "Number", numSocket);

        inp1.addControl(new TextControl(this.editor, 'num1', false, 'number'))
        inp2.addControl(new TextControl(this.editor, 'num2', false, 'number'))

        return node
            .addInput(inp1)
            .addInput(inp2)
            .addControl(new TextControl(this.editor, 'preview', true))
            .addOutput(out);
    }

    worker(node, inputs, outputs, {
        silent
    } = {}) {
        var n1 = inputs['num1'].length ? inputs['num1'][0] : node.data.num1;
        var n2 = inputs['num2'].length ? inputs['num2'][0] : node.data.num2;
        var sum = n1 + n2;

        if (!silent)
            this.editor.nodes.find(n => n.id == node.id).controls.get('preview').setValue(sum);

        outputs['num'] = sum;
    }

    created(node) {
        //console.log('created', node)
    }

    destroyed(node) {
        //console.log('destroyed', node)
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
            outputs: {answer1:'output', answer2:'output', answer3: 'output'}
        }
    }

    // Node's features
    builder(node) {
        var input1 = new Rete.Input('in1', "Situation", strSocket, false);
        var input2 = new Rete.Input('in2', "Situation", strSocket, false);
        var input3 = new Rete.Input('in3', "Situation", strSocket, false);


        var ctrl = new InputControl("text");


        //var ctrl1 = new TextControl(this.editor, 'text1');
        //var ctrl2 = new TextControl(this.editor, 'text2');
        //var ctrl3 = new TextControl(this.editor, 'text3');

        //ctrl1.onChange = () => {
        //    console.log(this)
        //}

        //input1.addControl(ctrl1);
        //input2.addControl(ctrl2);
        //input3.addControl(ctrl3);
        //input2.addControl(new InputControl('text2'));
        //input3.addControl(new InputControl('text3'));

        

        //var out = new Rete.Output("text", "Text", strSocket);
        //var ctrl = new InputControl("answer1");

        //var inp2 = new Rete.Input('num2', "Number", numSocket);
        var out1 = new Rete.Output('answer1', "Answer 1", strSocket, false);
        var out2 = new Rete.Output('answer2', "Answer 2", strSocket, false);
        var out3 = new Rete.Output('answer3', "Answer 3", strSocket, false);

        //out1.addControl(ctrl);

        //inp2.addControl(new TextControl(this.editor, 'num2', false, 'number'))

        return node
            .addControl(ctrl)
            .addInput(input1)
            .addInput(input2)
            .addInput(input3)
            .addOutput(out1)
            .addOutput(out2)
            .addOutput(out3);
    }

    // How the node process the data
    worker(node, inputs, outputs, {
        silent
    } = {}) {
        /*var n1 = inputs['num1'].length ? inputs['num1'][0] : node.data.num1;
        var n2 = inputs['num2'].length ? inputs['num2'][0] : node.data.num2;
        var sum = inputs['path'];


        if (!silent)
            this.editor.nodes.find(n => n.id == node.id).controls.get('preview').setValue("hi");*/

        outputs['answer1'] = "heeey";
    }

    created(node) {

        //adjustLayoutNodes();
        //console.log('dialogue extract created', node)
    }

    destroyed(node) {
        console.log('dialogue extract destroyed', node)
    }

}


class DialogueComponent extends Rete.Component {

    constructor() {
        super("Dialogue");
        this.module = {
            socket: strSocket
        }
        this.task = {
            outputs: {answer1:'option', answer2:'option', answer3: 'option'}
        }
    }

    builder(node) {
        var inp1 = new Rete.Input('path', "Situation", strSocket, false);
        //var inp2 = new Rete.Input('num2', "Number", numSocket);
        var out1 = new Rete.Output('answer1', "Answer 1", strSocket, false);
        var out2 = new Rete.Output('answer2', "Answer 2", strSocket, false);
        var out3 = new Rete.Output('answer3', "Answer 3", strSocket, false);

        inp1.addControl(new TextControl(this.editor, 'path', false, 'number'))
        //inp2.addControl(new TextControl(this.editor, 'num2', false, 'number'))
        //out1.addControl(new TextControl(this.editor, 'answer1', false, 'number'))

        return node
            .addInput(inp1)
            .addControl(new TextControl(this.editor, 'preview', true))
            .addOutput(out1)    
            .addOutput(out2)
            .addOutput(out3);
    }

    worker(node, inputs, outputs, {
        silent
    } = {}) {
        /*var n1 = inputs['num1'].length ? inputs['num1'][0] : node.data.num1;
        var n2 = inputs['num2'].length ? inputs['num2'][0] : node.data.num2;
        var sum = n1 + n2;

        if (!silent)
            this.editor.nodes.find(n => n.id == node.id).controls.get('preview').setValue(sum);*/

        outputs['answer1'] = "hi";
    }

    created(node) {
        //console.log('dialogue extract created', node)
        layoutnodes();
    }

    destroyed(node) {
       //console.log('dialogue extract destroyed', node)
    }

}

