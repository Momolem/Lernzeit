import { render, screen } from '@testing-library/react';
import App from './App';

test('renders timetable', () => {
  render(<App />);
  const titleElement = screen.getByText(/My Timetable/i);
  expect(titleElement).toBeInTheDocument();
});