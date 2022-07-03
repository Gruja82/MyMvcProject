////////////    Create View     ///////////////

// Counter variable
var counter = 0;

// Create an array to hold product names added to list
var productNames = [];

// Declare variable that will hold total value of order
var total = 0;

// Function for adding rows
function addOrderRow() {
    // Create span element
    var span = $('<span id="span' + counter + '"></span>');

    // Declare variable that holds selected product from drop down list
    var product = $('#lstProducts option:selected').text();

    // Find index of selected product
    var productIndex = $('#lstProducts option:selected').index();

    // Declare variable that holds product quantity
    var quantity = $('#inputQuantity').val();

    // Create input element which holds product name
    var inputProduct = $('<input class="form-control" id="product' + counter + '" readonly name="OrderDetailList[' + counter + '].Product" value="' + product + '"/>');

    // Create input element which holds product quantity
    var inputQuantity = $('<input id="quantity' + counter + '" class="form-control" readonly name="OrderDetailList[' + counter + '].Quantity" value="' + quantity + '"/>');

    // Create button element for deleting current span
    var deleteButton = $('<input type="button" class="btn btn-danger rounded" value="Remove" onclick="removeOrderRow(' + counter + ');" />');

    if ((jQuery.inArray(inputProduct.val(), productNames)) == -1) {
        if (quantity > 0) {
            // Append inputProduct, inputQuantity and deleteButton to span
            span.append(inputProduct);
            span.append(inputQuantity);
            span.append(deleteButton);

            // Append span to div
            $('#orderDetails').append(span);
            $('#orderDetails').append('<hr id="hr' + counter + '" />');

            // Add product to array
            productNames.push(inputProduct.val());

            // Variable that represents lstPrice element
            var price = document.getElementById("lstPrice");

            // Increment total variable
            total += quantity * price[productIndex].value;

            // Round total value to 2 decimals
            var totalFixed = total.toFixed(2);

            // Append totalFixed value to inputTotal element
            document.getElementById("inputTotal").value = totalFixed;

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
function removeOrderRow(index) {
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

    // Variable that will hold price for certain product
    var price = document.getElementById('lstPrice')[productIndex].value;

    // Variable that represents input element which contains quantity of certain product
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

    productNames = jQuery.grep(productNames, function (value) {
        return value != removeItem;
    });
}

////////////    Create View End     ///////////////

////////////    Edit View       //////////////

var products = [];

var quantities = [];

var editCounter = 0;

function populateOrder(product, quantity, myTotal) {
    populateOrderRows(product, quantity);
    populateOrderTotal(myTotal);
}

// Function for adding products to products array that are already contained in model
function addOrderProducts(myProduct) {
    products.push(myProduct);
}

function addOrderQuantities(myQuantity) {
    quantities.push(myQuantity);
}

// Function for creating elements that represent currently present values in model
function populateOrderRows(product, quantity) {
    addOrderProducts(product);
    addOrderQuantities(quantity);

    // Create span element
    var span = $('<span id="span' + editCounter + '"></span>');

    // Create input element which holds product name
    var inputProduct = $('<input class="form-control" id="product' + editCounter + '" readonly name="OrderDetailList[' + editCounter + '].Product" value="' + products[editCounter] + '"/>');

    // Create input element which holds product quantity
    var inputQuantity = $('<input id="quantity' + editCounter + '" class="form-control" readonly name="OrderDetailList[' + editCounter + '].Quantity" value="' + quantities[editCounter] + '"/>');

    // Create button element for deleting current span
    var deleteButton = $('<input type="button" class="btn btn-danger rounded" value="Remove" onclick="removeOrderEditRow(' + editCounter + ');" />');

    // Append inputProduct, inputQuantity and deleteButton to span
    span.append(inputProduct);
    span.append(inputQuantity);
    span.append(deleteButton);

    // Append span to div
    $('#orderDetails').append(span);
    $('#orderDetails').append('<hr id="hr' + editCounter + '" />');

    // Append product names to productNames
    productNames.push(inputProduct.val());

    // Increase editCounter
    editCounter++;
}

// Function for adding new rows
function addOrderEditRow() {
    // Create span element
    var span = $('<span id="span' + editCounter + '"></span>');

    // Declare variable that holds selected product from drop down list
    var product = $('#lstProducts option:selected').text();

    // Find index of selected product
    var productIndex = $('#lstProducts option:selected').index();

    // Declare variable that holds product quantity
    var quantity = $('#inputQuantity').val();

    // Create input element which holds product name
    var inputProduct = $('<input class="form-control" id="product' + editCounter + '" readonly name="OrderDetailList[' + editCounter + '].Product" value="' + product + '"/>');

    // Create input element which holds product quantity
    var inputQuantity = $('<input id="quantity' + editCounter + '" class="form-control" readonly name="OrderDetailList[' + editCounter + '].Quantity" value="' + quantity + '"/>');

    // Create button element for deleting current span
    var deleteButton = $('<input type="button" class="btn btn-danger rounded" value="Remove" onclick="removeOrderEditRow(' + editCounter + ');" />');

    if ((jQuery.inArray(inputProduct.val(), productNames)) == -1) {
        if (quantity > 0) {
            // Append inputProduct, inputQuantity and deleteButton to span
            span.append(inputProduct);
            span.append(inputQuantity);
            span.append(deleteButton);

            // Append span to div
            $('#orderDetails').append(span);
            $('#orderDetails').append('<hr id="hr' + editCounter + '" />');

            // Add product to array
            productNames.push(inputProduct.val());

            // Variable that represents lstPrice element
            var price = document.getElementById("lstPrice");

            // Increment total variable
            total += quantity * price[productIndex].value;

            // Round total value to 2 decimals
            var totalFixed = total.toFixed(2);

            // Append totalFixed value to inputTotal element
            document.getElementById("inputTotal").value = totalFixed;

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
function removeOrderEditRow(index) {
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

    // Variable that will hold price for certain product
    var price = document.getElementById('lstPrice')[productIndex].value;

    // Variable that represents input element which contains quantity of certain product
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

    productNames = jQuery.grep(productNames, function (value) {
        return value != removeItem;
    });
}


// Function to populate total variable
function populateOrderTotal(totalValue) {
    total = totalValue;
    var totalFixed = total.toFixed(2);

    // Append totalFixed value to inputTotal element
    document.getElementById("inputTotal").value = totalFixed;
}

////////////    Edit View End      //////////////