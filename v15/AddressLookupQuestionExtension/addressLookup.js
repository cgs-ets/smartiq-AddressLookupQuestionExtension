$('#{{controlPrefix}}').on('keyup', delay(function (e) {
    // If already searcing don't trigger another search.
    if ($('#{{controlPrefix}}-wraper').hasClass('searching')) {
        return;
    }
    // If the input does not have any text no need to perform the lookup there will be no results.
    if (!$('#{{controlPrefix}}').val().length) {
        $('#{{controlPrefix}}-address-lookup-results').removeClass('has-results');
        return;
    }
    // The control is active.
    $('#{{controlPrefix}}-address-lookup-results').removeClass('inactive');
    $('#{{controlPrefix}}-wraper').addClass('searching');

    // Perform the lookup.
    var url = 'https://mappify.io/api/rpc/address/autocomplete/';
    var payload = {
        "streetAddress": $('#{{controlPrefix}}').val(),
        "formatCase": true,
        "apiKey": "{{apiKey}}"
    };
    var resultsHtml = "";
    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(payload),
        contentType: "application/json; charset=utf-8",
        crossDomain: true,
        dataType: "json",
        success: function (data, status, jqXHR) {
            $('#{{controlPrefix}}-wraper').removeClass('searching');
            // If the control has lost focus by the time results come back, do not continue.
            if (!$('#{{controlPrefix}}').is(":focus")) {
                return;
            }
            // Did we get any results?
            if (data.result.length) {
                $('#{{controlPrefix}}-address-lookup-results').addClass('has-results');
            } else {
                $('#{{controlPrefix}}-address-lookup-results').removeClass('has-results');
            }
            // Add the results to the lookup.
            data.result.forEach(address => {
                resultsHtml += '<li><a class="{{controlPrefix}}-streetaddress" href="#" data-json=\'' + JSON.stringify(address) + '\'>' + address.streetAddress + '</a></li>';
            });
            $('#{{controlPrefix}}-address-lookup-results').html(resultsHtml);
        },
        error: function (jqXHR, status) {
            $('#{{controlPrefix}}-wraper').removeClass('searching');
            console.log(jqXHR);
        }
    });
}, 300));

$('#{{controlPrefix}}').on('blur', function (e) {
    // Left the control, set to inactive.
    $('#{{controlPrefix}}-address-lookup-results').addClass('inactive');
    // Check for unchanged selected address. If so, set back to selected.
    if ($('#{{controlPrefix}}-tempaddress').val().length && $('#{{controlPrefix}}-tempaddress').val() == $('#{{controlPrefix}}').val()) {
        $('#{{controlPrefix}}-wrapper').addClass('selected');
        $('#{{controlPrefix}}-selected').val(1);
    }
});

$('#{{controlPrefix}}').on('focus', function (e) {
    // Re-entered the contol. Remove existing results and trigger a fresh lookup.
    $('#{{controlPrefix}}-address-lookup-results li').remove();
    $('#{{controlPrefix}}').trigger('keyup');
});

$('#{{controlPrefix}}-wrapper').on('mousedown', '.{{controlPrefix}}-streetaddress', function (e) {
    var selection = $(this);
    // Mark as selected.
    $('#{{controlPrefix}}-wrapper').addClass('selected');
    $('#{{controlPrefix}}-selected').val(1);
    // Set the input value to the full address.
    $('#{{controlPrefix}}').val(selection.data('json').streetAddress);
    $('#{{controlPrefix}}-tempaddress').val(selection.data('json').streetAddress);
    // Set sub fields.
    var addressline1 = selection.data('json').numberFirst + " " + selection.data('json').streetName + " " + selection.data('json').streetType;
    if (selection.data('json').flatNumber) {
        addressline1 = "UNIT " + selection.data('json').flatNumber + " " + addressline1;
    }
    $('#{{controlPrefix}}-addressline1').val(addressline1);
    $('#{{controlPrefix}}-suburb').val(selection.data('json').suburb);
    $('#{{controlPrefix}}-state').val(selection.data('json').state);
    $('#{{controlPrefix}}-country').val('Australia');
    $('#{{controlPrefix}}-postcode').val(selection.data('json').postCode);
});

$('#{{controlPrefix}}-changeaddress').on('click', function (e) {
    e.preventDefault();
    $('#{{controlPrefix}}-wrapper').removeClass('selected');
    $('#{{controlPrefix}}-selected').val(0);
    $('#{{controlPrefix}}').focus();
});

function delay(fn, ms) {
    let timer = 0
    return function (...args) {
        clearTimeout(timer)
        timer = setTimeout(fn.bind(this, ...args), ms || 0)
    }
}