var dropdown = document.getElementById("dropdown")
var refButton = document.querySelector("#refreshButton");
var tableXml = document.querySelector("#scriptsXML");

function displayCategories(categories){
    console.log(categories);
    var scr = categories;
    var opt1 = document.createElement('option');
    opt1.value = "All";
    opt1.innerHTML = "All";
    dropdown.appendChild(opt1);
    scr.forEach(element => {
        var opt = document.createElement('option');
        opt.value = element.name;
        opt.innerHTML = element.name;
        dropdown.appendChild(opt);
    }); 
}

function displayScripts(scripts){
    const obj = JSON.parse(scripts);

    for(let i = 0; i < obj.length; i++) {
        var row = tableXml.insertRow();
        var cell = row.insertCell();
        var cell2 = row.insertCell();
        cell.innerHTML = obj[i].script;
        cell2.innerHTML = obj[i].price;
    }
}
function viewAllScripts(){
    fetch('https://localhost:7128/api/ScriptController').then(response => {
        response.json().then((data) => {
            displayCategories(data);
          })
        }
      )
}

function generateTableHead(table) {
    table.innerHTML = "";

    let thead = table.createTHead();
    let row = thead.insertRow();
    
    let th = document.createElement("th");
    let text = document.createTextNode("Script");
    th.appendChild(text);
    row.appendChild(th);
    
    let th2 = document.createElement("th");
    let text2 = document.createTextNode("Price");
    th2.appendChild(text2);
    row.appendChild(th2);
}

function parseScripts(valueCat){
    var body = {
        num: 1,
        name: valueCat
    }

    fetch('https://localhost:7128/api/ScriptController/specific', {
        method: 'POST',
        body: JSON.stringify(body),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        }
    }).then(response=> response.json())
    .then(response=>{
        generateTableHead(tableXml);
        displayScripts(response);
    })
}

document.onload = viewAllScripts();     //as soon as the page loads, the categories will be added in the dropdown

refButton.addEventListener('click', function(){
    parseScripts(dropdown.value);
});