import React, { Component } from 'react';
import {Redirect, Route} from 'react-router-dom';
import {Login} from "./components/Login";
import {Layout} from "./components/Layout";
import {Welcome} from "./components/Welcome";
import {Home} from "./components/Home";

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
        <Layout>
            <Route exact path='/login' component={Login} />
            <Route exact path='/welcome' component={Welcome} />
            <Route exact path='/home' component={Home} />
        </Layout>
    );
  }
}
