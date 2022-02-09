import React from "react";

import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import classNames from "classnames";

export class Box extends React.Component {
    render() {
        return (
            <div className={classNames('box', this.props.className)}>
                {
                    this.props.rightButton != null && <button className="right-box-button" onClick={this.props.onRightButtonClick}> 
                        <FontAwesomeIcon icon={this.props.rightButton}/>
                    </button>
                }
                {this.props.children}
            </div>
        )
    }
}