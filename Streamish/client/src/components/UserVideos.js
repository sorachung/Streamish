import React, { useEffect, useState } from "react";
import Video from "./Video";
import { getWithVideos } from "../modules/UserProfileManager";
import { useParams } from "react-router-dom";

const UserVideos = () => {
    const [user, setUser] = useState({});
    const [videos, setVideos] = useState([]);
    const { id } = useParams();

    const getUser = (id) => {
        getWithVideos(id).then((userData) => setUser(userData));
    };

    useEffect(() => {
        getUser(id);
    }, []);

    useEffect(() => {
        if (user.videos) {
            const videosFromUser = user.videos;
            for (const video of videosFromUser) {
                video.userProfile = {};
                video.userProfile.name = user.name;
            }
            setVideos(videosFromUser);
        }
    }, [user]);

    return (
        <div className="container">
            <div className="row justify-content-center">
                {videos?.map((video) => (
                    <>
                        <Video video={video} key={video.id} />
                    </>
                ))}
            </div>
        </div>
    );
};

export default UserVideos;
