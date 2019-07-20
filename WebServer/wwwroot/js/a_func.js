//States for the control
const STATES = {
    BeforeStart: 'bstart',
    STARTED: 'started'
}
var state = STATES.BeforeStart;

//List of Buttons
var B_list = document.createElement("ol");
B_list.style.display = 'none';
B_list.id = 'MainOL'

//Show status for the client
var state_stringElement = document.getElementById("status");
var updateState = (newState) => {
    document.getElementById('status').textContent='NotConnected\n'
};
state_stringElement.textContent = "Not Connected\n";

//function to append Li to the ordered List
var socket;

document.getElementById('connectIP').value = "192.168.1.136";

var Bucle = (txt) => {
    switch (state) {
        case STATES.BeforeStart:
            updateState('Not Connected');
            if (txt == 'connect') {
                try {

                    DisableClickedButton(document.getElementById("ConnectButton"));

                    //Communication
                    let IP = document.getElementById('connectIP').value;
                    socket = new WebSocket('ws://' + IP+'/ws');

                    //Status for the client
                    updateState("Connecting...\n");

                    socket.addEventListener('open', function (event) {
                        socket.send('Start');
                        console.log("Connected to server");
                        state_stringElement.textContent = "Connected";
                        appendChildren(B_list, createButtons())
                        B_list.style.display = 'block';
                        document.getElementById('Second_OL').appendChild(B_list);
                    });
                    var restore2BeforeStart = (reason) => {
                        state = STATES.BeforeStart;
                        updateState("Not Connected:"+reason+'\n');
                        document.getElementById("ConnectButton").disabled = false;
                        //hide the buttons
                        document.getElementById('Second_OL').style.display = 'none';
                    }
                    socket.addEventListener('error', (event) => {
                        EnableClickedButton(document.getElementById("ConnectButton"));
                        restore2BeforeStart('Error in connection')
                    });
                    socket.addEventListener('close', (ev) => {
                        restore2BeforeStart('Connection Closed')
                    });
                    
                    //next State
                    state = STATES.STARTED;
                } catch (error) {
                    console.log(error);
                }
            }
            break;
        case STATES.STARTED:

            socket.send(txt);

            state = STATES.STARTED;
            break;
        default:
            break;
    }
}

Bucle();

var DisableClickedButton = (button) => {
    button.disabled = true;
    button.children[1].textContent = "Connecting...";
    var Loading = button.children[0];//span
    Loading.className = "spinner-border spinner-border-sm";
}
var EnableClickedButton = (button) => {
    button.disabled = false;
    button.children[1].textContent = "Connect:";
    var Loading = button.children[0];
    Loading.className = "";
}