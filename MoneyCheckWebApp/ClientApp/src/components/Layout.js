import React, { Component } from 'react';
import "../assets/scss/layout.scss";
import "../assets/scss/main.scss";
import {Container} from "reactstrap";

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <Container>
          {this.props.children}
      </Container>
    );
  }
}
