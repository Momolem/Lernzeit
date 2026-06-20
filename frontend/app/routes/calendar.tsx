import {useEffect, useState} from "react";
import {TimetableComponent} from "~/components/timetable/timetable";
import type {TimetableEvents} from "~/types/timetable";
import Button from "~/components/button/button";
import Input from "~/components/input/input";
import {Modal} from "~/components/modal/modal";
import {apiClient} from "~/api/client";

export const handle = {
    displayName: "Kalender"
};

export default function Calendar() {
    const [events, setEvents] = useState<TimetableEvents | null>(null);
    const [status, setStatus] = useState<"loading" | "success" | "unauthorized" | "not-found">("loading");
    const [isLoginModalOpen, setIsLoginModalOpen] = useState(false);
    const [loginError, setLoginError] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);
    
    // For the modal forms
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [icalLink, setIcalLink] = useState("");

    const BACKEND_URL = import.meta.env.REACT_APP_BACKEND_URL ?? "https://localhost:7113";

    useEffect(() => {
        apiClient.getCalendar()
            .then(({ status, data }) => {
                if (status === 401) {
                    setStatus("unauthorized");
                } else if (status === 404) {
                    setStatus("not-found");
                } else if (status >= 200 && status < 300 && data) {
                    setEvents(data);
                    setStatus("success");
                } else {
                    setStatus("not-found");
                }
            })
            .catch(err => {
                console.error("Calendar fetch error:", err);
                setStatus("not-found"); 
            });
    }, []);

    const handleGoogleLogin = () => {
        window.location.href = `${BACKEND_URL}/api/auth/login`;
    };

    const handleCredentialsSubmit = async () => {
        setIsSubmitting(true);
        setLoginError("");
        try {
            await apiClient.loginRaumzeit(username, password);
            setIsLoginModalOpen(false);
            window.location.reload(); // Reload to fetch the newly connected calendar
        } catch (error) {
            setLoginError("Login fehlgeschlagen. Bitte überprüfe deine Zugangsdaten.");
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleIcalSubmit = () => {
        // Implement ical submit
        console.log("Submit ical link", icalLink);
    };

    if (status === "loading") {
        return <div>Lade Kalender...</div>;
    }

    if (status === "unauthorized") {
        return (
            <div className="flex flex-col items-center justify-center h-full gap-4">
                <h2>Bitte melde dich an, um deinen Kalender zu sehen</h2>
                <Button onClick={handleGoogleLogin}>Login with Google</Button>
            </div>
        );
    }

    if (status === "not-found") {
        return (
            <div className="flex flex-col items-center justify-center h-full gap-4">
                <h2>Kein Kalender gefunden</h2>
                <p>Verbinde deinen Kalender, um deine Termine zu sehen.</p>
                <Button onClick={() => setIsLoginModalOpen(true)}>Kalender verbinden</Button>
                
                <Modal isOpen={isLoginModalOpen} onClose={() => setIsLoginModalOpen(false)} title="Kalender verbinden">
                    <div className="flex flex-col gap-6">
                        <div className="flex flex-col gap-2">
                            <h3 className="font-semibold">Mit Zugangsdaten anmelden</h3>
                            <Input 
                                type="text" 
                                placeholder="Benutzername" 
                                value={username} 
                                onChange={(e) => setUsername(e.target.value)} 
                            />
                            <Input 
                                type="password" 
                                placeholder="Passwort" 
                                value={password} 
                                onChange={(e) => setPassword(e.target.value)} 
                            />
                            {loginError && <p className="text-red-500 text-sm">{loginError}</p>}
                            <Button onClick={handleCredentialsSubmit} disabled={isSubmitting}>
                                {isSubmitting ? "Wird angemeldet..." : "Anmelden"}
                            </Button>
                        </div>
                        
                        <div className="flex flex-col gap-2">
                            <h3 className="font-semibold">Alternativ: iCal-Link angeben</h3>
                            <Input 
                                type="url" 
                                placeholder="https://..." 
                                value={icalLink} 
                                onChange={(e) => setIcalLink(e.target.value)} 
                            />
                            <Button onClick={handleIcalSubmit} variant="secondary">Link speichern</Button>
                        </div>
                    </div>
                </Modal>
            </div>
        );
    }

    return (
        <div className="h-full w-full">
            <TimetableComponent initialEvents={events || undefined} />
        </div>
    );
}
