import React from "react";

export class Loader extends React.Component {
    render() {
        return (
            <div className="d-flex flex-row loader justify-content-around">
                <div className="first-circle">
                </div>
                <div className="second-circle">
                </div>
                <div className="third-circle">
                </div>
            </div>
        );
    }
}