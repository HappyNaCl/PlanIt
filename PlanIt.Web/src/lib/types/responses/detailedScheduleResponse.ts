import type { Attraction } from "../models/attraction";

export interface DetailedScheduleResponse {
	id: string;
	name: string;
	description: string;
	location: string;
	startTime: string;
	endTime: string;
	attractions: Attraction[];
}
