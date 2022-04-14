const baseUrl = "/api/video";

export const getAllVideos = () => {
    return fetch(`${baseUrl}/GetWithComments`).then((res) => res.json());
};

export const addVideo = (video) => {
    video.userProfileId = 1;
    return fetch(baseUrl, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(video),
    });
};

export const searchVideo = (searchTerm) => {
    return fetch(`${baseUrl}/search?q=${searchTerm}`).then((res) => res.json());
};

export const getVideo = (id) => {
    return fetch(`${baseUrl}/getwithcomments/${id}`).then((res) => res.json());
};
