import { TimetableComponent } from './components/Timetable/Timetable';
import { TimetableEvents } from './types/timetable';

const sampleEvents: TimetableEvents = {
  monday: [
    {
      id: 1,
      name: 'Mathematics',
      startTime: new Date('2024-01-01T09:00:00'),
      endTime: new Date('2024-01-01T10:30:00'),
      room: 'Room 101',
    },
    {
      id: 2,
      name: 'Physics',
      startTime: new Date('2024-01-01T11:00:00'),
      endTime: new Date('2024-01-01T12:30:00'),
      room: 'Room 203',
    },
  ],
  tuesday: [
    {
      id: 3,
      name: 'Computer Science',
      startTime: new Date('2024-01-02T10:00:00'),
      endTime: new Date('2024-01-02T12:00:00'),
      room: 'Lab 301',
    },
  ],
  wednesday: [
    {
      id: 4,
      name: 'Mathematics',
      startTime: new Date('2024-01-03T09:00:00'),
      endTime: new Date('2024-01-03T10:30:00'),
      room: 'Room 101',
    },
  ],
  thursday: [
    {
      id: 5,
      name: 'Physics',
      startTime: new Date('2024-01-04T11:00:00'),
      endTime: new Date('2024-01-04T12:30:00'),
      room: 'Room 203',
    },
    {
      id: 6,
      name: 'Computer Science',
      startTime: new Date('2024-01-04T14:00:00'),
      endTime: new Date('2024-01-04T16:00:00'),
      room: 'Lab 301',
    },
  ],
  friday: [],
  saturday: [],
  sunday: [],
};

function App() {
  return <TimetableComponent initialEvents={sampleEvents} />;
}

export default App;
