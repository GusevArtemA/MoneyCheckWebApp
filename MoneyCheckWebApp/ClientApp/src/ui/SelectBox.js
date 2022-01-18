import React from "react";

import {faAngleUp} from "@fortawesome/free-solid-svg-icons";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";
import {Box} from "./Box";

export class SelectBox extends React.Component {
    constructor(props) {
        super(props);
        
        this.state = {
            isOpened: false,
            context: this.props.items[0]
        };
    }
    
    render() {
        return <Box className="d-flex flex-column select-box">
            <div className="d-flex flex-row align-items-center justify-content-center" onClick={() => this.setState({ isOpened: !this.state.isOpened })}>
                <span>{this.state.context}</span>
                <FontAwesomeIcon icon={faAngleUp} className={!this.state.isOpened ? 'closed-select-box-arrow' : 'opened-select-box-arrow'}/>    
            </div>
            {this.renderList()}
        </Box>
    }
    
    renderList() {
        return <div className={`d-flex flex-column ${this.state.isOpened ? 'open' : 'closed'}-list-select-box`}>
            {
                this.props.items.map(x => x !== this.state.context ?  <div key={x} onClick={() => {
                    this.setState({ context: x, isOpened: false });
                    this.props.onValueChanged(x);
                }}>
                    <span>{x}</span>
                </div> : null
            )}
        </div>
    }
}