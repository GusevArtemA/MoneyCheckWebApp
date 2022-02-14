import React from "react";
import AnimatedLogoSvg from "../assets/images/animated/animated-logo.svg";

export function AnimatedLogo(props) {
    return <object {...props} width={props.width ?? "75"} type="image/svg+xml" data={AnimatedLogoSvg}>Animated Logo</object>;
}