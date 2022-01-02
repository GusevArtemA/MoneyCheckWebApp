import React from "react";

export class Button extends React.Component {
    render() {
        return <button className="brand-button">{this.props.children}</button>
    }
}

export class SubmitButton extends React.Component {
    render() {
        return <input type="submit" className="brand-button" value={this.props.value}/>
    }
}
