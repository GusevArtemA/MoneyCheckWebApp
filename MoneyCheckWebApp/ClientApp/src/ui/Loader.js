import React from "react";
import LoadingIcon from "../assets/images/animated/loader.svg";

export class Loader extends React.Component {
    render() {
        return <object width="100" type="image/svg+xml" data={LoadingIcon}>svg-animation</object>
    }
}