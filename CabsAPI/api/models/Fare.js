module.exports = Fare;

function Fare(baseFare, costPerKilometer, costPerMinute, surcharge) {
    this.baseFare = baseFare;
    this.costPerKilometer = costPerKilometer;
    this.costPerMinute = costPerMinute;
    this.surcharge = surcharge;
}