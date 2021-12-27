import React, { Component } from 'react';
import { Route } from 'react-router-dom';

import "./brand-assetes.scss";
import {Auth} from "./components/Auth";

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
        <Route exact path='/auth' component={Auth} />
    );
  }
}
