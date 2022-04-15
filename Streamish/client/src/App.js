import React, { useEffect, useState } from "react";
import { BrowserRouter as Router } from "react-router-dom";
import { Spinner } from "reactstrap";
import "./App.css";
import ApplicationViews from "./components/ApplicationViews";
import Header from "./components/Header";
import { onLoginStatusChange } from "./modules/authManager";

function App() {
    const [isLoggedIn, setIsLoggedIn] = useState(null);

    useEffect(() => {
        onLoginStatusChange(setIsLoggedIn);
    }, []);

    if (isLoggedIn === null) {
        // Until we know whether or not the user is logged in or not, just show a spinner
        return <Spinner className="app-spinner dark" />;
    }

    return (
        <div className="App">
            <Router>
                <Header isLoggedIn={isLoggedIn} />
                <ApplicationViews isLoggedIn={isLoggedIn} />
            </Router>
        </div>
    );
}

export default App;
