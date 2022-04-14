const baseUrl = "/api/userProfile";

export const getWithVideos = (userId) => {
    return fetch(`${baseUrl}/getwithvideos/${userId}`).then((res) =>
        res.json()
    );
};
