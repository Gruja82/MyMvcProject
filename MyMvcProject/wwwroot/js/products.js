////////////    Create View     ///////////////

// Counter variable
var counter = 0;

// Create an array to hold material names added to list
var materialNames = [];

// Declare variable that will hold total value of order
var total = 0;

// Function for adding rows
function addProductRow() {
    // Create span element
    var span = $('<span id="span' + counter + '"></span>');

    // Declare variable that holds selected material from drop down list
    var material = $('#lstMaterials option:selected').text();

    // Find index of selected material
    var materialIndex = $('#lstMaterials option:selected').index();

    // Declare variable that holds material quantity
    var quantity = $('#inputQuantity').val();

    // Create input element which holds material name
    var inputMaterial = $('<input class="form-control" id="material' + counter + '" readonly name="ProductDetailsList[' + counter + '].Material" value="' + material + '"/>');

    // Create input element which holds material quantity
    var inputQuantity = $('<input id="quantity' + counter + '" class="form-control" readonly name="ProductDetailsList[' + counter + '].Quantity" value="' + quantity + '"/>');

    // Create button element for deleting current span
    var deleteButton = $('<input type="button" class="btn btn-danger rounded" value="Remove" onclick="removeProductRow(' + counter + ');" />');

    if ((jQuery.inArray(inputMaterial.val(), materialNames)) == -1) {
        if (quantity > 0) {
            // Append inputMaterial, inputQuantity and deleteButton to span
            span.append(inputMaterial);
            span.append(inputQuantity);
            span.append(deleteButton);

            // Append span to div
            $('#productDetails').append(span);
            $('#productDetails').append('<hr id="hr' + counter + '" />');

            // Add material to array
            materialNames.push(inputMaterial.val());

            // Variable that represents lstPrice element
            var price = document.getElementById("lstPrice");

            // Increment total variable
            total += quantity * price[materialIndex].value;

            // Round total value to 2 decimals
            var totalFixed = total.toFixed(2);

            // Append totalFixed value to inputTotal element
            document.getElementById("inputTotal").value = totalFixed;

            // Increase counter
            counter++;
        }
        else {
            alert('Material quantity must be greater than 0!');
        }
    }
    else {
        alert('Material ' + '"' + inputMaterial.val() + '" is already added!')
    }
}

// Function for removing rows
function removeProductRow(index) {
    var removeItem = $('#material' + index).val();

    // Declare variable that will hold Material name
    var materialName = document.getElementById('material' + index).value;

    // Variable that represents lstMaterials drop down list
    var materialList = document.getElementById('lstMaterials');

    // Variable that will hold the index of certain material in lstMaterials
    var materialIndex;

    // Iterate through lstMaterials and fetch index of option element whose value is equal to materialName
    for (var i = 0; i < materialList.length; i++) {
        if (materialList.options[i].value == materialName) {
            materialIndex = i;
        }
    }

    // Variable that will hold price for certain material
    var price = document.getElementById('lstPrice')[materialIndex].value;

    // Variable that represents input element which contains quantity of certain material
    var inputQuantity = document.getElementById('quantity' + index).value;

    // Reduce total variable
    total -= price * inputQuantity;

    // Round total value to 2 decimals
    var totalFixed = total.toFixed(2);

    // Append totalFixed value to inputTotal element
    document.getElementById("inputTotal").value = totalFixed;

    // Remove current span element
    $('#span' + index).remove();
    $('#hr' + index).remove();
    counter--;

    materialNames = jQuery.grep(materialNames, function (value) {
        return value != removeItem;
    });
}

////////////    Create View End     ///////////////

////////////    Edit View       //////////////

var materials = [];

var quantities = [];

var editCounter = 0;

function populateProduct(material, quantity, myTotal) {
    populateProductRows(material, quantity);
    populateProductTotal(myTotal);
}

// Function for adding materials to materials array that are already contained in model
function addProductMaterials(myMaterial) {
    materials.push(myMaterial);
}

function addProductQuantities(myQuantity) {
    quantities.push(myQuantity);
}

// Function for creating elements that represent currently present values in model
function populateProductRows(material, quantity) {
    addProductMaterials(material);
    addProductQuantities(quantity);

    // Create span element
    var span = $('<span id="span' + editCounter + '"></span>');

    // Create input element which holds material name
    var inputMaterial = $('<input class="form-control" id="material' + editCounter + '" readonly name="ProductDetailsList[' + editCounter + '].Material" value="' + materials[editCounter] + '"/>');

    // Create input element which holds material quantity
    var inputQuantity = $('<input id="quantity' + editCounter + '" class="form-control" readonly name="ProductDetailsList[' + editCounter + '].Quantity" value="' + quantities[editCounter] + '"/>');

    // Create button element for deleting current span
    var deleteButton = $('<input type="button" class="btn btn-danger rounded" value="Remove" onclick="removeProductEditRow(' + editCounter + ');" />');

    // Append inputMaterial, inputQuantity and deleteButton to span
    span.append(inputMaterial);
    span.append(inputQuantity);
    span.append(deleteButton);

    // Append span to div
    $('#productDetails').append(span);
    $('#productDetails').append('<hr id="hr' + editCounter + '" />');

    // Append material names to materialNames
    materialNames.push(inputMaterial.val());

    // Increase editCounter
    editCounter++;
}

// Function for adding new rows
function addProductEditRow() {
    // Create span element
    var span = $('<span id="span' + editCounter + '"></span>');

    // Declare variable that holds selected material from drop down list
    var material = $('#lstMaterials option:selected').text();

    // Find index of selected material
    var materialIndex = $('#lstMaterials option:selected').index();

    // Declare variable that holds material quantity
    var quantity = $('#inputQuantity').val();

    // Create input element which holds material name
    var inputMaterial = $('<input class="form-control" id="material' + editCounter + '" readonly name="ProductDetailsList[' + editCounter + '].Material" value="' + material + '"/>');

    // Create input element which holds material quantity
    var inputQuantity = $('<input id="quantity' + editCounter + '" class="form-control" readonly name="ProductDetailsList[' + editCounter + '].Quantity" value="' + quantity + '"/>');

    // Create button element for deleting current span
    var deleteButton = $('<input type="button" class="btn btn-danger rounded" value="Remove" onclick="removeProductEditRow(' + editCounter + ');" />');

    if ((jQuery.inArray(inputMaterial.val(), materialNames)) == -1) {
        if (quantity > 0) {
            // Append inputMaterial, inputQuantity and deleteButton to span
            span.append(inputMaterial);
            span.append(inputQuantity);
            span.append(deleteButton);

            // Append span to div
            $('#productDetails').append(span);
            $('#productDetails').append('<hr id="hr' + editCounter + '" />');

            // Add material to array
            materialNames.push(inputMaterial.val());

            // Variable that represents lstPrice element
            var price = document.getElementById("lstPrice");

            // Increment total variable
            total += quantity * price[materialIndex].value;

            // Round total value to 2 decimals
            var totalFixed = total.toFixed(2);

            // Append totalFixed value to inputTotal element
            document.getElementById("inputTotal").value = totalFixed;

            // Increase editCounter
            editCounter++;
        }
        else {
            alert('Material quantity must be greater than 0!');
        }
    }
    else {
        alert('Material ' + '"' + inputMaterial.val() + '" is already added!')
    }
}

// Function for removing rows
function removeProductEditRow(index) {
    var removeItem = $('#material' + index).val();

    // Declare variable that will hold Material name
    var materialName = document.getElementById('material' + index).value;

    // Variable that represents lstMaterials drop down list
    var materialList = document.getElementById('lstMaterials');

    // Variable that will hold the index of certain material in lstMaterials
    var materialIndex;

    // Iterate through lstMaterials and fetch index of option element whose value is equal to materialName
    for (var i = 0; i < materialList.length; i++) {
        if (materialList.options[i].value == materialName) {
            materialIndex = i;
        }
    }

    // Variable that will hold price for certain material
    var price = document.getElementById('lstPrice')[materialIndex].value;

    // Variable that represents input element which contains quantity of certain material
    var inputQuantity = document.getElementById('quantity' + index).value;

    // Reduce total variable
    total -= price * inputQuantity;

    // Round total value to 2 decimals
    var totalFixed = total.toFixed(2);

    // Append totalFixed value to inputTotal element
    document.getElementById("inputTotal").value = totalFixed;

    // Remove current span element
    $('#span' + index).remove();
    $('#hr' + index).remove();
    editCounter--;

    materialNames = jQuery.grep(materialNames, function (value) {
        return value != removeItem;
    });
}

// Function to populate total variable
function populateProductTotal(totalValue) {
    total = totalValue;
    var totalFixed = total.toFixed(2);

    // Append totalFixed value to inputTotal element
    document.getElementById("inputTotal").value = totalFixed;
}

////////////    Edit View End      //////////////