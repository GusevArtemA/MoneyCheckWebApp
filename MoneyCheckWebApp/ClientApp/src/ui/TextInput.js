import React from "react";

export class TextInput extends React.Component {
    render() {
        return (
            <div className="brand-text-input-pre-wrapper">
                <span className="brand-text-input-error-span" ref={this.props.errorRef}/>
                <div className="brand-text-input-wrapper">
                    <input type={this.props.type ?? 'text'}
                           ref={this.props.inputRef}
                           className="brand-text-input"
                           name={this.props.name}
                           id={this.props.id}
                           placeholder={this.props.placeholder}
                            {...this.props}/>
                </div>
            </div>
        );
    }
}