function GetCoordinatesLetter(element, i) {
    const spanList = document.querySelectorAll(".words-area span")
    const currentSpan =  spanList[i]
    return {
        top: Math.round(currentSpan.offsetTop),
        left: Math.round(currentSpan.offsetLeft),
        width : Math.round(currentSpan.offsetWidth)
    };
}

