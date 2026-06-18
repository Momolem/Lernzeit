import type { TimetableEvents } from "~/types/timetable";

const BACKEND_URL = import.meta.env.REACT_APP_BACKEND_URL ?? "https://localhost:7113";

export const apiClient = {
    async loginRaumzeit(username: string, password: string): Promise<boolean> {
        try {
            const response = await fetch(`${BACKEND_URL}/api/user/raumzeit/login`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include",
                body: JSON.stringify({ username, password }),
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Login failed: ${response.status} ${errorText}`);
            }

            return true;
        } catch (error) {
            console.error("Raumzeit login error:", error);
            throw error;
        }
    },

    async getCalendar(): Promise<{ status: number; data: TimetableEvents | null }> {
        try {
            const response = await fetch(`${BACKEND_URL}/api/user/calendar`, {
                credentials: "include",
            });

            if (!response.ok) {
                return { status: response.status, data: null };
            }

            const rawData = await response.json();
            
            const parsedData: TimetableEvents = {
                monday: [],
                tuesday: [],
                wednesday: [],
                thursday: [],
                friday: [],
                saturday: [],
                sunday: []
            };

            const dayMap: Record<number, keyof TimetableEvents> = {
                0: "sunday",
                1: "monday",
                2: "tuesday",
                3: "wednesday",
                4: "thursday",
                5: "friday",
                6: "saturday",
            };

            // Backend returns a Calendar object with an 'events' array
            const eventList = Array.isArray(rawData) ? rawData : (Array.isArray(rawData?.events) ? rawData.events : []);

            for (const [index, item] of eventList.entries()) {
                const startTime = new Date(item.start || item.startTime);
                const endTime = new Date(item.end || item.endTime);
                
                if (isNaN(startTime.getTime()) || isNaN(endTime.getTime())) continue;

                const dayOfWeek = startTime.getDay();
                const dayKey = dayMap[dayOfWeek];

                if (dayKey) {
                    parsedData[dayKey].push({
                        id: item.id || `event-${index}`,
                        name: item.name || "Unnamed Event",
                        type: item.type,
                        startTime,
                        endTime,
                        room: item.room
                    });
                }
            }

            return { status: response.status, data: parsedData };
        } catch (error) {
            console.error("Calendar fetch error:", error);
            throw error;
        }
    },
    async getGroups(): Promise<{ status: number; data: TimetableEvents | null }> {
        try {
            const response = await fetch(`${BACKEND_URL}/api/user/calendar`, {
                credentials: "include",
            });

            if (!response.ok) {
                return { status: response.status, data: null };
            }

            const rawData = await response.json();
            
            const parsedData: TimetableEvents = {
                monday: [],
                tuesday: [],
                wednesday: [],
                thursday: [],
                friday: [],
                saturday: [],
                sunday: []
            };

            const dayMap: Record<number, keyof TimetableEvents> = {
                0: "sunday",
                1: "monday",
                2: "tuesday",
                3: "wednesday",
                4: "thursday",
                5: "friday",
                6: "saturday",
            };

            // Backend returns a Calendar object with an 'events' array
            const eventList = Array.isArray(rawData) ? rawData : (Array.isArray(rawData?.events) ? rawData.events : []);

            for (const [index, item] of eventList.entries()) {
                const startTime = new Date(item.start || item.startTime);
                const endTime = new Date(item.end || item.endTime);
                
                if (isNaN(startTime.getTime()) || isNaN(endTime.getTime())) continue;

                const dayOfWeek = startTime.getDay();
                const dayKey = dayMap[dayOfWeek];

                if (dayKey) {
                    parsedData[dayKey].push({
                        id: item.id || `event-${index}`,
                        name: item.name || "Unnamed Event",
                        type: item.type,
                        startTime,
                        endTime,
                        room: item.room
                    });
                }
            }

            return { status: response.status, data: parsedData };
        } catch (error) {
            console.error("Calendar fetch error:", error);
            throw error;
        }
    }
};

