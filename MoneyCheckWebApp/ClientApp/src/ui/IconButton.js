import React from "react";

import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import classNames from "classnames";

export class IconButton extends React.Component {
    render() {
        return <button onClick={this.props.onClick} className={classNames(this.props.basicLayout ? 'layout-icon-button' : 'icon-button', this.props.className)}>
            <FontAwesomeIcon icon={this.props.icon}/>
        </button>
    }
}