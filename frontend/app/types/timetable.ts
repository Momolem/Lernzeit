import { format, parse, startOfWeek, getDay } from 'date-fns';
import { enUS } from 'date-fns/locale';

export interface TimetableEvent {
  id: number | string;
  name: string;
  type?: string;
  startTime: Date;
  endTime: Date;
  room?: string;
}

export interface TimetableEvents {
  monday: TimetableEvent[];
  tuesday: TimetableEvent[];
  wednesday: TimetableEvent[];
  thursday: TimetableEvent[];
  friday: TimetableEvent[];
  saturday: TimetableEvent[];
  sunday: TimetableEvent[];
  [key: string]: TimetableEvent[];
}

export const DAYS = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'] as const;

export type Day = typeof DAYS[number];

export interface CalendarEvent {
  id: string;
  title: string;
  start: Date;
  end: Date;
  allDay?: boolean;
  resource?: {
    room?: string;
    day?: Day;
  };
}

export const localizer = {
  format,
  parse,
  startOfWeek,
  getDay,
  locales: { enUS },
};

export function timetableEventsToCalendarEvents(events: TimetableEvents): CalendarEvent[] {
  const calendarEvents: CalendarEvent[] = [];

  DAYS.forEach((day) => {
    const dayEvents = events[day];
    const referenceDate = getReferenceDateForDay(day);

    dayEvents.forEach((event) => {
      const startDate = new Date(referenceDate);
      startDate.setHours(
        event.startTime.getHours(),
        event.startTime.getMinutes(),
        0,
        0
      );

      const endDate = new Date(referenceDate);
      endDate.setHours(
        event.endTime.getHours(),
        event.endTime.getMinutes(),
        0,
        0
      );

      calendarEvents.push({
        id: String(event.id),
        title: event.name,
        start: startDate,
        end: endDate,
        resource: {
          room: event.room,
          day: day,
        },
      });
    });
  });

  return calendarEvents;
}

export function calendarEventsToTimetableEvents(calendarEvents: CalendarEvent[]): TimetableEvents {
  const events: TimetableEvents = {
    monday: [],
    tuesday: [],
    wednesday: [],
    thursday: [],
    friday: [],
    saturday: [],
    sunday: [],
  };

  calendarEvents.forEach((event) => {
    const day = event.resource?.day || determineDayFromDate(event.start);
    if (DAYS.includes(day)) {
      events[day].push({
        id: event.id,
        name: event.title,
        startTime: event.start,
        endTime: event.end,
        room: event.resource?.room,
      });
    }
  });

  return events;
}

function getReferenceDateForDay(day: Day): Date {
  const today = new Date();
  const dayIndex = DAYS.indexOf(day);
  const currentDay = today.getDay();
  const diff = (dayIndex + 7 - currentDay) % 7;
  const reference = new Date(today);
  reference.setDate(today.getDate() + diff);
  reference.setHours(0, 0, 0, 0);
  return reference;
}

function determineDayFromDate(date: Date): Day {
  const dayMap: Record<number, Day> = {
    0: 'sunday',
    1: 'monday',
    2: 'tuesday',
    3: 'wednesday',
    4: 'thursday',
    5: 'friday',
    6: 'saturday',
  };
  return dayMap[date.getDay()] || 'monday';
}
