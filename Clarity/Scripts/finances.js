/// <reference path="finances.dataAccess.js"/>

(function () {

    function Bill(dto) {
        var self = this;

        //persisted properties
        self.Id = dto.Id;
        self.Name = ko.observable(dto.Name);
        self.UserId = dto.UserId;
        self.Amount = ko.observable(dto.Amount);
        self.PaymentDay = ko.observable(dto.PaymentDay);
        self.IsFixed = ko.observable(dto.IsFixed);

        self.isEditingName = ko.observable(false);
        self.errorMessage = ko.observable();

        self.save = function () {
            self.errorMessage(null);
            //var t = { Name: "Test", UserId: 1 };
            //var tj = ko.toJSON(t);
            return Finances.Db.saveBill(self)
                .fail(function () {
                    var message = self.id ? "Error updating bill." : "Error adding bill.";
                    self.errorMessage(message);
                });
        };

        self.del = function () {
            return Finances.Db.deleteBill(self.id)
                .fail(function () { self.errorMessage("Error removing bill."); });
        };

        // Auto-save when these properties change
        self.Name.subscribe(self.save);
    }

    function FinancesViewModel() {
        var self = this;
        self.bills = ko.observableArray();
        self.error = ko.observable();

        // Load initial state from server, convert it to Bill instances, then populate self.bills
        Finances.Db.getBills()
            .done(function (allData) {
                var mappedBills = $.map(allData, function (item) { return new Bill(item); });
                self.bills(mappedBills);
            })
            .fail(function () {
                self.error("Error retrieving bills.");
            });

        // Operations for the Bills
        self.addBill = function () {
            var bill = new Bill({ Name: "Bill 1", PaymentDay: 1 });  //, PaymentDay: 1, Amount: 0, IsFixed: false
            self.bills.unshift(bill); // Inserts on client a new item at the beginning of the array
            bill.save();                  // Inserts on server
            bill.isEditingName(true);
        };

        self.deleteBill = function (bill) {
            bill.del() // Deletes on server
                .done(function () { self.bills.remove(bill); }); // Deletes on client
        };
    }

    // Initiate the Knockout bindings
    ko.applyBindings(new FinancesViewModel());
})();