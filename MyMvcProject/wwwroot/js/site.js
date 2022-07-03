function PagerClick(index) {
    document.getElementById("hfPageIndex").value = index;
    document.forms[0].submit();
}

function setSrc() {
    let img = document.getElementById("imgControl");
    const [inputValue] = document.getElementById("inputControl").files;
    if (inputValue) {
        img.src = URL.createObjectURL(inputValue);
    }
}


