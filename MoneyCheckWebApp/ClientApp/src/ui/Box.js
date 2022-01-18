import React from "react";

import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";

export class Box extends React.Component {
    render() {
        return (
            <div {...this.props} className={`box${this.props.className == null ?  '' : ' ' + this.props.className}`}>
                {
                    this.props.rightButton != null ? <button className="right-box-button" onClick={this.props.rightButtonClick}> 
                        <FontAwesomeIcon icon={this.props.rightButton}/>
                    </button> : null
                }
                {this.props.children}
            </div>
        )
    }
}