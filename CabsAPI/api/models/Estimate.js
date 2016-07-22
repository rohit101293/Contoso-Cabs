module.exports = Estimate;

function Estimate(distance, lowRange, highRange, time, fare) {
	this.highRange = highRange;
	this.distance = distance;
	this.lowRange = lowRange;
	this.time = time;
	this.fare = fare;
}