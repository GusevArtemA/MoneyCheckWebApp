import {useState} from "react";
import React from "react";
import classNames from "classnames";

export function ImagesSlider(props) {
    const [index, setIndex] = useState(0);
    const maxIndex = props.items.length - 1;
    
    const [leftMouseEnter, setMouseEnterLeftState] = useState(false);
    const [rightMouseEnter, setMouseEnterRightState] = useState(false);
    
    let prevIndex = index === 0 ? maxIndex : index - 1;
    let nextIndex = index === maxIndex ? 0 : index + 1;
    
    function slideForward() {
        setIndex(nextIndex);
        props.onValueChanged(nextIndex);
    }
    
    function slideBackwards() {
        setIndex(prevIndex);
        props.onValueChanged(prevIndex);
    }
    
    function onLeftMouseEnterHandler() {
        setMouseEnterLeftState(true);
    }

    function onLeftMouseLeaveHandler() {
        setMouseEnterLeftState(false);
    }

    function onRightMouseEnterHandler() {
        setMouseEnterRightState(true);
    }

    function onRightMouseLeaveHandler() {
        setMouseEnterRightState(false);
    }
    
    return <div className="d-flex flex-row images-slider-container">
        <div className={classNames("left-slide-button", leftMouseEnter && "selected-slider-button")}
             onMouseEnter={onLeftMouseEnterHandler}
             onMouseLeave={onLeftMouseLeaveHandler}
            onClick={slideBackwards}>
            
        </div>
        <div className={classNames("right-slide-button", rightMouseEnter && "selected-slider-button")}
             onMouseEnter={onRightMouseEnterHandler}
             onMouseLeave={onRightMouseLeaveHandler}
            onClick={slideForward}>
            
        </div>
        <ImageCard url={props.items[index].url}/>
    </div>
}

function ImageCard(props) {
    return <img className="slider-card" src={props.url} alt="Card" width="66" {...props}/>
}