import { getToken } from "./authManager";

const baseUrl = "/api/video";

export const getAllVideos = () => {
    return getToken().then((token) => {
        return fetch(`${baseUrl}/GetWithComments`, {
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

export const addVideo = (video) => {
    return getToken().then((token) => {
        return fetch(baseUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(video),
        }).then((resp) => {
            if (resp.ok) {
                return resp.json();
            } else if (resp.status === 401) {
                throw new Error("Unauthorized");
            } else {
                throw new Error(
                    "An unknown error occurred while trying to save a new quote."
                );
            }
        });
    });
};

export const searchVideo = (searchTerm) => {
    return getToken().then((token) => {
        return fetch(`${baseUrl}/search?q=${searchTerm}`, {
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

export const getVideo = (id) => {
    return getToken().then((token) => {
        return fetch(`${baseUrl}/getwithcomments/${id}`, {
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
