module.exports = Cab;

function Cab(provider, type, eta, distance, capacity, image, fare) {
    this.provider = provider;
    this.type = type;
    this.eta = eta;
    this.capacity = capacity;
    this.distance = distance;
    this.image = image;
    this.fare = fare;
}