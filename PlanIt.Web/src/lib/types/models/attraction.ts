export interface Attraction {
	id: string;
	scheduleId: string;
	name: string;
	description: string;
	imageUrl: string;
	capacity: number;
	remainingCapacity: number;
	hasJoined?: boolean;
}
