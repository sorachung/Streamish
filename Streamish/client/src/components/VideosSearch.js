import React, { useEffect, useState } from "react";
import { searchVideo } from "../modules/VideoManager";

export const VideosSearch = ({ setVideos, getVideos }) => {
    const [searchTerm, setSearchTerm] = useState("");

    useEffect(() => {
        if (searchTerm === "") {
            getVideos();
        } else {
            searchVideo(searchTerm).then((data) => setVideos(data));
        }
    }, [searchTerm]);
    return (
        <div className="search-bar">
            <label htmlFor="video-search">Search for videos:</label>
            <input
                type="search"
                id="video-search"
                onKeyUp={(event) => {
                    setSearchTerm(event.target.value);
                }}
            />
        </div>
    );
};
