import React from "react";

export class Box extends React.Component {
    render() {
        return (
            <div {...this.props} className={`box${this.props.className == null ?  '' : ' ' + this.props.className}`}>
                {this.props.children}
            </div>
        )
    }
}