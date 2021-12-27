import React from "react";

import "../../assets/scss/inputs/float-input-style.scss";

export class TextInput extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            inputState: 'bottom',
            isError: false
        };
        
        this.onInputFocusIn = this.onInputFocusIn.bind(this);
        this.onFocusOut = this.onFocusOut.bind(this); 
        this.setError = this.setError.bind(this);
    }
    
    render() {
        return (
            <div onClick={(e) => {
                let target = e.target;

                while(target.className !== 'float-input') {
                    target = target.parentNode;
                }

                target.querySelector('input').focus();
            }} className={"float-input"}>
                <label className={`label-${this.state.inputState}`}>
                    {this.props.placeholder}
                </label>
                <input onFocus={this.onInputFocusIn}
                       onBlur={this.onFocusOut}
                       type={this.props.type ?? 'text'}
                        className={this.state.isError ? 'error-float-input' : ''}
                        name={this.props.name}
                        ref={this.props.inputRef}/>
            </div>
        );
    }
    
    onInputFocusIn(e) {
        this.setState({
            inputState: 'top'
        });
    }
    
    onFocusOut(e) {
        let targetedInput = e.target;

        if(targetedInput.value === '') {
            this.setState({
                inputState: 'bottom'
            });
        }
    }
    
    setError() {
        this.setState({
            isError: true
        });
    }
}