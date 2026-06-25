import type { TimetableEvents } from "~/types/timetable";
import type {Group} from "~/types/groups";

export const getBackendUrl = () => {
    if (typeof window !== "undefined" && (window as any).__ENV__?.BACKEND_URL) {
        return (window as any).__ENV__.BACKEND_URL;
    }
    return "https://api.lernzeit.vogel.business";
};

function parseCalendarData(rawData) {
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
                        name: item.name || "",
                        type: item.type,
                        startTime,
                        endTime,
                        room: item.room
                    });
                }
            }
            return parsedData
}

export const apiClient = {
    async loginRaumzeit(username: string, password: string): Promise<boolean> {
        try {
            const response = await fetch(`${getBackendUrl()}/api/user/raumzeit/login`, {
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
            const response = await fetch(`${getBackendUrl()}/api/user/calendar`, {
                credentials: "include",
            });

            if (!response.ok) {
                return { status: response.status, data: null };
            }

            const rawData = await response.json();
            const parsedData = parseCalendarData(rawData)
            
            return { status: response.status, data: parsedData };
        } catch (error) {
            console.error("Calendar fetch error:", error);
            throw error;
        }
    },
    async getGroupCalendar(groupId:string): Promise<{ status: number; data: TimetableEvents | null }> {
        try {
            const response = await fetch(`${getBackendUrl()}/api/group/${groupId}/calendar`, {
                credentials: "omit",
            });
            console.log("resp:", response)

            if (!response.ok) {
                return { status: response.status, data: null };
            }

            const rawData = await response.json();
            const parsedData = parseCalendarData(rawData)

            return { status: response.status, data: parsedData };
        } catch (error) {
            console.error("Calendar fetch error:", error);
            throw error;
        }
    },
    async createGroup(name: string) : Promise<boolean> {
        try {
            console.log("create group")
            const response = await fetch(`${getBackendUrl()}/api/group`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include",
                body: JSON.stringify({ groupName: name }),
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
    
    async getGroups(): Promise<{ status: number; data: Group[] }> {
        try {
            const response = await fetch(`${getBackendUrl()}/api/group`, {
                credentials: "include",
            });

            if (!response.ok) {
                return { status: response.status, data: [] };
            }
            const groupsResponse: Group[] = await response.json();
            
            return { status: response.status, data: groupsResponse };
        } catch (error) {
            console.error("Calendar fetch error:", error);
            throw error;
        }
    },
    async joinGroup(groupId: string) {
        try {
            const response = await fetch(`${getBackendUrl()}/api/group/join/${groupId}`, {
                method: "POST",
                credentials: "include",
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Login failed: ${response.status} ${errorText}`);
            }

            return true;
        } catch (error) {
            console.error("Error joining group", error);
            throw error;
        }
    },
    async leaveGroup(groupId: string) {
        try {
            const response = await fetch(`${getBackendUrl()}/api/group/leave/${groupId}`, {
                method: "POST",
                credentials: "include",
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Error leaving group: ${response.status} ${errorText}`);
            }

            return true;
        } catch (error) {
            console.error("Error leaving group", error);
            throw error;
        }
    }
};

