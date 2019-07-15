var ButtonListNames = [
    "Search",
    "Next Result",
    "Click",
    "Previous Result",
    "Center",
    "Play/Pause",
    "Next Video",
    "Full Screen",
    "Close Advertisement",
    "Previous Page",
    "Next Page",
    "Close IE"
];
//Function for adding an array of elements to a list 
//(in this case) ordered list
var appendChildren = (list, Elements2Put) => {
    let ar_listElement = [];
    //fill with array

    let max = Elements2Put.length;

    for (let i = 0; i < max; i++) {
        ar_listElement[i] = document.createElement('li');
        ar_listElement[i].appendChild(Elements2Put[i]);
    }
    ar_listElement.forEach(element => {
        list.appendChild(element);
    });
}

var createButtons = () => {
    let max = ButtonListNames.length;
    let ElementsArray = [];
    for (let index = 0; index < max; index++) {

        let button = document.createElement('button');
        if (ButtonListNames[index] == "Search") {
            let i = index;
            let temp_button = button;
            temp_button.textContent = ButtonListNames[i];
            temp_button.id = ButtonListNames[i];
            temp_button.addEventListener('click', function () { Bucle('Search;' + document.getElementById('toSearchSTR').value + ';'); });
            button = document.createElement('ul');
            let text = document.createElement('textarea');
            text.id = 'toSearchSTR';

            button.appendChild(text);
            button.appendChild(temp_button);
            ElementsArray[i] = button;
        }
        else {
            let i = index;
            button.onclick = () => {
                Bucle(ButtonListNames[i].toString());
            };
            button.textContent = ButtonListNames[i];
            ElementsArray[i] = button;
        }

    }
    return ElementsArray;
};
