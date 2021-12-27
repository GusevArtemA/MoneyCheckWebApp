import React from "react";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import "../../assets/scss/buttons/basic-button-assets.scss";
import "../../assets/scss/buttons/icon-button-style.scss";

export class IconButton extends React.Component {
    render() {
        return (
            <button className={`icon-button filled-${this.props.color ?? 'green'}`}>
                <FontAwesomeIcon icon={this.props.icon}/>
            </button>
        );
    }
}