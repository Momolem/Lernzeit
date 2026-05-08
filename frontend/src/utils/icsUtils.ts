import { TimetableEvents, DAYS, type Day } from '../types/timetable';

function formatIcsDate(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  const seconds = String(date.getSeconds()).padStart(2, '0');
  return `${year}${month}${day}T${hours}${minutes}${seconds}`;
}

export function timetableToIcs(
  events: TimetableEvents,
  calendarName: string = 'My Timetable'
): string {
  const lines: string[] = [
    'BEGIN:VCALENDAR',
    'VERSION:2.0',
    'PRODID:-//lernzeit//timetable//EN',
    `X-WR-CALNAME:${calendarName}`,
    'CALSCALE:GREGORIAN',
    'METHOD:PUBLISH',
  ];

  DAYS.forEach((day) => {
    const dayEvents = events[day];
    const referenceDate = getReferenceDateForDay(day);

    dayEvents.forEach((event) => {
      const startDate = combineTimeWithDate(referenceDate, event.startTime);
      const endDate = combineTimeWithDate(referenceDate, event.endTime);

      lines.push('BEGIN:VEVENT');
      lines.push(`UID:${event.id}-${day}@lernzeit`);
      lines.push(`DTSTAMP:${formatIcsDate(new Date())}`);
      lines.push(`DTSTART:${formatIcsDate(startDate)}`);
      lines.push(`DTEND:${formatIcsDate(endDate)}`);
      lines.push(`SUMMARY:${event.name}`);
      if (event.room) {
        lines.push(`LOCATION:${event.room}`);
      }
      lines.push('END:VEVENT');
    });
  });

  lines.push('END:VCALENDAR');
  return lines.join('\r\n');
}

export function icsToTimetable(icsContent: string): TimetableEvents {
  const events: TimetableEvents = {
    monday: [],
    tuesday: [],
    wednesday: [],
    thursday: [],
    friday: [],
    saturday: [],
    sunday: [],
  };

  const lines = icsContent.split(/\r?\n/);
  let currentEvent: { id?: string | number; name?: string; startTime?: Date; endTime?: Date; room?: string } | null = null;
  let inEvent = false;

  for (const line of lines) {
    if (line === 'BEGIN:VEVENT') {
      currentEvent = {};
      inEvent = true;
    } else if (line === 'END:VEVENT' && currentEvent) {
      if (currentEvent.startTime && currentEvent.endTime) {
        const day = determineDayFromDate(currentEvent.startTime);
        if (day && DAYS.includes(day)) {
          events[day].push({
            id: currentEvent.id || Math.random(),
            name: currentEvent.name || 'Unnamed',
            startTime: currentEvent.startTime,
            endTime: currentEvent.endTime,
            room: currentEvent.room,
          });
        }
      }
      currentEvent = null;
      inEvent = false;
    } else if (inEvent && currentEvent) {
      if (line.startsWith('DTSTART:')) {
        currentEvent.startTime = parseIcsDate(line.replace('DTSTART:', ''));
      } else if (line.startsWith('DTEND:')) {
        currentEvent.endTime = parseIcsDate(line.replace('DTEND:', ''));
      } else if (line.startsWith('SUMMARY:')) {
        currentEvent.name = line.replace('SUMMARY:', '');
      } else if (line.startsWith('LOCATION:')) {
        currentEvent.room = line.replace('LOCATION:', '');
      } else if (line.startsWith('UID:')) {
        currentEvent.id = line.replace('UID:', '').replace(/-.*@lernzeit$/, '');
      }
    }
  }

  return events;
}

export function downloadIcsFile(content: string, filename: string = 'timetable.ics'): void {
  const blob = new Blob([content], { type: 'text/calendar;charset=utf-8' });
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = filename;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
}

export function parseIcsFile(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result as string);
    reader.onerror = () => reject(new Error('Failed to read file'));
    reader.readAsText(file);
  });
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

function combineTimeWithDate(date: Date, time: Date): Date {
  const result = new Date(date);
  result.setHours(time.getHours(), time.getMinutes(), 0, 0);
  return result;
}

function parseIcsDate(icsDate: string): Date {
  const match = icsDate.match(/(\d{4})(\d{2})(\d{2})T(\d{2})(\d{2})(\d{2})/);
  if (!match) return new Date();

  const [, year, month, day, hours, minutes, seconds] = match;
  return new Date(
    parseInt(year),
    parseInt(month) - 1,
    parseInt(day),
    parseInt(hours),
    parseInt(minutes),
    parseInt(seconds)
  );
}

function determineDayFromDate(date: Date): Day | null {
  const dayMap: Record<number, Day | null> = {
    0: 'sunday',
    1: 'monday',
    2: 'tuesday',
    3: 'wednesday',
    4: 'thursday',
    5: 'friday',
    6: 'saturday',
  };

  return dayMap[date.getDay()] ?? null;
}