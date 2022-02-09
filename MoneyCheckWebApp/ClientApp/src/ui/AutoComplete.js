import {useRef, useState} from "react";
import React from "react";
import classNames from "classnames";

export function AutoComplete(props) {
    const [items, setItems] = useState(props.items);
    const inputRef = useRef();
    const contextRef = useRef();
    
    const [blurInput, setBlurInput] = useState(true);
    
    const inputRefreshHandler = function () {
        let value = inputRef.current.value;
        
        if(value === '') {
            setItems(props.items);
        }
        
        setItems(props.items.filter(a => a.startsWith(value)));
        
        if(props.onInputValueCanged) {
            props.onInputValueCanged(value);    
        }
    }
    
    const onValueSelectedHandler = function () {
        setBlurInput(true);
    }
    
    const onFocusInput = function () {
        setBlurInput(false);
    }
    
    document.addEventListener('click', function(e) {
        let current = contextRef.current;
        let context = e.target;
        
        let contextTarget = false;
        
        while(context != null) {
            if(context === current) {
                contextTarget = true;
                break;
            }
            
            context = context.parentNode;
        }
        
        if(!contextTarget) {
            setBlurInput(true)
        }
    })
    
    return <div 
            ref={contextRef}
            className={classNames("auto-complete-wrapper", !blurInput ? "shown-state" : "hidden-state")}>
        <input type="text"
                id={props.id}
                onKeyUp={inputRefreshHandler}
                ref={inputRef}
                className="auto-complete-input"
                onFocus={onFocusInput}
                placeholder="Начинайте вводить"
                autoComplete="off"/>
        <div className={!blurInput ? 'd-block' : 'd-none'}>
            <div className={"can-hide d-flex flex-column auto-complete-options-container"}>
                {items.map(x => <OptionContainer item={x} inputRef={inputRef} onValueSelected={onValueSelectedHandler}/>)}
            </div>    
        </div>
    </div>
}

function OptionContainer (props) {
    const clickHandler = function () {
        props.inputRef.current.value = props.item;
        props.onValueSelected();
    }
    
    return <div onClick={clickHandler} className="autocomplete-option">
        <span>{props.item}</span>
    </div>
}