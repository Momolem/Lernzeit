import React, { useEffect, useState } from 'react';
import logo from './logo.svg';
import './App.css';

interface User {
  isAuthenticated: boolean;
  name?: string;
  email?: string;
  avatar?: string;
}

function App() {
  const [user, setUser] = useState<User | null>(null);

  const BACKEND_URL = process.env.REACT_APP_BACKEND_URL ?? "https://localhost:7113";

  useEffect(() => {
    // Check if user is authenticated on load
    fetch(`${BACKEND_URL}/api/auth/me`, { credentials: 'include' })
      .then(res => res.json())
      .then(data => setUser(data))
      .catch(err => console.error("Failed to fetch auth status", err));
  }, []);

  const login = () => {
    // Redirect to backend login endpoint
    window.location.href = `${BACKEND_URL}/api/auth/login`;
  };

  const logout = () => {
    fetch(`${BACKEND_URL}/api/auth/logout`, { credentials: 'include' })
      .then(() => setUser({ isAuthenticated: false }))
      .catch(err => console.error("Logout failed", err));
  };

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        
        {user?.isAuthenticated ? (
          <div>
            <p>Welcome, {user.name}!</p>
            {user.avatar && <img src={user.avatar} alt="avatar" style={{ borderRadius: '50%', width: '50px' }} />}
            <button onClick={logout} style={{ display: 'block', margin: '10px auto', padding: '10px' }}>Logout</button>
          </div>
        ) : (
          <div>
            <p>You are not logged in.</p>
            <button onClick={login} style={{ padding: '10px 20px', fontSize: '16px' }}>Login with Google</button>
          </div>
        )}

      </header>
    </div>
  );
}

export default App;
