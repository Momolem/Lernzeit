import { useEffect } from "react";
import { useParams, useNavigate } from "react-router";
import {apiClient} from "~/api/client";

export default function JoinGroup() {
    const { groupId } = useParams();
    const navigate = useNavigate();

    useEffect(() => {
        const joinGroup = async () => {
            if (!groupId) {
                navigate("/");
                return;
            }
            
            try {
                await apiClient.joinGroup(groupId);
                console.log(`Attempting to join group: ${groupId}`);
                
                // Simulate network delay to prevent jarring flash
                await new Promise((resolve) => setTimeout(resolve, 1500));
            } catch (error) {
                console.error("Failed to join group", error);
            } finally {
                // Redirect to home screen after attempting to join
                navigate("/");
            }
        };

        joinGroup();
    }, [groupId, navigate]);

    return (
        <div className="flex flex-col items-center justify-center min-h-[calc(100vh-100px)] p-6">
            <div className="bg-white p-8 rounded-xl shadow-sm border border-gray-100 text-center max-w-sm w-full">
                <h1 className="text-2xl font-bold text-gray-800 mb-2">Joining Group...</h1>
                <p className="text-gray-500 mb-8">Please wait while we set things up.</p>
                
                <div className="flex justify-center">
                    <div className="animate-spin rounded-full h-10 w-10 border-4 border-gray-200 border-t-blue-600"></div>
                </div>
            </div>
        </div>
    );
}