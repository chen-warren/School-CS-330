let text = "The Dick Van Dyke Show..."; 
let element = document.getElementById('Footer'); 

function typeWriter() {
    let i = 0;
    element.innerHTML = ''; 

    function type() {
        if (i < text.length) {
            element.innerHTML += text[i];
            if (text[i] === ' ') 
                setTimeout(type, 10);
            else 
                setTimeout(type, 100);
            i++; 
        } else {
            setTimeout(typeWriter, 10000);
        }
    }
    type(); 
}

typeWriter();