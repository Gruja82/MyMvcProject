////////////    Create View     ///////////////

// Counter variable
var counter = 0;

// Create an array to hold product names added to list
var productNames = [];

// Function for adding rows
function addProductionRow() {
    // Create span element
    var span = $('<span id="span' + counter + '"></span>');

    // Declare variable that holds selected product from drop down list
    var product = $('#lstProducts option:selected').text();

    // Find index of selected product
    var productIndex = $('#lstProducts option:selected').index();

    // Declare variable that holds product quantity
    var quantity = $('#inputQuantity').val();

    // Create input element which holds product name
    var inputProduct = $('<input class="form-control" id="product' + counter + '" readonly name="ProductionDetailsList[' + counter + '].Product" value="' + product + '"/>');

    // Create input element which holds product quantity
    var inputQuantity = $('<input id="quantity' + counter + '" class="form-control" readonly name="ProductionDetailsList[' + counter + '].Quantity" value="' + quantity + '"/>');

    // Create button element for deleting current span
    var deleteButton = $('<input type="button" class="btn btn-danger rounded" value="Remove" onclick="removeProductionRow(' + counter + ');" />');

    if ((jQuery.inArray(inputProduct.val(), productNames)) == -1) {
        if (quantity > 0) {
            // Append inputProduct, inputQuantity and deleteButton to span
            span.append(inputProduct);
            span.append(inputQuantity);
            span.append(deleteButton);

            // Append span to div
            $('#productionDetails').append(span);
            $('#productionDetails').append('<hr id="hr' + counter + '" />');

            // Add product to array
            productNames.push(inputProduct.val());

            // Increase counter
            counter++;
        }
        else {
            alert('Product quantity must be greater than 0!');
        }
    }
    else {
        alert('Product ' + '"' + inputProduct.val() + '" is already added!')
    }
}

// Function for removing rows
function removeProductionRow(index) {
    var removeItem = $('#product' + index).val();

    // Declare variable that will hold Product name
    var productName = document.getElementById('product' + index).value;

    // Variable that represents lstProducts drop down list
    var productList = document.getElementById('lstProducts');

    // Variable that will hold the index of certain product in lstProducts
    var productIndex;

    // Iterate through lstProducts and fetch index of option element whose value is equal to productName
    for (var i = 0; i < productList.length; i++) {
        if (productList.options[i].value == productName) {
            productIndex = i;
        }
    }

    // Remove current span element
    $('#span' + index).remove();
    $('#hr' + index).remove();
    counter--;

    productNames = jQuery.grep(productNames, function (value) {
        return value != removeItem;
    });
}

////////////    Create View End     ///////////////

////////////    Edit View       //////////////

var products = [];

var quantities = [];

var editCounter = 0;

function populateProduction(product, quantity) {
    populateOrderRows(product, quantity);
}

// Function for adding products to products array that are already contained in model
function addProductionProducts(myProduct) {
    products.push(myProduct);
}

function addProductionQuantities(myQuantity) {
    quantities.push(myQuantity);
}

// Function for creating elements that represent currently present values in model
function populateProductionRows(product, quantity) {
    addProductionProducts(product);
    addProductionQuantities(quantity);

    // Create span element
    var span = $('<span id="span' + editCounter + '"></span>');

    // Create input element which holds product name
    var inputProduct = $('<input class="form-control" id="product' + editCounter + '" readonly name="ProductionDetailsList[' + editCounter + '].Product" value="' + products[editCounter] + '"/>');

    // Create input element which holds product quantity
    var inputQuantity = $('<input id="quantity' + editCounter + '" class="form-control" readonly name="ProductionDetailsList[' + editCounter + '].Quantity" value="' + quantities[editCounter] + '"/>');

    // Create button element for deleting current span
    var deleteButton = $('<input type="button" class="btn btn-danger rounded" value="Remove" onclick="removeProductionEditRow(' + editCounter + ');" />');

    // Append inputProduct, inputQuantity and deleteButton to span
    span.append(inputProduct);
    span.append(inputQuantity);
    span.append(deleteButton);

    // Append span to div
    $('#productionDetails').append(span);
    $('#productionDetails').append('<hr id="hr' + editCounter + '" />');

    // Append product names to productNames
    productNames.push(inputProduct.val());

    // Increase editCounter
    editCounter++;
}

// Function for adding new rows
function addProductionEditRow() {
    // Create span element
    var span = $('<span id="span' + editCounter + '"></span>');

    // Declare variable that holds selected product from drop down list
    var product = $('#lstProducts option:selected').text();

    // Find index of selected product
    var productIndex = $('#lstProducts option:selected').index();

    // Declare variable that holds product quantity
    var quantity = $('#inputQuantity').val();

    // Create input element which holds product name
    var inputProduct = $('<input class="form-control" id="product' + editCounter + '" readonly name="ProductionDetailsList[' + editCounter + '].Product" value="' + product + '"/>');

    // Create input element which holds product quantity
    var inputQuantity = $('<input id="quantity' + editCounter + '" class="form-control" readonly name="ProductionDetailsList[' + editCounter + '].Quantity" value="' + quantity + '"/>');

    // Create button element for deleting current span
    var deleteButton = $('<input type="button" class="btn btn-danger rounded" value="Remove" onclick="removeProductionEditRow(' + editCounter + ');" />');

    if ((jQuery.inArray(inputProduct.val(), productNames)) == -1) {
        if (quantity > 0) {
            // Append inputProduct, inputQuantity and deleteButton to span
            span.append(inputProduct);
            span.append(inputQuantity);
            span.append(deleteButton);

            // Append span to div
            $('#productionDetails').append(span);
            $('#productionDetails').append('<hr id="hr' + editCounter + '" />');

            // Add product to array
            productNames.push(inputProduct.val());

            // Increase editCounter
            editCounter++;
        }
        else {
            alert('Product quantity must be greater than 0!');
        }
    }
    else {
        alert('Product ' + '"' + inputProduct.val() + '" is already added!')
    }
}

// Function for removing rows
function removeProductionEditRow(index) {
    var removeItem = $('#product' + index).val();

    // Declare variable that will hold Product name
    var productName = document.getElementById('product' + index).value;

    // Variable that represents lstProducts drop down list
    var productList = document.getElementById('lstProducts');

    // Variable that will hold the index of certain product in lstProducts
    var productIndex;

    // Iterate through lstProducts and fetch index of option element whose value is equal to productName
    for (var i = 0; i < productList.length; i++) {
        if (productList.options[i].value == productName) {
            productIndex = i;
        }
    }

    // Remove current span element
    $('#span' + index).remove();
    $('#hr' + index).remove();
    editCounter--;

    productNames = jQuery.grep(productNames, function (value) {
        return value != removeItem;
    });
}

////////////    Edit View End      //////////////