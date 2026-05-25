import { useCallback, useState } from 'react';
import type {TimetableEvents} from '~/types/timetable';
import {
  timetableToIcs,
  icsToTimetable,
  downloadIcsFile,
  parseIcsFile,
} from '~/utils/icsUtils';

export function useTimetableICS(initialEvents: TimetableEvents) {
  const [events, setEvents] = useState<TimetableEvents>(initialEvents);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const exportToIcs = useCallback(
    (filename?: string) => {
      try {
        setError(null);
        const icsContent = timetableToIcs(events, 'My Timetable');
        downloadIcsFile(icsContent, filename || 'timetable.ics');
      } catch (err) {
        setError('Failed to export timetable');
        console.error(err);
      }
    },
    [events]
  );

  const importFromIcs = useCallback(async (file: File) => {
    try {
      setError(null);
      setIsLoading(true);
      const icsContent = await parseIcsFile(file);
      const importedEvents = icsToTimetable(icsContent);
      setEvents(importedEvents);
    } catch (err) {
      setError('Failed to import timetable. Please check the file format.');
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const updateEvents = useCallback((newEvents: TimetableEvents) => {
    setEvents(newEvents);
  }, []);

  const addEvent = useCallback(
    (day: keyof TimetableEvents, event: TimetableEvents[keyof TimetableEvents][0]) => {
      setEvents((prev) => ({
        ...prev,
        [day]: [...prev[day], event],
      }));
    },
    []
  );

  const removeEvent = useCallback(
    (day: keyof TimetableEvents, eventId: number | string) => {
      setEvents((prev) => ({
        ...prev,
        [day]: prev[day].filter((e) => e.id !== eventId),
      }));
    },
    []
  );

  return {
    events,
    error,
    isLoading,
    exportToIcs,
    importFromIcs,
    updateEvents,
    addEvent,
    removeEvent,
  };
}