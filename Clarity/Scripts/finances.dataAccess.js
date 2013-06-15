

(function() {
	
	//Establish the namespace and make sure we're only defining this once
	window.Finances = window.Finances || {};

	    // Private: Routes
    var billsUrl = function (id) { return "/api/bills/" + (id || "") },
        paymentsUrl = function (id) { return "/api/payments/" + (id || "") };

    // Private: Ajax helper
    function ajaxRequest(type, url, data) {
        var options = { dataType: "json", contentType: "application/json", cache: false, type: type, data: ko.toJSON(data) }
        return $.ajax(url, options);
    }

    // Public: Db methods
    window.Finances.Db = {

        getBills: function () {
            return ajaxRequest("get", billsUrl());
        },

        saveBill: function (bill) {
            if (bill.Id) {
                // Update
                return ajaxRequest("put", billsUrl(bill.Id), bill);
            } else {
                // Create
                return ajaxRequest("post", billsUrl(), bill)
                    .done(function (result) {
                        bill.Id = result.Id;
                        bill.UserId = result.UserId;
                    });
            }
        },

        deleteBill: function (billId) {
            return ajaxRequest("delete", billsUrl(billId));
        },

        savePayment: function (payment) {
            if (payment.id) {
                // Update
                return ajaxRequest("put", paymentsUrl(payment.id), payment);
            } else {
                // Create
                return ajaxRequest("post", paymentUrl(), payment)
                    .done(function (result) {
                        payment.id = result.Id;
                    });
            }
        },

        deletePayment: function (paymentId) {
            return ajaxRequest("delete", paymentUrl(paymentId));
        }
    };

})();