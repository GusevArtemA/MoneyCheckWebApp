import React from "react";

export class Button extends React.Component {
    render() {
        return <button {...this.props} className={`brand-button ${this.props.className}`}>{this.props.children}</button>
    }
}

export class SubmitButton extends React.Component {
    render() {
        return <input type="submit" className="brand-button" value={this.props.value}/>
    }
}

export function LinkButton(props) {
    return <a href={props.href} className="brand-button">{props.children}</a>
}