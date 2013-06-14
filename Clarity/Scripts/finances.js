/// <reference path="finances.dataAccess.js"/>
/// <reference path="clarity.common.js"/>

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

        self.isEditing = ko.observable(false);
        self.errorMessage = ko.observable();

        self.original = null;

        self.copyTo = function(bill){
            bill.Name(self.Name());
            bill.UserId = self.UserId;
            bill.Amount(self.Amount());
            bill.PaymentDay(self.PaymentDay());
            bill.IsFixed(self.IsFixed);
        };

        self.save = function () {
            self.errorMessage(null);
            //var t = { Name: "Test", UserId: 1 };
            //var tj = ko.toJSON(t);
            return Finances.Db.saveBill(self)
                .fail(function () {
                    var message = self.id ? "Error updating bill." : "Error adding bill.";
                    self.errorMessage(message);
                });

            self.isEditing(false);
        };

        self.del = function () {
            confirmAction('Delete Bill', 'Are you sure you want to delete this bill?', self.doDelete);            
        };

        self.doDelete = function(){
            return Finances.Db.deleteBill(self.id)
                .fail(function () { self.errorMessage("Error removing bill."); });
        };

        self.edit = function () {
            self.original = new Bill();
            self.copyTo(self.original);

            self.isEditing(true);
        };

        self.cancelEdit = function () {
            self.original.copyTo(self);
            self.original = null;

            self.isEditing(false);
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
            bill.isEditing(true);
        };

        self.deleteBill = function (bill) {
            bill.del() // Deletes on server
                .done(function () { self.bills.remove(bill); }); // Deletes on client
        };
    }

    // Initiate the Knockout bindings
    ko.applyBindings(new FinancesViewModel());
})();