import React, { useEffect, useState } from "react";
import Video from "./Video";
import { getAllVideos } from "../modules/VideoManager";
import { VideosSearch } from "./VideosSearch";

const VideoList = () => {
    const [videos, setVideos] = useState([]);

    const getVideos = () => {
        getAllVideos().then((videos) => setVideos(videos));
    };

    useEffect(() => {
        getVideos();
    }, []);

    return (
        <>
            <VideosSearch
                videos={videos}
                setVideos={setVideos}
                getVideos={getVideos}
                key="videoSearch"
            />
            <div className="container">
                <div className="row justify-content-center">
                    {videos.map((video) => (
                        <>
                            <Video video={video} key={video.id} />
                        </>
                    ))}
                </div>
            </div>
        </>
    );
};

export default VideoList;
