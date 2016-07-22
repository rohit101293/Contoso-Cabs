module.exports = CabEstimate;

function CabEstimate(provider, type, eta, distance, capacity, image, estimate) {
	this.provider = provider;
	this.type = type;
	this.eta = eta;
	this.capacity = capacity;
	this.distance = distance;
	this.image = image;
	this.estimate = estimate;
}