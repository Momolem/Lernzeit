import "./Timetable.module.css"

import {useMemo, useState} from "react";
import {
    Calendar,
    dateFnsLocalizer,
    Views,
    type SlotInfo,
} from "react-big-calendar";
import {format, parse, startOfWeek, getDay} from "date-fns";
import {enUS} from "date-fns/locale";
import "react-big-calendar/lib/css/react-big-calendar.css";
import {
    timetableEventsToCalendarEvents,
    type CalendarEvent,
    DAYS,
    type Day, type TimetableEvents,
} from "~/types/timetable";
import {useTimetableICS} from "~/hooks/useTimetableICS";
import styles from "./Timetable.module.css";

const locales = {"en-US": enUS};

const localizer = dateFnsLocalizer({
    format,
    parse,
    startOfWeek: () => startOfWeek(new Date(), {weekStartsOn: 1}),
    getDay,
    locales,
});

interface TimetableProps {
    initialEvents?: TimetableEvents;
}

interface NewEventFormData {
    title: string;
    startTime: string;
    room: string;
}

export function TimetableComponent({initialEvents}: TimetableProps) {
    const [showModal, setShowModal] = useState(false);
    const [selectedSlot, setSelectedSlot] = useState<{
        start: Date;
        end: Date;
    } | null>(null);
    const [formData, setFormData] = useState<NewEventFormData>({
        title: "",
        startTime: "09:00",
        room: "",
    });

    const defaultEvents: TimetableEvents = useMemo(
        () => ({
            monday: [],
            tuesday: [],
            wednesday: [],
            thursday: [],
            friday: [],
            saturday: [],
            sunday: [],
        }),
        [],
    );

    const {events, error, isLoading, exportToIcs, importFromIcs, updateEvents} =
        useTimetableICS(initialEvents || defaultEvents);

    const calendarEvents = useMemo(
        () => timetableEventsToCalendarEvents(events),
        [events],
    );

    const handleExport = () => {
        exportToIcs("my-timetable.ics");
    };

    const handleImportClick = () => {
        const fileInput = document.createElement("input");
        fileInput.type = "file";
        fileInput.accept = ".ics";
        fileInput.onchange = async (e) => {
            const file = (e.target as HTMLInputElement).files?.[0];
            if (file) {
                await importFromIcs(file);
            }
        };
        fileInput.click();
    };

    const handleSelectSlot = (slotInfo: SlotInfo) => {
        setSelectedSlot({start: slotInfo.start, end: slotInfo.end});
        const hours = slotInfo.start.getHours();
        const minutes = slotInfo.start.getMinutes();
        const timeStr = `${String(hours).padStart(2, "0")}:${String(minutes).padStart(2, "0")}`;
        setFormData({
            title: "",
            startTime: timeStr,
            room: "",
        });
        setShowModal(true);
    };

    const handleSelectEvent = (event: CalendarEvent) => {
        console.log("Event clicked:", event);
        alert(
            `Event: ${event.title}\nRoom: ${event.resource?.room || "No room"}\nTime: ${format(event.start, "HH:mm")} - ${format(event.end, "HH:mm")}`,
        );
    };

    const handleSaveEvent = () => {
        if (!formData.title.trim()) {
            alert("Please enter an event title");
            return;
        }

        if (!selectedSlot) {
            alert("No time slot selected");
            return;
        }

        const [startHours, startMinutes] = formData.startTime
            .split(":")
            .map(Number);
        const duration = selectedSlot.end.getTime() - selectedSlot.start.getTime();
        const startDate = new Date(selectedSlot.start);
        startDate.setHours(startHours, startMinutes, 0, 0);
        const endDate = new Date(startDate.getTime() + duration);

        const day = determineDayFromDate(startDate);
        if (!DAYS.includes(day)) {
            alert("Invalid day");
            return;
        }

        const newEvent = {
            id: Date.now(),
            name: formData.title,
            startTime: startDate,
            endTime: endDate,
            room: formData.room,
        };

        const currentDayEvents = events[day] || [];
        const updatedEvents = {
            ...events,
            [day]: [...currentDayEvents, newEvent],
        };

        updateEvents(updatedEvents);
        setShowModal(false);
        setSelectedSlot(null);
        setFormData({title: "", startTime: "09:00", room: ""});
    };

    const handleCloseModal = () => {
        setShowModal(false);
        setSelectedSlot(null);
        setFormData({title: "", startTime: "09:00", room: ""});
    };

    return (
        <div className={styles.container}>
            <div className={styles.header}>
                <h1 className={styles.title}>My Timetable</h1>
                <div className={styles.actions}>
                </div>
            </div>

            {error && <div className={styles.error}>{error}</div>}
            {isLoading && <div>Loading...</div>}

            <div className={styles.timetableWrapper}>
                <Calendar
                    localizer={localizer}
                    events={calendarEvents}
                    startAccessor="start"
                    endAccessor="end"
                    defaultView={Views.WEEK}
                    views={[Views.WEEK, Views.DAY, Views.MONTH]}
                    step={60}
                    timeslots={1}
                    min={new Date(0, 0, 0, 7, 0, 0)}
                    max={new Date(0, 0, 0, 20, 0, 0)}
                    selectable
                    onSelectSlot={handleSelectSlot}
                    onSelectEvent={handleSelectEvent}
                    style={{height: 700}}
                    eventPropGetter={() => ({
                        style: {
                            backgroundColor: "#4f46e5",
                            borderRadius: "4px",
                        },
                    })}
                />
            </div>

            {showModal && (
                <div className={styles.modalOverlay} onClick={handleCloseModal}>
                    <div className={styles.modal} onClick={(e) => e.stopPropagation()}>
                        <h2 className={styles.modalTitle}>Add New Event</h2>
                        <div className={styles.formGroup}>
                            <label className={styles.label}>Event Title</label>
                            <input
                                type="text"
                                className={styles.input}
                                value={formData.title}
                                onChange={(e) =>
                                    setFormData({...formData, title: e.target.value})
                                }
                                placeholder="e.g., Mathematics"
                                autoFocus
                            />
                        </div>
                        <div className={styles.formGroup}>
                            <label className={styles.label}>Start Time</label>
                            <input
                                type="time"
                                className={styles.input}
                                value={formData.startTime}
                                onChange={(e) =>
                                    setFormData({...formData, startTime: e.target.value})
                                }
                            />
                        </div>
                        <div className={styles.formGroup}>
                            <label className={styles.label}>Room</label>
                            <input
                                type="text"
                                className={styles.input}
                                value={formData.room}
                                onChange={(e) =>
                                    setFormData({...formData, room: e.target.value})
                                }
                                placeholder="e.g., Room 101"
                            />
                        </div>
                        <div className={styles.modalActions}>
                            <button className={styles.button} onClick={handleCloseModal}>
                                Cancel
                            </button>
                            <button
                                className={`${styles.button} ${styles.buttonPrimary}`}
                                onClick={handleSaveEvent}
                            >
                                Save Event
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

function determineDayFromDate(date: Date): Day {
    const dayMap: Record<number, Day> = {
        0: "sunday",
        1: "monday",
        2: "tuesday",
        3: "wednesday",
        4: "thursday",
        5: "friday",
        6: "saturday",
    };
    return dayMap[date.getDay()] || "monday";
}
