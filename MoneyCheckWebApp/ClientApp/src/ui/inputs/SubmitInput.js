import React from "react";

import "../../assets/scss/inputs/submit-input-style.scss";

export class SubmitInput extends React.Component {
    render() {
        return <input className={`flat-submit filled-${this.props.color ?? 'green'}`} type={"submit"} value={this.props.children}/>
    }
}