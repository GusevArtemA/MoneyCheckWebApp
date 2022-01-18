import React from "react";

import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";

export class IconButton extends React.Component {
    render() {
        return <button onClick={this.props.onClick} className={this.props.basicLayout ? 'layout-icon-button' : 'icon-button'}>
            <FontAwesomeIcon icon={this.props.icon}/>
        </button>
    }
}