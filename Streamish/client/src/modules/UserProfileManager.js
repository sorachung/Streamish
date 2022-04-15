import { getToken } from "./authManager";

const baseUrl = "/api/userProfile";

export const getWithVideos = (userId) => {
    return getToken().then((token) => {
        return fetch(`${baseUrl}/getwithvideos/${userId}`, {
            method: "GET",
            headers: {
                Authorization: `Bearer ${token}`,
            },
        }).then((res) => {
            if (res.ok) {
                return res.json();
            } else {
                throw new Error(
                    "An unknown error occurred while trying to get quotes."
                );
            }
        });
    });
};
