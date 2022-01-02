import React, { Component } from 'react';
import "../assets/scss/layout.scss";
import "../assets/scss/main.scss";

export class Layout extends Component {
  static displayName = Layout.name;

  render () {
    return (
      <>
          {this.props.children}
      </>
    );
  }
}
